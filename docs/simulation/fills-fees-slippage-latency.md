# Fills, Fees, Slippage, and Latency

Strategies submit order intents. The simulator turns those intents into execution events only after admission, status checks, policy checks, matching, fees, and slippage are applied.

```csharp
partial void OnBar(ref BarContext bar)
{
    bar.Buy(new Qty(10m), Execution.Limit().At(new Price(123m, Currency.USD)));
}
```

After the run, inspect both sides of the lifecycle:

```csharp
var submits = result.OrderIntents;
var fills = result.ExecutionEvents.OfType<OrderFilled>();
var rejections = result.ExecutionEvents.OfType<OrderRejected>();
```

`OrderIntents` are strategy requests: submit, cancel, and modify. `ExecutionEvents` are exchange-side outcomes: accepted, modified, rejected, filled, package-leg-filled, cancelled, or expired.

## Lifecycle Events

`OrderAccepted` means the simulated venue acknowledged the order and opened it.

`OrderFilled` means some quantity traded. A fill may be partial or complete depending on the matching mode and simulation config. Queue-accurate matching can produce partial trade fills when configured for that behavior; fast vector same-event fills are full-quantity in the tested path.

`OrderCancelled` covers user cancellations plus paths such as IOC/FOK or exhausted available liquidity.

`OrderRejected` means the order never became open and fillable. Common causes include closed, halted, or pre-open market status; unsupported policy or time-in-force combinations; invalid order parameters; post-only orders that would take liquidity; and account or reference-data failures.

## Fill Shape

An `OrderFilled` includes the order, instrument, variant, strategy, side, filled quantity, fill price, commission, execution id, venue order id, optional asset id, and event time. Its value is:

```csharp
var value = fill.FilledQty * fill.FillPrice;
```

Fees and slippage are visible on the fill itself:

```csharp
foreach (var fill in result.ExecutionEvents.OfType<OrderFilled>())
{
    Console.WriteLine($"{fill.Side} {fill.FilledQty} @ {fill.FillPrice}");
    Console.WriteLine($"Commission: {fill.Commission}");
}
```

The simulator computes price improvement first, then slippage, then commission. Slippage models include none, volume-proportional, and volatility-adjusted behavior. Buy slippage increases the fill price; sell slippage decreases it, without going below zero. Fee models include percentage of value, per quantity, per trade, tiered by volume, directional, contract terms, and maker/taker-style helpers.

Configure those through `SimulationConfig` and pass it with `WithConfig(...)`, or override config at the venue or instrument level.

## Strategy Callbacks

Execution events also reach strategy callbacks:

```csharp
protected override void OnOrderFilled(ref FillContext fill)
{
    var current = fill.Position;
}

protected override void OnPositionOpened(ref PositionContext position) { }
protected override void OnPositionChanged(ref PositionContext position) { }
protected override void OnPositionClosed(ref PositionContext position) { }
```

Position callbacks receive previous and current strategy-local position state. Position state includes quantity, average entry price, and realized PnL; commissions are applied to realized PnL.

`FillContext` exposes strategy id, order id, asset id, side, filled quantity, fill price, commission, and current position. It also exposes helper methods to submit or manage orders from the fill callback when that is part of the strategy design.

`OrderContext` is used by order lifecycle callbacks such as accepted, rejected, cancelled, modified, and expired. It exposes strategy, order, status, variant, asset, reason, and helpers to cancel or modify the referenced order.

## Order Helper Surface

Common strategy helpers:

```csharp
Portfolio.Buy(asset, new Qty(10m), Execution.Market());
Portfolio.Sell(asset, new Qty(10m), Execution.Limit().At(new Price(101.25m, Currency.USD)));
Portfolio.Cancel(asset, orderId, "risk limit");
Portfolio.Modify(asset, orderId, newQuantity: new Qty(5m), newLimitPrice: new Price(101.00m, Currency.USD));
```

Visible execution helpers include `Market`, `Limit`, `MarketToLimit`, `StopMarket`, `StopLimit`, `TrailingStop`, `TrailingStopLimit`, `MarketIfTouched`, `LimitIfTouched`, `Twap`, `Vwap`, and `Pov`.

`ExecutionSpec` fluent settings include `AtBid`, `AtAsk`, `AtMid`, `At(price)`, `GoodTilCancelled`, `GoodTil`, `ImmediateOrCancel`, `WithPostOnly`, `WithMaxSlippageTicks`, `Over`, `Every`, `MaxParticipation`, `Display`, and `WithStopPrice`.

## Latency

Latency is part of `SimulationConfig`, so it is configured with the same defaults and venue/instrument override model as fees and slippage. Read latency behavior from execution events and diagnostics rather than from order intents: intents show what the strategy asked for, while execution events show what the simulated venue accepted, rejected, cancelled, or filled.

Avoid treating fills as raw market trades. A market-data `TradeOccurred` is replay input; an `OrderFilled` is the simulated execution outcome for your strategy.

Cookbook examples:

- `cookbook/GettingStarted/04-order-intents.cs`
- `cookbook/StrategyAuthoring/tick-quote-trade-hooks.cs`
- `cookbook/GettingStarted/06-venue-config.cs`
- `cookbook/Simulation/slippage-fees-latency.cs`
