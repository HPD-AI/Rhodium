# Orders And Positions

Rhodium keeps strategy requests, venue order state, and execution outcomes as
separate objects.

## Execution Specs

`ExecutionSpec` is the strategy-facing execution request shape. It covers order
type, limit price or price mode, time-in-force, post-only behavior, max slippage
ticks, TWAP/VWAP/POV metadata, stops, GTD, display quantity, and trailing-stop
metadata.

Use `Execution` helpers for common specs.

```csharp
partial void OnTick(ref TickContext tick)
{
    if (tick.BookSpreadTicks <= 1)
        tick.Buy(new Qty(1m), Execution.Limit().AtBid().WithPostOnly());
    else
        tick.Sell(new Qty(1m), Execution.Twap().Over(Duration.FromSeconds(30)));
}
```

## Order Intents

Context helpers emit `OrderIntent`s. An intent records `StrategyId`, `AssetId`,
side, quantity, execution spec, and whether the request is submit, cancel, or
modify.

```csharp
partial void OnBar(ref BarContext bar)
{
    bar.Buy(new Qty(10m), Execution.Market());
    bar.Modify(orderId, newLimitPrice: new Price(99.50m, Currency.USD));
    bar.Cancel(orderId, "signal expired");
}
```

An `OrderIntent` is not a durable exchange order and not a fill. It is the
strategy request that a venue or simulation layer can process.

For a runnable order-intent walkthrough, see
`cookbook/GettingStarted/04-order-intents.cs` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted).

## Venue Orders

`Order` is mutable venue/order state after submission. It contains identity,
side, quantity, order type, limit/stop prices, time-in-force, GTD, `VariantId`,
numeric tags, HFT timestamps, tick/lot metadata, queue position, maker flag,
fill tracking, trailing-stop metadata, order-list membership, display quantity,
and execution-algorithm metadata.

State transitions are explicit:

- `Accept`
- `Reject`
- `Fill`
- `Cancel`
- `Expire`

Execution events are the venue outcomes. `OrderAccepted`, `OrderModified`,
`OrderRejected`, `OrderFilled`, `OrderCancelled`, and `OrderExpired` are distinct
from both `OrderIntent` and `Order`.

```csharp
var fills = result.ExecutionEvents.OfType<OrderFilled>();
var rejected = result.ExecutionEvents.OfType<OrderRejected>();
```

## Positions

`Position` tracks an instrument, signed quantity, average entry price, realized
P&L, open/close timestamps, and derived flat/long/short/side state.

Fills update average price when adding and realize P&L when reducing or
reversing. Positions can also apply transfers and split adjustments.

```csharp
if (bar.PositionQuantity == 0m)
    bar.Buy(new Qty(1m));
else
    bar.Flatten();
```

There is no single universal position key. Strategy runtime state is scoped by
`StrategyId + AssetId.VirtualIndex`; simulation account snapshots are scoped by
`StrategyId + VariantId + Instrument`.

## Simulation Results

Simulation results bridge request, execution, account state, and performance.

```csharp
var result = Rhodium.Simulation.Rhodium.Simulate<MyStrategy>()
    .WithHistory(history)
    .Run();

var best = result.TopBySharpe(5);
var intents = result.OrderIntents;
var fills = result.ExecutionEvents.OfType<OrderFilled>();
var statements = result.AccountStatements;
```

`SimulationResult` contains per-strategy/per-variant `Runs`, the batch tear
sheet, captured `OrderIntents`, `ExecutionEvents`, `AccountStatements`,
`SimulatorEvents`, and `Diagnostics`. Each `StrategyRunResult` carries
`StrategyId`, `VariantIndex`, parameter values, a tear sheet, and the final
strategy-local portfolio snapshot.
