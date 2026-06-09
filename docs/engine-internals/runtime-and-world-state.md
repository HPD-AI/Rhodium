# Runtime And World State

Use this page when a strategy-visible portfolio behavior needs an internal explanation: isolated positions, drained order intents, parent/child allocation timing, snapshots, or memory disposal.

## Per-Strategy State Rule

`WorldState` stores hot-path positions, working orders, cash/allocation flags, strategy-private tensors, pending order intents, and reusable snapshot buffers per strategy id. Position/order pages allocate on demand in 1024-entry pages, so sparse high virtual indexes can work without preallocating the full universe for every strategy.

Tests prove positions are isolated by strategy id, high virtual indexes allocate pages on demand, and parallel contexts do not bleed state or order intents across strategies.

## Context Commit Rule

Dispatch builds a span-backed `PortfolioContext` with counters, allocation command scratch, child snapshots, and order-intent scratch. `CommitContext` writes cash/allocation/pause state back to the strategy slice and drains emitted order intents.

Allocation commands are phase-delimited. Engine loops collect commands for one hierarchy depth, finish that phase, then apply the commands. Parent/child controls should be documented as taking effect at phase boundaries, not as immediate mutation of another currently running context.

## Snapshots

Snapshots include non-flat positions and can use contract/mark inputs for exposure. `EnsureSnapshotCapacity` warms reusable buffers; warmed snapshot building is covered by zero-managed-allocation tests. Child snapshots are built before group/lifecycle dispatch and passed into parent contexts.

## Evidence

- Source: `Rhodium.Kernel/WorldState.cs`, `Rhodium.Kernel/RhodiumRuntime.cs`, `Rhodium.Platform/Patterns/EngineLoops.cs`, `Rhodium.Platform/Patterns/StrategyEventProcessor.cs`.
- Tests: `Rhodium.Control.Tests/WorldStateTests.cs`, `Rhodium.Kernel.Tests/MemoryLeakTests.cs`.

## Do Not Depend On

- Do not share state between strategies except through documented parent/child snapshots and allocation controls.
- Do not assume allocation commands mutate another strategy mid-hook.
- Do not retain spans or contexts beyond the hook that received them.
