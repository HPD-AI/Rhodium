# Market Kernel

Use this page when a strategy-visible market read needs an internal explanation: why a bar field is the latest projection, why best bid/ask comes from depth, or why contract/grid metadata is available in hooks.

## Projection Rule

Replay market events are projected into runtime state before strategies read them. Bars write OHLCV raw fields across the instrument range. Quotes write bid/ask/size fields and update HFT depth. Trades update close/volume raw fields. Book snapshots, depth snapshots, and deltas clear/rebuild or update depth by variant.

When a projection returns `RequiresAdjustment`, `StrategyEventProcessor` runs `MarketKernel.RunAdjustmentKernel` before strategy dispatch. Generated adjusted fields should therefore be documented as projected tensor values, not as retained replay objects.

## Kernel Surface

`MarketKernel` is a read-only `ref struct` created from runtime state for dispatch. It exposes:

- scalar tensor reads and field existence checks;
- universe size, version, and tensor basis;
- best bid/ask ticks, quantity at tick, and copied depth levels;
- contract projection, trading grid, tradability, allowed order types/time-in-force, option/package flags, and exposure metadata.

## Evidence

- Source: `Rhodium.Simulation/Projection/SimulationMarketProjector.cs`, `Rhodium.Platform/Patterns/StrategyEventProcessor.cs`, `Rhodium.Kernel/MarketKernel.cs`, `Rhodium.Kernel/RhodiumRuntime.cs`.
- Tests: `Rhodium.Kernel.Tests/RhodiumRuntimeTests.cs` proves contract projection and depth reads come from runtime state.

## Do Not Depend On

- Do not treat `MarketKernel` as mutable storage.
- Do not promise raw market event retention in results.
- Do not read fields that were never registered/projected for the current run.
