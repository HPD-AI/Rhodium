# Exporters

Rhodium exposes three export families:

- Run metrics from `SimulationResult.Analyze()`.
- Account, custody, and transfer CSV exporters.
- Directory bundles from `BacktestArtifactExporter`.

Public export methods are synchronous. They write UTF-8 CSV or Parquet and create parent directories.

## Run Metrics

Start from `result.Analyze()`:

```csharp
var analyzer = result.Analyze();

var csv = analyzer.ToCsv();
analyzer.ExportToCsv("artifacts/run_metrics.csv");
analyzer.ExportToParquet("artifacts/run_metrics.parquet");
```

CSV columns are:

```text
strategy_id,variant_index,total_return,sharpe,max_drawdown,win_rate,total_trades,<parameter names>
```

Parameter names are gathered across runs, sorted ordinally, and emitted without a prefix in CSV. Values use invariant culture and CSV escaping. Rows are sorted by `variant_index`.

Parquet columns are the same fixed metrics followed by string parameter columns named `param_<sanitized parameter name>`. Sanitization replaces non-letter, non-digit, and non-underscore characters with `_`. The writer emits one row group.

Run-metric exports do not include every `TearSheet` field.

## Account Artifacts

Use these directly when you want one CSV string or file:

```csharp
var statementsCsv = AccountStatementExporter.ToCsv(result.AccountStatements);
AccountStatementExporter.ExportToCsv(result.AccountStatements, "artifacts/account_statements.csv");
```

Exporters:

- `AccountStatementExporter.ToCsv(...)` and `ExportToCsv(...)`
- `CustodyPositionExporter.ToCsv(...)` and `ExportToCsv(...)`
- `AccountTransferExporter.ToCsv(...)` and `ExportToCsv(...)`

Ordering is stable:

- Account statements: by `Time`.
- Custody positions: by time, strategy id, variant id, asset symbol, then venue.
- Account transfers: by status time, then transfer id.

Account transfer rows leave missing cash, instrument, and destination fields empty.

## Directory Bundle

Use `BacktestArtifactExporter.ExportToDirectory(...)` for account/custody/transfer artifacts plus a manifest:

```csharp
var transfers = result.SimulatorEvents
    .OfType<AccountTransferStatusSnapshot>()
    .ToArray();

var manifest = BacktestArtifactExporter.ExportToDirectory(
    result.AccountStatements,
    result.SimulatorEvents.OfType<CustodyPositionSnapshot>().ToArray(),
    "artifacts/backtest",
    transfers);
```

The bundle writes:

- `account_statements.csv`
- `custody_positions.csv`
- `account_transfers.csv`
- `manifest.csv`

`manifest.csv` has columns:

```text
artifact,file_name,row_count
```

Manifest rows cover `account_statements`, `custody_positions`, and `account_transfers`.

Copy `08-export-results.cs` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted) for the intended metrics-plus-artifacts flow.

## Do Not Assume

- There is no public async export API.
- There is no tested CSV or Parquet import API.
- Export files are not a full `SimulationResult` snapshot.
- Artifact CSVs do not include order intents or execution events.
