# Streaming Data

Use `SharedHistory` when replay events are already materialized in memory. Use `SimulationDataPlan` or `SimulationDataIterator` when a run should merge, filter, or stream replay sources.

```csharp
var plan = SimulationDataPlan
    .Create()
    .AddSource("bars", barEvents, priority: 0, sourceKind: "memory")
    .AddSource("trades", tradeEvents, priority: 1, sourceKind: "memory");

var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithData(plan)
    .Run();
```

The builder accepts `SimulationDataPlan` and `SimulationDataIterator`. It does not accept a raw `IAsyncEnumerable<FinanceEvent>` directly; wrap async streams with `SimulationDataPlan.AddSource(...)`, or use `SimulationSession.RunAsync(...)` directly when you are working at the session layer.

## Data Plans

`SimulationDataPlan` is the user-facing composition object. It can be created with optional `ReplayReadOptions` and can add:

- a `SimulationDataSource`
- a named `IReplaySource<FinanceEvent>`
- a named `IEnumerable<FinanceEvent>`
- a named `IAsyncEnumerable<FinanceEvent>`
- a catalog source from `ISimulationCatalog`

Read options can be set on the plan and on the run. The iterator intersects those options, then applies filters such as `From`, `To`, `EventFlowId`, and `Limit`.

## Ordering And Provenance

`SimulationDataIterator` merges plan sources with Rhodium replay ordering. Use it when you need to inspect the effective sources before or after a run:

```csharp
var iterator = new SimulationDataIterator(plan);
var provenance = iterator.GetProvenance();
```

`result.Diagnostics.DataSources` also reports source provenance: source id, priority, ordinal, source kind, and effective read options. That is the first place to look when a composed run did not read the events you expected.

## Catalog Sources

`ISimulationCatalog` exposes replay sources plus metadata such as instruments and available ranges. `SimulationDataQuery` can target instruments, date ranges, and data kinds.

```csharp
var plan = SimulationDataPlan
    .Create()
    .AddCatalogSource(
        "research-catalog",
        catalog,
        SimulationDataQuery.ForInstrument(spy));
```

The tested catalog implementations are in-memory or replay-source adapters. Do not assume filesystem or vendor-provider behavior unless the catalog implementation you are using documents it.

## Generated Data

For synthetic or generated scenarios, create `FinanceEvent` objects such as `BarClosed`, `QuoteReceived`, or `TradeOccurred`, then feed them through `SharedHistory.Load(...)` or `SimulationDataPlan.AddSource(...)`. There is no separate public synthetic-market generator API documented for the inspected simulation surface.

Related pages:

- [Shared history](shared-history.md)
- [Diagnostics and frames](diagnostics-and-frames.md)
