#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0

// This sample writes a custom generated bar field through BarContext.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

GeneratedFieldStrategy.Reset();

var history = SharedHistory.Load([
    CreateBarClosed(100m),
    CreateBarClosed(101m)
]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<GeneratedFieldStrategy>()
    .WithHistory(history)
    .Run();

Console.WriteLine($"Bars seen: {GeneratedFieldStrategy.BarCount}");
Console.WriteLine($"Last signal: {GeneratedFieldStrategy.LastSignal:N1}");
Console.WriteLine($"Order intents: {result.OrderIntents.Count}");

static BarClosed CreateBarClosed(decimal close)
{
    var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
    var bar = Bar.Create(new Price(close, Currency.USD), new Qty(10_000m), default, Duration.FromMinutes(1));
    return new BarClosed(instrument, bar);
}

public sealed partial class GeneratedFieldStrategy : Strategy
{
    private AssetId _spy;

    public static int BarCount { get; private set; }
    public static double LastSignal { get; private set; }

    [BarField(ReadOnly = false)]
    public partial double Signal { get; set; }

    public static void Reset()
    {
        BarCount = 0;
        LastSignal = 0d;
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
        bar.Signal = BarCount * 10d;
        LastSignal = bar.Signal;
    }
}
