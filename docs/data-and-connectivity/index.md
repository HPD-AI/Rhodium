# Data And Connectivity

Use this section when a Rhodium run needs replay data, source provenance, bar aggregation, connector interfaces, venue routing, or financing events.

For simulation and backtesting, start with data plans:

```csharp
var plan = SimulationDataPlan
    .Create(ReplayReadOptions.All with { Limit = 3 })
    .AddSource("quotes-fixture", quotes, priority: 0)
    .AddSource("bars-fixture", bars, priority: 10);

var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithData(plan)
    .Run();
```

`SimulationDataPlan` and `SimulationDataIterator` are the current replay-data path. They merge named sources, apply read options, preserve provenance, and feed `SimulationSession`.

## Pages

- [Simulation data plans](simulation-data-plans.md): named replay sources, read options, provenance, catalogs, and cookbook output.
- [Replay connectors](replay-connectors.md): the obsolete `ReplayConnector` surface, documented as a legacy parity oracle rather than the recommended simulation path.
- [Aggregators](aggregators.md): public `Rhodium.Data.Aggregators` APIs for manual trade-to-bar aggregation.
- [Routing policies](routing-policies.md): `TradingHost` cross-venue routing knobs, policy catalogs, and fee-aware market routing.
- [Financing feeds](financing-feeds.md): financing charge command feeds, rate-curve feeds, explicit `FinancingChargeApplied` replay events, and account effects.
- [Data providers](data-providers.md): connector, subscription, normalizer, metadata-normalizer, and rate-limiter interfaces.

Related simulation pages:

- [Streaming data](../simulation/streaming-data.md)
- [Shared history](../simulation/shared-history.md)
- [Diagnostics and frames](../simulation/diagnostics-and-frames.md)

## Boundaries

The inspected source backs in-memory and replay-source data plans, catalog adapters, public aggregators, routing policy catalogs, financing feed parsers, and connector interfaces.

The inspected source does not show built-in live broker/provider connectors, a filesystem or vendor catalog implementation, or an automatic data-plan stage that aggregates trades into bars during replay. Those areas are documented at the interface level or marked as manual composition.
