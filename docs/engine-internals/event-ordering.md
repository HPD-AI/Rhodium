# Event Ordering

Use this page when a result looks out of order: timers firing before a replay event, modules processing after a timestamp group, or a fill cascade stopping with a same-timestamp iteration error.

## Replay Turn Rule

For each replay event, the session:

1. Drains the previous timestamp with modules enabled when the timestamp changes.
2. Advances the clock to the replay event time.
3. Processes due scheduled timer events.
4. Drains already-due exchange, execution, and simulator work at that timestamp.
5. Projects/processes the replay event through the session.
6. Drains the same timestamp again.

At completion, the active timestamp is drained with modules enabled, exchanges complete replay, and final due work is drained.

## Quiescence

`DrainReplayTurn` repeats while exchanges, execution events, simulator events, or optional module processing make progress. `SimulationRunOptions.MaxSameTimestampIterations` bounds this loop so a same-timestamp fill or module cascade cannot run forever.

Tests assert that modules see all same-timestamp replay events after that timestamp group is complete, and that a runaway fill cascade is stopped by the iteration cap.

## Schedules

Schedules are bound after account seeding and strategy initialization. Recurring schedules use `Clock.SetTimer`; one-shot schedules require `FireAt` and use `Clock.SetAlert`. Timer callbacks enqueue `Scheduled` lifecycle events targeted to the owning strategy id.

## Evidence

- Source: `Rhodium.Simulation/SimulationSession.cs`, `Rhodium.Simulation/SimulationRunOptions.cs`, `Rhodium.Platform/SetupContext.cs`.
- Tests: `Rhodium.Simulation.Tests/SimulationSessionArchitectureTests.cs` covers module ordering, runaway cascades, and `ScheduleEvery`/`OnScheduled`.

## Do Not Depend On

- Do not assume raw replay market events are retained as simulator events. Replay market events are projected into runtime and exchange state; simulator events are generated/auxiliary finance and account/lifecycle effects.
- Do not assume modules process after every individual event in a same-timestamp group.
