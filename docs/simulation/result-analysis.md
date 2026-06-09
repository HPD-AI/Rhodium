# Result Analysis

`SimulationResult` is the object to read after a simulation run. It contains per-run strategy summaries, batch-level metrics, captured order intents, exchange execution events, account statements, simulator events, and diagnostics.

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithHistory(history)
    .Run();
```

## Result Shape

Use `Runs` for per-strategy or per-variant output:

```csharp
foreach (var run in result.Runs)
{
    Console.WriteLine(run.StrategyId);
    Console.WriteLine(run.VariantIndex);
    Console.WriteLine(run.TearSheet.TotalReturn);
}
```

Each `StrategyRunResult` has:

- `StrategyId`
- `VariantIndex`
- `Parameters`
- `TearSheet`
- `FinalSnapshot`

`Batch` is the batch-level tear sheet summary across the result.

`OrderIntents` are the strategy-side submit, cancel, and modify requests captured during the run.

`ExecutionEvents` are the simulated exchange outcomes, including accepted, modified, rejected, filled, package-leg-filled, cancelled, and expired events.

`AccountStatements` are venue account snapshots emitted by account activity.

`SimulatorEvents` are non-execution simulator finance events. Raw replay bars, quotes, trades, and similar market events are projected through the simulation; do not expect them to be retained here as a market-data log.

`Diagnostics` includes venue, instrument, latency, quiescence, timing, frame, module, data-source, and rejection diagnostics. Venues and instruments appear when the run touches or creates them; an empty replay can complete with empty venue diagnostics.

## Final Positions

Read final strategy-local positions from `FinalSnapshot`:

```csharp
var run = result.Runs.Single();

foreach (var position in run.FinalSnapshot.GetPositions())
{
    Console.WriteLine($"{position.AssetId}: {position.Quantity}");
}
```

Filled orders update the final snapshot. If there are no fills for a strategy, its final position collection can be empty.

## Fills and Round Trips

For execution analysis, start from `OrderFilled` events:

```csharp
var fills = result.ExecutionEvents
    .OfType<OrderFilled>()
    .OrderBy(static fill => fill.Time)
    .ToArray();
```

Round-trip analytics are built from fills, not from market-data trades. `RoundTripBuilder.FromFills(...)` performs FIFO matching and produces completed trades with instrument, side, quantity, entry and exit prices and times, commission, gross and net PnL, return percent, holding period, and notional.

```csharp
var roundTrips = RoundTripBuilder.FromFills(fills).ToArray();
```

The per-run `TearSheet` is built from the strategy fills for that run. Use it for summary metrics, then drill into `ExecutionEvents` when you need to explain a specific order lifecycle or fill price.

## Parameter Grids

When a strategy supports generated parameter sets, `WithGrid(...)` registers one variant per parameter set. The parameters are preserved on each `StrategyRunResult`:

```csharp
foreach (var run in result.Runs)
{
    Console.WriteLine($"{run.VariantIndex}: {run.Parameters}");
}
```

Use `result.ToParameterGrid()` when you need to reconstruct grid-shaped output from the completed runs.

Cookbook examples:

- `cookbook/GettingStarted/05-parameter-grid.cs`
- `cookbook/GettingStarted/08-export-results.cs`
- `cookbook/Simulation/queue-accurate-backtest.cs`
