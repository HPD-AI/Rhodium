# Shared History

`SharedHistory` is the materialized replay input for deterministic in-memory simulation. It stores `FinanceEvent` objects in order and can be reused across runs.

```csharp
var history = SharedHistory.Load([
    new BarClosed(spy, bar) { Time = bar.EndTime }
]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithHistory(history)
    .Run();
```

Use it for tests, cookbook examples, and small research scenarios where you already have the replay events in memory.

## What It Contains

`SharedHistory` contains `FinanceEvent` objects. It does not accept raw OHLC values directly; create replay events such as:

- `BarClosed`
- `QuoteReceived`
- `TradeOccurred`
- account or lifecycle events when the scenario needs them

The container exposes count and indexed/span-style access for inspection before a run.

```csharp
Console.WriteLine(history.Count);
Console.WriteLine(history[0].Time);
```

`SharedHistory.Load(...)` materializes an `IEnumerable<FinanceEvent>`. `SharedHistory.LoadAsync(...)` materializes an `IAsyncEnumerable<FinanceEvent>`.

## Event Order

Rhodium preserves the order of events loaded into `SharedHistory`. Use explicit `Time` values when same-timestamp behavior matters, especially for order admission, settlement release, or option expiry examples.

```csharp
var history = SharedHistory.Load([
    CreateBarClosed(spy, 100m, Instant.FromUnixSeconds(1)),
    CreateBarClosed(spy, 101m, Instant.FromUnixSeconds(2))
]);
```

For multi-source runs, prefer `SimulationDataPlan` so the iterator can merge sources with the replay ordering policy and preserve source provenance in diagnostics.

## Materializing A Data Plan

When you want to debug a streaming plan as a fixed replay, read the iterator and load it:

```csharp
var iterator = new SimulationDataIterator(plan);
var history = await SharedHistory.LoadAsync(iterator.ReadAsync());
```

This is useful for shrinking a composed data issue into a deterministic reproduction.

Cookbook examples:

- `cookbook/GettingStarted/01-first-backtest.cs`
- `cookbook/Simulation/queue-accurate-backtest.cs`
- `cookbook/Simulation/options-lifecycle.cs`
