# Tensor Store

Use this page when generated strategy fields behave differently inside and outside hooks, when a window diagnostic appears, or when a field is registered into market versus portfolio storage.

## Storage Rule

Generated read-only market fields are registered into runtime market tensors when needed. Generated indicators and writable fields are registered as strategy-private portfolio tensor fields. Runtime tests prove generated indicators write to the strategy-private tensor store, and writable bar fields persist in portfolio tensor storage.

`RhodiumRuntime` has two tensor stores: `Tensors` for strategy-facing market columns and `MarketState` for L3/order-book tensor space. Strategy-private generated state lives through `WorldState`/portfolio tensor storage rather than the shared market tensor columns.

## Context-Only Properties

Generated strategy properties are valid through hook contexts such as `tick.Property` or `bar.Property`. Generated code throws if users read or write those properties on the strategy instance outside a hook context.

The generated context path uses `ref PortfolioContext` and `PortfolioContextFrame`; do not document generated contexts as storing a market kernel or by-value portfolio.

## Windows

Windows are generated only for read-only `double` bar fields. They are backed by per-asset `RollingTensorHistory` arrays sized to the maximum requested window length. Invalid window usage reports generator diagnostics `RHD015` or `RHD016`.

## Evidence

- Source: `Rhodium.Generators/StrategyGenerator.cs`, `Rhodium.Kernel/RhodiumRuntime.cs`, `Rhodium.Kernel/WorldState.cs`, `Rhodium.Kernel/PortfolioContextFrame.cs`.
- Tests: `Rhodium.Platform.Tests/StrategyTests.cs`, `Rhodium.Generators.Tests/StrategyGeneratorTests.cs`.

## Do Not Depend On

- Do not read generated properties on the strategy instance outside hook contexts.
- Do not assume writable/generated indicator fields are shared market columns.
- Do not request windows for unsupported field types or non-bar frequencies.
