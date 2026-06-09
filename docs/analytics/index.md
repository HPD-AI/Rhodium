# Analytics

Start analytics from a completed `SimulationResult`:

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithHistory(history)
    .Run();
```

Use `result.Runs` when you need per-strategy or per-variant output. Each `StrategyRunResult` contains `StrategyId`, `VariantIndex`, `Parameters`, `TearSheet`, and `FinalSnapshot`.

Use `run.TearSheet` for completed-trade performance metrics:

```csharp
foreach (var run in result.Runs)
{
    Console.WriteLine(run.VariantIndex);
    Console.WriteLine(run.TearSheet.TotalReturn);
    Console.WriteLine(run.TearSheet.SharpeRatio);
}
```

Use `result.Batch` for compact batch arrays across the same run order. It exposes only `TotalReturn`, `Cagr`, `Sharpe`, and `MaxDrawdown`; use each run's `TearSheet` for win/loss, P&L, commissions, holding periods, and period details.

Use `result.Analyze()` for grid and vector-scan exploration:

```csharp
var analyzer = result.Analyze();
var top = analyzer.TopByTotalReturn(5);
var csv = analyzer.ToCsv();

analyzer.ExportToCsv("artifacts/run_metrics.csv");
```

`SimulationResult.TopBySharpe(count)` and `TopByTotalReturn(count)` are shortcuts over the analyzer.

Use `ExecutionEvents` when a summary metric needs an order-level explanation:

```csharp
var fills = result.ExecutionEvents
    .OfType<OrderFilled>()
    .OrderBy(static fill => fill.Time)
    .ToArray();
```

Use `FinalSnapshot` for remaining positions. Open positions can appear there without contributing completed trades to the tear sheet.

Use `AccountStatements` and finance events in `SimulatorEvents` for account and custody exports. `SimulatorEvents` are not a retained raw market-data replay log.

## Copy These Examples

- Parameter scans: `05-parameter-grid.cs` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted)
- Metrics and account artifacts: `08-export-results.cs` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted)
- Result shape details: [result analysis](../simulation/result-analysis.md)
- Diagnostics surface: [diagnostics](../reference/diagnostics.md)

## Do Not Assume

- `BatchTearSheet` replaces per-run `TearSheet`.
- Every fill becomes a completed trade; unmatched open fills are excluded from round-trip metrics.
- Analyzer CSV or Parquet contains the full tear sheet.
- Exported files can rehydrate a full `SimulationResult`.
