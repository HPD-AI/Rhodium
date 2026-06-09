# Diagnostics

This page covers runtime diagnostics, simulation result surfaces, modules, frames, and common runtime errors. Generator/analyzer diagnostic codes are listed in [generator diagnostics](generator-diagnostics.md).

## Runtime Errors

Common user-visible exceptions and invariant failures:

- `HotPathAllocationException`: debug guard reports managed allocations after warm-up in guarded market, execution, lifecycle, or timer paths. The current message can say `OnTick()` even when another guarded path triggered it.
- `UniverseTopologyChangedException`: market universe version changed after strategy initialization; reinitialize the strategy.
- `TensorAccessException`: debug validation for out-of-bounds asset id or unregistered read-only market field.
- `InvalidOperationException`: common causes include adding instruments outside `OnInitialize`, manual generated registration outside initialization, reading/writing generated properties from the strategy instance, missing parameter grid values, incompatible `[Param]` values, extending exact-row grids with Cartesian axes, or duplicate parameter axes.
- `ArgumentOutOfRangeException`: common causes include invalid parameter variant index, recurring schedule interval not positive, or invalid group child index.

## Simulation Result

`SimulationResult` exposes:

- `Runs`: per-strategy/variant `StrategyRunResult` with `StrategyId`, `VariantIndex`, `Parameters`, `TearSheet`, and `FinalSnapshot`.
- `Batch`: batch tear sheet.
- `OrderIntents`: strategy-side intents.
- `ExecutionEvents`: exchange-side outcomes.
- `AccountStatements`: venue account snapshots.
- `SimulatorEvents`: non-execution simulator events. Raw replay market events are projected, not retained as a market-data log in tested paths.
- `Diagnostics`: defaults to `SimulationDiagnostics.Empty`.
- Helpers: `Analyze()`, `TopBySharpe`, `TopByTotalReturn`, `ToParameterGrid`.

## Vector Scan Analysis

`VectorScanAnalyzer` provides:

- `TopBySharpe`
- `TopByTotalReturn`
- `ToHeatmap(metric, xParameter, yParameter)`
- `Filter(minSharpe, maxDrawdown, minWinRate, minTrades)`
- `ToCsv`
- `ExportToCsv`
- `ExportToParquet`

CSV and Parquet output include `strategy_id`, `variant_index`, `total_return`, `sharpe`, `max_drawdown`, `win_rate`, and `total_trades`. CSV appends sorted parameter names without a prefix; Parquet appends string parameter fields as `param_<sanitized name>`.

## Simulation Diagnostics

`SimulationDiagnostics` groups final simulator state and counters:

- `Venues`: final market/account state, cash/available/reserved, pending settlement/delivery, counters, policies.
- `Instruments`: status, fidelity, policies, mark/close mark, order counters.
- `Quiescence`: max and total same-timestamp iterations.
- `Latency`: sampled command count and min/max/average entry latency.
- `Timing`: replay start/end, final clock, event count.
- `Modules`: per-module pre/process calls, emitted events/commands/frames, counters, metrics, messages.
- `FrameStats`: struct event hub stats.
- `DataSources`: source id, priority, ordinal, kind, effective read options.
- `Rejections`: venue, instrument, order id, reason.

## Modules

`ISimulationModule` lifecycle:

- `Reset`
- `PreProcess`
- `Process`
- `AppendDiagnostics`

Scopes:

- `ISessionSimulationModule`
- `IVenueSimulationModule`
- `IInstrumentSimulationModule`

Module context exposes clock, market snapshot, and read-only venue/instrument views. Module sinks can emit semantic events, exchange commands, or struct frames according to `SimulationFrameMode`.

## Frames

`SimulationFrameMode` values:

- `Disabled`
- `MarketData`
- `Execution`
- `Diagnostics`
- `All`

Frame families:

- Market frames: `QuoteFrame`, `TradeFrame`, `BookLevelDeltaFrame`, `BookDepthLevelFrame`.
- Book-order frames: add, modify, delete, executed.
- Execution frames: `ExecutionFillFrame`.
- Diagnostic frames: `RiskMetricFrame`, `TensorProjectionFrame`.

`SimulationFrameBus` owns routes and sequenced emitters. Normal users should start with object events and `SimulationDiagnostics`, then enable frames when they need lower-level stream output.

See [result analysis](../simulation/result-analysis.md) and [diagnostics and frames](../simulation/diagnostics-and-frames.md).
