# Matching Fidelity

Matching fidelity is the simulation policy that decides when an accepted order can trade against replayed market data. It is configured as a run, venue, or instrument setting; it is not a different simulation API.

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.QueueAccurate)
    .Run();
```

`QueueAccurate` is the default. If you do not set a fidelity mode on the run, venue, or instrument, simulated venues use queue-accurate matching.

## Modes

Use `QueueAccurate` for normal backtests where resting-order timing matters. A limit order submitted from a bar does not fill just because that same bar touched the price. It must wait for later replay events that can execute it. When the simulation config uses partial fills on trades, later trade events can fill an order in pieces.

Use `FastVectorApproximation` for fast screening runs. If the current replay event touches a submitted order and execution is allowed, the simulator can accept and fill that order inside the same event boundary. Tests also pin that this mode fills the full quantity in that path; do not use it to study queue position or partial-fill behavior.

Use `MarketByOrder` only when the replay stream contains book or market-by-order information you want the simulator to consume. The engine maintains book liquidity from L3/book frames and can use that liquidity for market-order execution, including cancelling unfilled remainder when the replay book is exhausted. Treat it as book-driven matching, not as a blanket promise of venue-exact microstructure.

## Override Scope

Set the run default with `WithMatchingFidelity(...)`:

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .Run();
```

Override a venue when one venue should use a different execution model:

```csharp
builder.WithVenue(SimulationVenueConfig.For(venue) with
{
    MatchingFidelity = MatchingFidelity.QueueAccurate
});
```

Override an instrument when only one contract needs special treatment:

```csharp
builder.WithInstrument(new SimulationInstrumentConfig(
    Instrument: instrument,
    Contract: contract)
{
    MatchingFidelity = MatchingFidelity.MarketByOrder
});
```

Instrument settings are the most specific; venue settings override the run default for that venue; the run setting is the fallback.

## Status Gates

Market status gates all fidelity modes. If the simulated instrument or venue is not `Open`, orders are rejected before they can become open or fillable. Tests cover `PreOpen`, `Closed`, and `Halted` status paths. In those cases, expect `OrderRejected` execution events, not `OrderAccepted` or `OrderFilled`.

Status can come from simulation config defaults, venue or instrument configuration, or replayed status events. When a run unexpectedly has no fills, check `result.ExecutionEvents` for rejections and `result.Diagnostics` for the venue and instrument state that was active during the run.

## Choosing a Mode

Start with `QueueAccurate`. It is the default because it avoids same-event fills and gives resting orders a more conservative lifecycle.

Switch to `FastVectorApproximation` when you are sweeping many parameter combinations and care more about speed than queue realism. Use it to narrow candidates, then rerun the candidates with `QueueAccurate` before reading fill timing too closely.

Use `MarketByOrder` when your data source carries book depth or L3 order information and the strategy behavior depends on consuming visible book liquidity.

Cookbook examples:

- `cookbook/Simulation/queue-accurate-backtest.cs`
- `cookbook/Simulation/fast-vector-backtest.cs`
- `cookbook/GettingStarted/06-venue-config.cs`
