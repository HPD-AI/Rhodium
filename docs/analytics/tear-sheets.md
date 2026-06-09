# Tear Sheets

`TearSheet` is the per-run performance summary on `StrategyRunResult`:

```csharp
var run = result.Runs.Single();
var sheet = run.TearSheet;

Console.WriteLine(sheet.TotalReturn);
Console.WriteLine(sheet.TotalTrades);
Console.WriteLine(sheet.TotalPnL);
```

Simulation tear sheets are calculated from completed FIFO round trips built from that run's `OrderFilled` events. They summarize closed trades, not open positions.

## What It Contains

Return fields:

- `TotalReturn`
- `Cagr`
- `AnnualizedReturn`

Risk and drawdown fields:

- `SharpeRatio`
- `SortinoRatio`
- `CalmarRatio`
- `MaxDrawdown`
- `MaxDrawdownDuration`

Win/loss and expectancy fields:

- `WinRate`
- `ProfitFactor`
- `PayoffRatio`
- `ExpectancyPerTrade`
- `TotalTrades`
- `WinningTrades`
- `LosingTrades`
- `BreakevenTrades`

P&L and commission fields:

- `TotalPnL`
- `GrossPnL`
- `TotalCommissions`
- `AvgWin`
- `AvgLoss`
- `LargestWin`
- `LargestLoss`

Timing fields:

- `AvgHoldingPeriod`
- `AvgWinHoldingPeriod`
- `AvgLossHoldingPeriod`
- `Period`

## When To Use It

Use `run.TearSheet` for compact strategy performance reporting, ranking, filters, and comparing variants. Use `result.Batch` only when the four batch arrays are enough. Use `ExecutionEvents` and round trips when you need to explain how a specific metric was produced.

For custom analysis:

```csharp
var fills = result.ExecutionEvents.OfType<OrderFilled>();
var roundTrips = RoundTripBuilder.FromFills(fills).ToArray();
var sheet = TearSheet.Calculate(roundTrips, initialCapital);
```

## Edge Cases

Empty or no-completed-trade input produces zero metrics. Commissions are subtracted from return and net P&L. Sharpe is zero when the return standard deviation is zero.

Open positions are not round trips. Inspect `run.FinalSnapshot.GetPositions()` when you need remaining position state after the run.

See [round trips](round-trips.md) for matching behavior and [result analysis](../simulation/result-analysis.md) for execution and final-position surfaces.
