# Architecture

Use this page when a user-visible rule depends on who owns state: why strategies do not receive a runtime object, why exchange fills are authoritative, or why adding assets after setup is rejected.

## Operational Rule

`SimulationSession` orchestrates a run. Exchanges own execution truth; `RhodiumRuntime` owns strategy-facing market tensors, market-state tensors, batch maps, contract projections, HFT depth, L3 state, and `WorldState`. Strategy code sees a `MarketKernel` plus a by-ref `PortfolioContext` or generated context, not the runtime itself.

That boundary is why user APIs should describe context reads and order intents rather than runtime mutation. The runtime is a disposable owner/backing store; the exchange and session decide when projected events, fills, schedules, and lifecycle events are processed.

## Batch Topology

`BatchMap` maps instruments to contiguous virtual-index ranges and supports variants. Adding an instrument bumps the batch-map version. Generated strategy code sees this through `MarketKernel.UniverseSize`, `UniverseVersion`, and `Basis`.

Strategies may register assets during initialization/setup. Once initialized, hot-path dispatch compares the captured version with the current market version and throws `UniverseTopologyChangedException` if the topology changed.

## Evidence

- Source: `Rhodium.Simulation/SimulationSession.cs`, `Rhodium.Kernel/RhodiumRuntime.cs`, `Rhodium.Kernel/MarketKernel.cs`, `Rhodium.Kernel/BatchMap.cs`, `Rhodium.Platform/Strategy.Core.cs`.
- Tests: `Rhodium.Kernel.Tests/RhodiumRuntimeTests.cs` verifies runtime owners and kernel reads for contracts/depth.

## Do Not Depend On

- Do not treat `RhodiumRuntime` as a strategy-authoring API.
- Do not assume virtual indexes are stable across different setup topologies.
- Do not bypass exchange/session processing by mutating runtime state from strategy code.
