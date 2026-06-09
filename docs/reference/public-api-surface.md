# Public API Surface

This page groups public Rhodium APIs by role. It is not an exhaustive symbol index; it calls out the surfaces users normally author against and the public transition-era surfaces tests explicitly guard.

## `Rhodium.Platform`

Strategy authoring starts with `Strategy`.

- `Strategy`: base type for authored strategies. User hooks are overrides or generator-recognized `partial void` methods.
- `SetupContext`: cold-path initialization context for adding instruments and schedules.
- Generated market context marker names with public marker files: `BarContext`, `TickContext`, `QuoteContext`, `TradeContext`, `BookSnapshotContext`. The public files are markers; concrete strategies receive generated nested ref structs with usable properties.
- Generated-only hook context names: `BookLevelDeltaContext`, `BookLevelDeltasContext`. These are valid generated hook signatures, but they are not standalone public marker files in `Rhodium.Platform`.
- Virtual contexts: `LifecycleContext`, `TimerContext`, `OrderContext`, `FillContext`, `PositionContext`, `GroupContext`, `ChildContext`.
- `StrategySchedule`: schedule description used by setup/timer flows.

Common extension groups:

- `Fields.RSI_14`: convenience generated field constant.
- `DataExtensions`: market-data getters.
- `MarketExtensions`: book and top-of-book helpers.
- `TradeExtensions`: position, flatten, and cancel helpers.

## `Rhodium.Platform.Attributes`

Generation attributes are property attributes:

- Field attributes: `BarFieldAttribute`, `TickFieldAttribute`, `QuoteFieldAttribute`, `TradeFieldAttribute`, `BookFieldAttribute`.
- Indicator attributes: `BarIndicatorAttribute`, `TickIndicatorAttribute`, `BarIndicatorGroupAttribute`.
- Rolling windows: `WindowAttribute`.
- Parameters: `ParamAttribute`.
- Source enums: `BarSource`, `TickSource`.

See [attributes](attributes.md) and [generated fields](../strategy-authoring/generated-fields.md).

## Parameterization

Use these when a strategy has `[Param]` properties or is run across variants.

- `ParameterSet`: immutable parameter row with typed getters and enumeration.
- `ParameterGrid`: Cartesian grids and exact-row grids.
- `IStrategyParameterFactory<TStrategy>`: generated interface implementation for parameterized strategies.
- `StrategyGrid<TStrategy>`: builds strategy variants from a `ParameterGrid`.
- `VariantDescriptor`: records strategy id, variant index, and parameter set.

See [parameters and grids](../strategy-authoring/parameters-and-grids.md).

## Strategy Orchestration

These are public orchestration/runtime types rather than normal hook authoring surfaces:

- `StrategyTree`
- `StrategyEventProcessor`
- `ParallelDispatchState`
- `StrategyContext`

Public API tests reject exposing older transition names such as `StrategyBase`, `ITickVisitor`, and `EngineLoops` as public API.

## `Rhodium.Primitives`

Core market and command primitives include:

- Market values and ids: prices, quantities, money/currency, instruments, contracts, venues, sides, asset/order/strategy ids.
- Orders and intents: `ExecutionSpec`, `Execution`, `OrderIntent`, command records, cancel/modify/set-position/allocation/liquidation commands.
- Execution enums: `OrderType`, `TimeInForce`, `ExecutionLimitPriceMode`, `ExecutionAlgorithm`, `TrailingOffsetType`.

See [execution spec](execution-spec.md) and [orders and positions](../market-model/orders-and-positions.md).

## `Rhodium.Indicators`

Indicator authoring and direct use:

- `IIndicator<T>` with `Value`, `IsReady`, `Count`, and `Reset()`.
- Update-specific interfaces: `IPriceIndicator`, `IBarIndicator`, `ITickIndicator`.
- Base classes: `PriceIndicatorBase`, `BarIndicatorBase`, `TickIndicatorBase`.
- `Indicators` factory and `Rhodium.Indicators.Streaming` implementations.

See [indicators](indicators.md).

## `Rhodium.Simulation`

Simulation-facing surfaces include:

- Builder: `Rhodium.Simulate<TStrategy>()`, `SimulationBuilder<TStrategy>`, grid extensions.
- Run shape: `SimulationRunOptions`, `SimulationVenueConfig`, `SimulationInstrumentConfig`, `MatchingFidelity`.
- Model config: `SimulationConfig`, latency, queue, fee, slippage, price improvement, fill behavior, account/margin/settlement/lifecycle config.
- Data: `ISimulationCatalog`, `InMemorySimulationCatalog`, `SimulationDataPlan`, `SimulationDataSource`, `SimulationDataQuery`, `SimulationDataKind`.
- Results: `SimulationResult`, `StrategyRunResult`, tear sheets, `VectorScanAnalyzer`, `SimulationDiagnostics`.
- Modules/frames: `ISimulationModule`, session/venue/instrument module scopes, `SimulationFrameMode`, frame bus and frame structs.

See [simulation config](simulation-config.md), [result analysis](../simulation/result-analysis.md), and [diagnostics and frames](../simulation/diagnostics-and-frames.md).

## Exceptions

Common public exceptions:

- `TensorAccessException`
- `StrategyExecutionInvariantException`
- `HotPathAllocationException`
- `UniverseTopologyChangedException`
