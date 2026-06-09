# Memory And Unsafe

Use this page when a maintainer needs to explain a hot-path allocation failure, an unsafe namespace diagnostic, or a generated-registration diagnostic.

## Hot-Path Allocation Rule

In debug builds, guarded market, execution, and lifecycle dispatch paths measure managed allocations after a warmup call. If a strategy allocates during the checked hot path, dispatch throws `HotPathAllocationException`.

This is a debug guard. Public guidance should still discourage hot-path allocations, but it should not promise the same exception in release builds.

## Unsafe Boundary

`UnsafeAccessAnalyzer` reports `RHD001` when safe `Rhodium.*` assemblies outside `Rhodium.Unsafe`, `Rhodium.Tensor`, and `Rhodium.Kernel` reference `Rhodium.Unsafe.*` types. Unsafe storage and memory tracking are implementation infrastructure, not general extension points.

`RhodiumRuntime.Dispose()` disposes world state, market-state tensors, and market tensors. Memory-leak coverage uses `GlobalMemoryTracker` to prove tracked runtime allocations are released.

## Generated Registration

`GeneratedRegistrationAnalyzer` reports `RHD019` for manual calls to generated registration helpers such as `__GeneratedRegisterIndicator` and `__GeneratedRegisterPortfolioField`. Users should declare generated fields/indicators and let the generator register them.

## Evidence

- Source: `Rhodium.Platform/Strategy.Core.cs`, `Rhodium.Platform/StrategyExecutionInvariantException.cs`, `Rhodium.Analyzers/UnsafeAccessAnalyzer.cs`, `Rhodium.Analyzers/GeneratedRegistrationAnalyzer.cs`, `Rhodium.Kernel/RhodiumRuntime.cs`.
- Tests: `Rhodium.Platform.Tests/StrategyTests.cs`, `Rhodium.Platform.Tests/EngineLoopsTests.cs`, `Rhodium.Generators.Tests/StrategyGeneratorTests.cs`, `Rhodium.Kernel.Tests/MemoryLeakTests.cs`.

## Do Not Depend On

- Do not describe debug allocation checks as release-mode enforcement.
- Do not expose `Rhodium.Unsafe` types as stable user-facing APIs.
- Do not recommend calling generated registration helpers by hand.
