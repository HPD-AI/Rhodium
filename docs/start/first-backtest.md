# First backtest

A minimal Rhodium backtest has four pieces:

1. A strategy type with a public parameterless constructor.
2. Instruments registered in `OnInitialize`.
3. Replay events loaded into `SharedHistory`.
4. `Rhodium.Simulate<TStrategy>().WithHistory(history).Run()`.

The first example uses one SPY bar and a strategy that buys once when that bar closes.

```csharp
var history = SharedHistory.Load([CreateBarClosed(123m)]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<FirstBacktestStrategy>()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .Run();
```

`WithHistory` selects materialized in-memory replay. If neither `WithHistory` nor `WithData` is called, the builder throws because there is no simulation input.

`MatchingFidelity.FastVectorApproximation` is useful for the first example because tests show it can fill a touched limit order inside the same event boundary. The default fidelity is queue-accurate, which can accept an order on the submission bar without filling it until later market data.

After `Run()`, inspect `SimulationResult`:

- `OrderIntents`: orders the strategy requested.
- `ExecutionEvents`: accepted, filled, rejected, canceled, and expired events.
- `Runs`: per-strategy run summaries with final snapshot and tear sheet.
- `Batch`: vectorized batch metrics for comparing runs.

Full runnable example:

```text
cookbook/GettingStarted/01-first-backtest.cs
```

Open the [GettingStarted cookbook folder](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted) to copy the file.

Run it:

```bash
dotnet run cookbook/GettingStarted/01-first-backtest.cs
```

Next copyable example:

```text
cookbook/GettingStarted/02-first-strategy.cs
cookbook/GettingStarted/03-bar-indicator.cs
```

Then continue through the rest of the copyable sequence:

```text
cookbook/GettingStarted/04-order-intents.cs
cookbook/GettingStarted/05-parameter-grid.cs
cookbook/GettingStarted/06-venue-config.cs
cookbook/GettingStarted/07-account-seed.cs
cookbook/GettingStarted/08-export-results.cs
```

The locally verified first-run outputs cover one submitted order intent for `01`, one filled buy at `123.00 USD` for `02`, five bars and a ready RSI for `03`, order accept/modify/cancel/fill flow for `04`, four parameter variants and CSV output for `05`, NASDAQ cash-account configuration for `06`, passing cash and position seed outputs for `07`, and exported metrics/account rows for `08`.

Source evidence:

- `Rhodium.Simulation/SimulationBuilder.cs`
- `Rhodium.Simulation/SharedHistory.cs`
- `Rhodium.Simulation.Tests/SimulationSessionBehaviorTests.cs`
