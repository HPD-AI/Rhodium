# Engine Internals

Use these pages when a public simulation or strategy rule needs an internal explanation: why a hook ran in that order, why a field is only valid inside a generated context, why topology changes fail after setup, or why a replay turn keeps draining at one timestamp.

These are maintainer notes, not a strategy-authoring API. Strategies should depend on setup APIs, generated hook contexts, `MarketKernel`, and `PortfolioContext`; they should not reach for `RhodiumRuntime`, `WorldState`, L3 tensor layout, or unsafe storage as stable contracts.

## Pages

- [Architecture](architecture.md): session/runtime ownership, batch topology, and the boundary between exchange truth and strategy-facing state.
- [Event Ordering](event-ordering.md): replay timestamp turns, schedules, drains, modules, and quiescence limits.
- [Strategy Dispatch](strategy-dispatch.md): initialization, hierarchical market/lifecycle dispatch, generated hooks, and topology guards.
- [Runtime And World State](runtime-and-world-state.md): per-strategy positions, order intents, allocation phases, snapshots, and disposal.
- [Market Kernel](market-kernel.md): projected market tensors, contract metadata, depth reads, and adjustment kernels.
- [Tensor Store](tensor-store.md): generated fields, writable portfolio tensors, hook-only property access, and windows.
- [HFT Depth](hft-depth.md): L2 depth implementations, L3 event routing, and optional struct frames.
- [Memory And Unsafe](memory-and-unsafe.md): debug allocation guards, unsafe assembly boundaries, and generated registration analyzer rules.

## Evidence

Primary source lives under `/Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/src`; tests live under `/Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/test`. Each page names the source and test files that back its claims.
