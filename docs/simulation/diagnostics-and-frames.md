# Diagnostics And Frames

Normal simulation output is available without enabling frames. Start with `SimulationResult`, then use diagnostics when you need to explain venue state, rejected orders, data provenance, module activity, or low-level timing.

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithHistory(history)
    .Run();

var diagnostics = result.Diagnostics;
```

## Diagnostics

`SimulationDiagnostics` includes:

- `Venues`: final cash, available cash, reserved cash, settlement and delivery counts, account type, base currency, order counters, and policies.
- `Instruments`: status, matching fidelity, policies, marks, and order counters.
- `Rejections`: venue, instrument, order, and rejection reason records.
- `DataSources`: source id, priority, ordinal, kind, and effective read options for data-plan runs.
- `Modules`: module calls, emitted events, commands, frames, counters, metrics, and messages.
- `FrameStats`: struct-event hub statistics.
- `Latency`, `Timing`, and `Quiescence`.

Venues and instruments appear when the simulation creates or touches them. Empty replay can finish with empty venue diagnostics.

Use diagnostics with execution events:

```csharp
var rejections = result.ExecutionEvents.OfType<OrderRejected>().ToArray();
var rejectionDiagnostics = result.Diagnostics.Rejections;
```

`OrderRejected` tells you the exchange-side outcome. Diagnostics give the broader venue, instrument, and policy context.

## Account And Lifecycle Diagnostics

Some account details are easiest to read from result event streams:

```csharp
var statements = result.AccountStatements;
var settlementEvents = result.SimulatorEvents.OfType<SettlementScheduled>();
var margin = result.SimulatorEvents.OfType<MarginStatusSnapshot>();
var lifecycle = result.SimulatorEvents.OfType<OptionLifecycleApplied>();
```

Use `Diagnostics.Venues` for final venue-level state. Use `AccountStatements` and `SimulatorEvents` when you need the sequence of account changes.

## Frames

Frames are optional local struct-event lanes on the simulation session. They are for low-allocation consumers and module integrations; ordinary object events, account statements, simulator events, and diagnostics do not require them.

Frame mode defaults to disabled:

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithHistory(history)
    .WithFrameMode(SimulationFrameMode.All)
    .Run();
```

`SimulationFrameMode` values are:

- `Disabled`
- `MarketData`
- `Execution`
- `Diagnostics`
- `All`

Market-data frames can project quotes, depth, and book-order events when the replay stream contains that information. Execution frames can project fills. Module frame sinks respect the enabled frame categories.

Treat frames as an advanced local integration surface. For normal backtest analysis, prefer `SimulationResult` and `SimulationDiagnostics`.

Related pages:

- [Result analysis](result-analysis.md)
- [Modules](modules.md)
