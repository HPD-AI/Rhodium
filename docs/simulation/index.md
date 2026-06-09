# Simulation

Rhodium simulations run strategies against replayed finance events and simulated execution venues.

The high-level entry point is:

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithHistory(history)
    .Run();
```

A run needs either:

- `WithHistory(SharedHistory)` for materialized in-memory replay, or
- `WithData(SimulationDataIterator)` / `WithData(SimulationDataPlan)` for streaming simulation data.

Important configuration surfaces:

- `WithMatchingFidelity(...)`: selects fast vector, queue-accurate, or market-by-order behavior.
- `WithConfig(...)`: applies `SimulationConfig` defaults such as latency, fees, slippage, queue model, settlement, and account model.
- `WithInitialCash(...)`: sets default starting cash.
- `WithAccountSeed(...)`: seeds opening cash and positions.
- `WithVenue(...)`: overrides venue-specific cash, base currency, account type, matching fidelity, config, and policies.
- `WithInstrument(...)`: overrides instrument-specific simulation behavior under a venue.

`SimulationResult` returns order intents, execution events, account statements, simulator events, diagnostics, per-run summaries, and batch metrics.

## Pages

- [Simulation builder](simulation-builder.md)
- [Shared history](shared-history.md)
- [Streaming data](streaming-data.md)
- [Matching fidelity](matching-fidelity.md)
- [Fills, fees, slippage, and latency](fills-fees-slippage-latency.md)
- [Result analysis](result-analysis.md)
- [Venues and instruments](venues-and-instruments.md)
- [Accounts, margin, and settlement](accounts-margin-settlement.md)
- [Options and lifecycle](options-lifecycle.md)
- [Diagnostics and frames](diagnostics-and-frames.md)
- [Modules](modules.md)

Start with [First backtest](../start/first-backtest.md), then use the specific simulation pages in this section for execution realism and configuration.
