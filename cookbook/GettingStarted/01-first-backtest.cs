#:package Rhodium.Simulation@0.5.0
#:property TargetFramework=net10.0

// This sample runs the smallest useful Rhodium backtest from in-memory bar data.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

var history = SharedHistory.Load([CreateBarClosed(123m)]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<FirstBacktestStrategy>()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .Run();

Console.WriteLine($"Order intents: {result.OrderIntents.Count}");
Console.WriteLine($"Execution events: {result.ExecutionEvents.Count}");
Console.WriteLine($"Runs: {result.Runs.Count}");

var run = result.Runs[0];
Console.WriteLine($"Final positions: {run.FinalSnapshot.GetPositions().Length}");
Console.WriteLine($"Total trades: {run.TearSheet.TotalTrades}");

static BarClosed CreateBarClosed(decimal close)
{
    var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
    var bar = new Bar(
        new Price(close, Currency.USD),
        new Price(close + 1m, Currency.USD),
        new Price(close - 1m, Currency.USD),
        new Price(close, Currency.USD),
        new Qty(10_000m),
        default,
        Duration.FromMinutes(1));

    return new BarClosed(instrument, bar);
}

public sealed partial class FirstBacktestStrategy : Strategy
{
    private AssetId _spy;
    private bool _submitted;

    [BarField(ReadOnly = true)]
    public partial double Close { get; }

    protected override void OnInitialize(in SetupContext setup)
    {
        _spy = setup.AddEquity("SPY");
    }

    partial void OnBar(ref BarContext bar)
    {
        if (bar.AssetId != _spy || _submitted)
            return;

        _submitted = true;
        bar.Buy(new Qty(1m), Execution.Limit().At(new Price((decimal)bar.Close, Currency.USD)));
    }
}
