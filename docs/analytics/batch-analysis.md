# Batch Analysis

Batch analysis starts from a `SimulationResult`, usually one produced with a parameter grid:

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<GridStrategy>()
    .WithHistory(history)
    .WithGrid(grid)
    .Run();
```

Use `result.Runs` for exact run rows and parameters. Use `result.Analyze()` for ranking, filtering, heatmaps, and export:

```csharp
var analyzer = result.Analyze();

var bestBySharpe = analyzer.TopBySharpe(10);
var bestByReturn = result.TopByTotalReturn(10);

var candidates = analyzer.Filter(
    minSharpe: 1.0,
    maxDrawdown: 0.20m,
    minWinRate: 0.50m,
    minTrades: 5);
```

`TopBySharpe` and `TopByTotalReturn` rank descending. `Filter` applies every threshold you provide.

## Heatmaps

`ToHeatmap` returns a `double[,]` indexed as `[y, x]`:

```csharp
var heatmap = analyzer.ToHeatmap(
    static run => (double)run.TearSheet.TotalReturn,
    xParameter: "Fast",
    yParameter: "Slow");
```

Parameter values are taken in first-seen order, and missing cells are `double.NaN`. The returned array does not include axis labels; keep or reconstruct the parameter values if you need labeled output.

## Batch Tear Sheet

`result.Batch` is a `BatchTearSheet` struct-of-arrays:

- `TotalReturn`
- `Cagr`
- `Sharpe`
- `MaxDrawdown`

It mirrors per-run tear sheets in run order. For lower-level batch work, `BatchTearSheetBuilder` can build a batch from tear sheets or variant-indexed round trips, return top variant indexes with `GetTopVariants`, and calculate summary statistics with `GetSummary`.

Use per-run `TearSheet` when you need metrics outside the four batch arrays.

## Parameter Round Trips

`result.ToParameterGrid()` and `result.Runs.ToParameterGrid()` rebuild an exact-row `ParameterGrid` from completed run parameters:

```csharp
var finalists = result.TopByTotalReturn(5).ToParameterGrid();
```

The recovered grid preserves observed rows. It does not reconstruct the original Cartesian axes and cannot be extended with `Add(...)`.

A common workflow is to run a broad fast-vector scan, take finalist rows, and rerun those exact rows with a queue-accurate simulation.

## Export

Use analyzer export for run metrics:

```csharp
var csv = analyzer.ToCsv();
analyzer.ExportToCsv("artifacts/run_metrics.csv");
analyzer.ExportToParquet("artifacts/run_metrics.parquet");
```

CSV and Parquet rows are ordered by `VariantIndex`. The schema is intentionally narrow: strategy id, variant index, total return, Sharpe, max drawdown, win rate, total trades, and parameter values.

Copy `05-parameter-grid.cs` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted) for the scan flow and `08-export-results.cs` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted) for export.
