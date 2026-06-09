# Strategy Dispatch

Use this page when a hook runs, skips, or fails in a way that depends on initialization, hierarchy depth, pause state, generated hooks, or execution-event targeting.

## Initialization Rule

Initialization runs by strategy-tree depth. Each strategy receives setup, calls `OnInitialize`, records registered assets/schedules, then generated initialization captures the batch-map version. Strategies can add instruments and register generated fields only during setup/initialization.

Hot-path dispatch rejects topology changes after initialization by comparing the captured universe version with `MarketKernel.UniverseVersion`.

## Market Dispatch

Market dispatch is hierarchical by depth. Each strategy receives fresh scratch buffers for counters, allocation commands, and order intents, plus child snapshots and a `PortfolioContext`. Paused strategies are skipped. Non-invariant exceptions are passed to `OnError`; invariant exceptions are rethrown.

Bars and ticks may use parallel dispatch by depth when enabled. Quote, trade, book snapshot, book-level delta, and book-level-deltas dispatch is currently instrument-range filtered and sequential in the session wiring.

## Generated Hooks

Generated hooks supported by the generator are `OnTick`, `OnQuote`, `OnTrade`, `OnBookSnapshot`, `OnBookLevelDelta`, `OnBookLevelDeltas`, and `OnBar`.

Quote/trade/book hooks visit registered assets whose virtual index falls within the event instrument range. Bar/tick hooks iterate all assets registered by the strategy. Generated contexts use `ref PortfolioContext` and context frames; the strategy instance does not store a reusable `MarketKernel`.

## Execution And Lifecycle

Execution events dispatch only to the event strategy id. Position transition hooks fire when the transition is non-empty. A filled order can trigger a group-only hierarchical pass afterward so parent/group logic observes changed child state.

## Evidence

- Source: `Rhodium.Platform/Patterns/StrategyEventProcessor.cs`, `Rhodium.Platform/Patterns/EngineLoops.cs`, `Rhodium.Platform/Strategy.Core.cs`, `Rhodium.Generators/StrategyGenerator.cs`.
- Tests: `Rhodium.Generators.Tests/StrategyGeneratorTests.cs`, `Rhodium.Platform.Tests/EngineLoopsTests.cs`.

## Do Not Depend On

- Do not assume quote/trade/book hooks are parallel-dispatched.
- Do not mutate universe topology after initialization.
- Do not depend on paused strategies receiving market hooks.
