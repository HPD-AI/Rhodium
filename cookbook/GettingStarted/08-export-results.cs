#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0

// This sample exports run metrics and account artifacts to CSV files.

using Rhodium.Analytics;
using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

var history = SharedHistory.Load([
    CreateBarClosed(100m),
    CreateBarClosed(110m)
]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<ExportStrategy>()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .Run();

var directory = Path.Combine(Path.GetTempPath(), "rhodium-cookbook-export");
var metricsPath = Path.Combine(directory, "run_metrics.csv");

Directory.CreateDirectory(directory);
result.Analyze().ExportToCsv(metricsPath);

var transfers = result.SimulatorEvents.OfType<AccountTransferStatusSnapshot>().ToArray();
var manifest = BacktestArtifactExporter.ExportToDirectory(
    result.AccountStatements,
    result.SimulatorEvents.OfType<CustodyPositionSnapshot>().ToArray(),
    directory,
    transfers);

var csvPreview = result.Analyze().ToCsv().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[0];

Console.WriteLine(csvPreview);
Console.WriteLine($"Metrics file: {File.Exists(metricsPath)}");
Console.WriteLine($"Account rows: {manifest.AccountStatementCount}");
Console.WriteLine($"Transfer rows: {manifest.AccountTransferCount}");
Console.WriteLine($"Directory: {directory}");

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

public sealed partial class ExportStrategy : Strategy
{
    private AssetId _spy;
    private int _bars;

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
        if (_bars == 1)
            bar.Buy(new Qty(1m), Execution.Market());
        else if (_bars == 2)
            bar.Sell(new Qty(1m), Execution.Market());
    }
}
