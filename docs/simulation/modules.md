# Modules

Simulation modules are advanced deterministic extensions. Use them when a run needs custom simulator-owned events, exchange commands, struct frames, or diagnostics that should participate in the replay lifecycle.

Ordinary backtests do not need modules.

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithSessionModule(new MySessionModule())
    .WithHistory(history)
    .Run();
```

## Module Types

Public module interfaces map to builder methods:

- `ISessionSimulationModule` with `WithSessionModule(...)`
- `IVenueSimulationModule` with `WithVenueModule(...)`
- `IInstrumentSimulationModule` with `WithInstrumentModule(...)`

All modules share the base lifecycle:

- `Reset`
- `PreProcess`
- `Process`
- `AppendDiagnostics`

`PreProcess` runs before normal processing for a replay turn. Module-emitted events can enter the same timestamp turn. `Process` runs after all same-timestamp events for the turn have been drained. Diagnostics are appended into `result.Diagnostics.Modules`.

## Context And Sinks

`SimulationModuleContext` is read-only inspection context. It exposes the clock, market, venue count, venue lookup, and instrument lookup.

Venue and instrument module views expose status, account state, policies, marks, and order counters for their scope.

Effects go through `SimulationModuleSinks`:

```csharp
sinks.Events.Emit(financeEvent);
sinks.Commands.Submit(command);
sinks.Commands.Cancel(command);
sinks.Commands.Modify(command);
sinks.Frames.Emit(frame);
```

Event sinks emit module-owned semantic events into the simulation turn. Command sinks route through the simulated exchange. Frame sinks write optional struct frames when the enabled `SimulationFrameMode` allows that category.

## Diagnostics

Use `AppendDiagnostics` for module counters, metrics, or messages that should appear in the final result:

```csharp
var moduleDiagnostics = result.Diagnostics.Modules;
```

Diagnostics are the supported way to explain what a module did. Avoid relying on private module state after a run unless the module owns that contract.

## When To Use

Use modules for deterministic simulation extensions such as:

- emitting custom finance events during replay
- routing simulator-owned exchange commands
- producing low-allocation frames for local consumers
- collecting module-scoped diagnostics

Do not use modules as a replacement for normal strategy logic, account setup, or result analysis. Start with strategy callbacks, builder configuration, account seeds, and `SimulationResult`; add modules only when the extension must live inside the simulation engine lifecycle.

Related pages:

- [Diagnostics and frames](diagnostics-and-frames.md)
- [Streaming data](streaming-data.md)
