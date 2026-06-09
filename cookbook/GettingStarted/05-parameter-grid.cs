#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0

// This sample scans generated strategy parameters with a small Cartesian grid.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

var grid = ParameterGrid.Create()
    .Add(nameof(GridStrategy.Lots), 1, 2)
    .Add(nameof(GridStrategy.ExitAfterBars), 1, 2);

var history = SharedHistory.Load([
    CreateBarClosed(100m),
    CreateBarClosed(110m),
    CreateBarClosed(112m)
]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<GridStrategy>()
    .WithHistory(history)
    .WithGrid(grid)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .Run();

var top = result.TopByTotalReturn(1).Single();
var csvLines = result.Analyze().ToCsv().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

Console.WriteLine($"Variants: {result.Runs.Count}");
Console.WriteLine($"Top lots: {top.Parameters.Get<int>(nameof(GridStrategy.Lots))}");
Console.WriteLine($"Top exit bars: {top.Parameters.Get<int>(nameof(GridStrategy.ExitAfterBars))}");
Console.WriteLine($"Top trades: {top.TearSheet.TotalTrades}");
Console.WriteLine(csvLines[0]);

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

public sealed partial class GridStrategy : Strategy
{
    private AssetId _spy;
    private int _bars;
    private bool _bought;
    private bool _sold;

    [Param] public int Lots { get; init; }
    [Param] public int ExitAfterBars { get; init; }

    [BarField(ReadOnly = true)]
    public partial double Close { get; }

    protected override void OnInitialize(in SetupContext setup)
    {
        _spy = setup.AddEquity("SPY");
    }

    partial void OnBar(ref BarContext bar)
    {
        if (bar.AssetId != _spy)
            return;

        _bars++;

        if (!_bought)
        {
            _bought = true;
            bar.Buy(new Qty(Lots), Execution.Market());
            return;
        }

        if (!_sold && _bars > ExitAfterBars)
        {
            _sold = true;
            bar.Sell(new Qty(Lots), Execution.Market());
        }
    }
}
