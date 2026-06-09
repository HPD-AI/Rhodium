# Simulation Data Plans

Use `SimulationDataPlan` when replay input comes from named sources and you want Rhodium to merge, filter, and report provenance for those sources.

```csharp
var plan = SimulationDataPlan
    .Create(ReplayReadOptions.All with { Limit = 3 })
    .AddSource("quotes-fixture", quotes, priority: 0)
    .AddSource("bars-fixture", bars, priority: 10);

var iterator = new SimulationDataIterator(plan);

var result = Rhodium.Simulation.Rhodium
    .Simulate<DataPlanStrategy>()
    .WithData(iterator)
    .Run();

Console.WriteLine(plan.SourceCount);
Console.WriteLine(string.Join(", ", iterator.Provenance.Select(p => p.SourceId)));
Console.WriteLine(result.Diagnostics.Timing.ReplayEventCount);
```

Cookbook example: `cookbook/Data/simulation-data-plan.cs` in the [Data cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/Data)

The cookbook was run with filtered output:

```text
Sources: 2
Provenance: quotes-fixture, bars-fixture
Order intents: 1
Filled: 1
Final position: 1
```

## Add Sources

Create an empty plan with optional base read options:

```csharp
var plan = SimulationDataPlan.Create(readOptions);
```

Source-backed inputs are:

- `SimulationDataSource`
- `IReplaySource<FinanceEvent>`
- `IEnumerable<FinanceEvent>`
- `IAsyncEnumerable<FinanceEvent>`
- `ISimulationCatalog` plus `SimulationDataQuery`

```csharp
var plan = SimulationDataPlan
    .Create()
    .AddSource("events", events, sourceKind: "enumerable")
    .AddSource("async-events", asyncEvents, sourceKind: "async-enumerable")
    .AddSource("replay", replaySource, sourceKind: "replay-source");
```

At builder level, raw async streams are not accepted directly. Wrap them in a plan with `AddSource(...)`, or use `SimulationSession.RunAsync(...)` directly at the session layer.

## Read Options

Plan read options are intersected with run read options by `SimulationDataIterator`:

- `From` uses the later start.
- `To` uses the earlier end.
- incompatible `EventFlowId` values produce an empty stream.
- `Limit` uses the smaller non-null limit.

The iterator filters by finance event time. Quotes use quote exchange time, trades use trade exchange time, bars use bar time, depth/book events use their event-specific time where available, and other events fall back to `FinanceEvent.Time`.

## Ordering

`SimulationDataIterator` merges sources through the finance replay ordering policy. Same-time ordering is finance-aware: lifecycle/status events come before books/depth, then quotes, trades, bars, execution, control, diagnostics, and unknown events.

Use `SharedHistory` when you already have one materialized ordered sequence. Use a data plan when multiple sources need deterministic merging.

## Provenance

Inspect provenance before or after a run:

```csharp
var iterator = new SimulationDataIterator(plan);
var provenance = iterator.GetProvenance();
```

`SimulationDataProvenance` records:

- source id
- priority
- source ordinal
- source kind
- effective `From`, `To`, `EventFlowId`, and `Limit`

`result.Diagnostics.DataSources` reports the same source provenance for data-plan runs. It does not report per-source emitted event counts; use `result.Diagnostics.Timing.ReplayEventCount` for the run-level count of processed replay events.

## Catalog Sources

`ISimulationCatalog` creates replay sources from queries and exposes metadata:

```csharp
var plan = SimulationDataPlan
    .Create()
    .AddCatalogSource(
        "catalog-bars",
        catalog,
        SimulationDataQuery.ForInstrument(spy, range, SimulationDataKind.Bars));
```

The source-backed catalog implementations are:

- `InMemorySimulationCatalog`
- `ReplaySourceSimulationCatalogAdapter`

`SimulationDataQuery` can target instruments, date ranges, and `SimulationDataKind` flags such as bars, trades, quotes, books, execution, lifecycle, diagnostics, and control events.

Cookbook example: `cookbook/Data/replay-source.cs` in the [Data cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/Data)

The cookbook was run with filtered output:

```text
Replayed events: 2
Plan sources: 1
Order intents: 1
Filled: 1
```

## Open Edges

No inspected source shows a filesystem-backed, vendor-backed, or database-backed simulation catalog. Keep catalog assumptions to the implementation you provide or to the in-memory/replay-source adapter surfaces documented here.
