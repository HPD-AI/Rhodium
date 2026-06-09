# Start

Rhodium's verified start path is a .NET file app that references Rhodium packages, declares a generated `partial` strategy, loads in-memory replay data, and runs a simulation.

Start here:

1. [Install](install.md): .NET `net10.0`, file apps, and generator/analyzer setup.
2. [First file app](first-file-app.md): the `#:package` and `Directory.Build.props` shape used by the cookbooks.
3. [First backtest](first-backtest.md): a runnable in-memory `SharedHistory` backtest.
4. [First strategy](first-strategy.md): generated hooks, fields, and order helpers.

Then open the [GitHub cookbook folder](https://github.com/HPD-AI/Rhodium/tree/main/cookbook) and work through `cookbook/GettingStarted` in order:

1. `01-first-backtest.cs`: smallest in-memory simulation path.
2. `02-first-strategy.cs`: first generated strategy with a filled buy order.
3. `03-bar-indicator.cs`: generated bar indicator and readiness example.
4. `04-order-intents.cs`: `Buy`, `Sell`, `Cancel`, and `Modify` order-intent flow.
5. `05-parameter-grid.cs`: `[Param]` and `WithGrid` variant scan.
6. `06-venue-config.cs`: venue, cash account, base currency, and fill accounting.
7. `07-account-seed.cs`: positive cash and settled position seeds.
8. `08-export-results.cs`: CSV-first result export with metrics and account rows.

Each cookbook is copyable and can be run directly:

```bash
dotnet run cookbook/GettingStarted/01-first-backtest.cs
```

Replace the filename with `02-first-strategy.cs` through `08-export-results.cs` to continue the sequence. The examples intentionally start with in-memory history, then add generated strategy hooks, indicators, order lifecycle events, parameter grids, venue/account configuration, account seeds, and CSV exports.
