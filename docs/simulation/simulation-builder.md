# Simulation Builder

`Rhodium.Simulate<TStrategy>()` creates the fluent builder for a simulation run. A strategy type must derive from `Strategy` and have a public parameterless constructor.

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithHistory(SharedHistory.Load(events))
    .Run();
```

A builder run needs replay input. Use `WithHistory(SharedHistory)` for materialized in-memory events, or `WithData(SimulationDataIterator)` / `WithData(SimulationDataPlan)` for composed replay sources. If neither input is set, the run throws because there is no simulation history or data plan.

`Run()` is the synchronous convenience path. Use `RunAsync(...)` when the caller is already asynchronous.

## Common Setup

Start with the smallest run:

```csharp
var history = SharedHistory.Load([
    CreateBarClosed(spy, 123m)
]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithHistory(history)
    .Run();
```

Then add configuration only where the scenario needs it:

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithHistory(history)
    .WithInitialCash(Money.USD(250_000m))
    .WithMatchingFidelity(MatchingFidelity.QueueAccurate)
    .WithConfig(SimulationConfig.USEquities())
    .Run();
```

Run-level defaults are conservative:

- `SimulationConfig.Instant()`
- `MatchingFidelity.QueueAccurate`
- `Money.USD(100_000m)` initial cash
- `MaxDegreeOfParallelism = 1`
- `SimulationFrameMode.Disabled`

## Builder Surface

Use replay input methods to choose where events come from:

- `WithHistory(SharedHistory)`
- `WithData(SimulationDataIterator)`
- `WithData(SimulationDataPlan)`

Use run defaults for behavior shared by all simulated venues and instruments:

- `WithConfig(SimulationConfig)`
- `WithMatchingFidelity(MatchingFidelity)`
- `WithInitialCash(Money)`
- `WithMaxDegreeOfParallelism(int)`
- `WithFrameMode(SimulationFrameMode)`

Use account and market setup methods when the default lazy setup is not enough:

- `WithVenue(SimulationVenueConfig)` or the venue overloads
- `WithInstrument(SimulationInstrumentConfig)`
- `WithAccountSeed(AccountSeed)`
- `WithAccountSeeds(...)`

Use extension points only for advanced deterministic simulation plumbing:

- `WithSessionModule(...)`
- `WithVenueModule(...)`
- `WithInstrumentModule(...)`

Generated parameter grids are registered with `WithGrid(...)` when a strategy exposes generated parameter factories. Each variant appears as a separate `StrategyRunResult` with its `Parameters` preserved.

## What To Read After

`SimulationResult` contains the run output:

```csharp
var fills = result.ExecutionEvents.OfType<OrderFilled>().ToArray();
var statements = result.AccountStatements;
var diagnostics = result.Diagnostics;
var finalPositions = result.Runs.Single().FinalSnapshot.GetPositions();
```

Read `OrderIntents` for what the strategy requested. Read `ExecutionEvents` for what the simulated venue accepted, rejected, filled, cancelled, modified, or expired. Read `SimulatorEvents` for account, settlement, lifecycle, margin, custody, transfer, and diagnostic events that are not exchange executions.

## Cookbook Examples

- `cookbook/GettingStarted/01-first-backtest.cs`
- `cookbook/GettingStarted/05-parameter-grid.cs`
- `cookbook/GettingStarted/06-venue-config.cs`
- `cookbook/GettingStarted/07-account-seed.cs`
- `cookbook/GettingStarted/08-export-results.cs`
