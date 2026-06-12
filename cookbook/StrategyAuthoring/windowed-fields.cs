#:package Rhodium.Simulation@0.5.0
#:property TargetFramework=net10.0

// This sample uses a generated rolling window over the close field.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

WindowedFieldStrategy.Reset();

var history = SharedHistory.Load([
    CreateBarClosed(100m),
    CreateBarClosed(101m),
    CreateBarClosed(102m),
    CreateBarClosed(103m)
]);

_ = Rhodium.Simulation.Rhodium
    .Simulate<WindowedFieldStrategy>()
    .WithHistory(history)
    .Run();

Console.WriteLine($"Bars seen: {WindowedFieldStrategy.BarCount}");
Console.WriteLine($"Last window length: {WindowedFieldStrategy.LastWindowLength}");
Console.WriteLine($"Last mean: {WindowedFieldStrategy.LastMean:N2}");

static BarClosed CreateBarClosed(decimal close)
{
    var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
    var bar = Bar.Create(new Price(close, Currency.USD), new Qty(10_000m), default, Duration.FromMinutes(1));
    return new BarClosed(instrument, bar);
}

public sealed partial class WindowedFieldStrategy : Strategy
{
    private AssetId _spy;

    public static int BarCount { get; private set; }
    public static int LastWindowLength { get; private set; }
    public static double LastMean { get; private set; }

    [BarField(ReadOnly = true)]
    [Window(3)]
    public partial double Close { get; }

    public static void Reset()
    {
        BarCount = 0;
        LastWindowLength = 0;
        LastMean = 0d;
    }

    protected override void OnInitialize(in SetupContext setup)
    {
        _spy = setup.AddEquity("SPY");
    }

    partial void OnBar(ref BarContext bar)
    {
        if (bar.AssetId != _spy)
            return;

        BarCount++;
        var window = bar.Close.Window(3);
        LastWindowLength = window.Length;
        LastMean = window.Mean();
    }
}
