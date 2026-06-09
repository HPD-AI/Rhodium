# HFT Depth

Use this page when a visible depth value, book-level frame, or L3 update needs an internal explanation.

## L2 Depth Rule

Depth implementations share `IHftDepth`: tick size, lot size, best bid/ask ticks, quantity at tick, level updates, clearing, and ordered level copying. `MarketKernel` reads depth through runtime state by virtual index, returning null/zero/empty reads when no depth exists for an asset.

Depth snapshots and deltas are projections. They update the runtime depth object for the instrument/variant; strategy hooks and fill/slippage code then read the projected state.

## L3 Routing

L3 book-order events route through `RhodiumRuntime.AddBookOrder`, `ModifyBookOrder`, `DeleteBookOrder`, and `ExecuteBookOrder` into `L3EventHandler` and `MarketState` tensors. Public docs should describe the resulting book/depth/frame behavior, not the internal tensor slot layout, unless that layout is explicitly promoted to a public contract.

## Struct Frames

Struct frame projection is optional and disabled by default. `SimulationFrameMode.MarketData` emits quote, trade, depth, and book-order frames. `SimulationFrameMode.Execution` emits fill frames. `All` enables both of those paths and allows module frame sinks; diagnostics/module details should be documented from the frame/module APIs, not from L3 internals.

## Evidence

- Source: `Rhodium.HFT/IHftDepth.cs`, `HashMapDepth.cs`, `BTreeDepth.cs`, `FusedDepth.cs`, `RoiVectorDepth.cs`, `Rhodium.Kernel/MarketKernel.cs`, `Rhodium.Kernel/RhodiumRuntime.cs`, `Rhodium.Simulation/Projection/SimulationMarketProjector.cs`, `SimulationStructFrameProjector.cs`, `SimulationFrameBus.cs`.
- Tests: `Rhodium.HFT.Tests/HftDepthTests.cs`, `Rhodium.HFT.Tests/L3EventHandlerTests.cs`, `Rhodium.Simulation.Tests/SimulationSessionArchitectureTests.cs`.

## Do Not Depend On

- Do not rely on a particular `IHftDepth` implementation unless the setup explicitly chooses it.
- Do not document L3 tensor internals as stable user API.
- Do not assume struct frames are emitted unless `FrameMode` enables them.
