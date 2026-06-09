#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0

// This sample declares an RSI bar indicator and reads it from OnBar.

using Rhodium.Events;
using Rhodium.Indicators.Streaming;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

RsiBacktestStrategy.Reset();

var history = SharedHistory.Load([
    CreateBarClosed(100m),
    CreateBarClosed(101m),
    CreateBarClosed(102m),
    CreateBarClosed(99m),
    CreateBarClosed(98m)
]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<RsiBacktestStrategy>()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .Run();

Console.WriteLine($"Bars seen: {RsiBacktestStrategy.BarCount}");
Console.WriteLine($"RSI ready: {RsiBacktestStrategy.WasReady}");
Console.WriteLine($"Last RSI: {RsiBacktestStrategy.LastRsi:N2}");
Console.WriteLine($"Order intents: {result.OrderIntents.Count}");

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

public sealed partial class RsiBacktestStrategy : Strategy
{
    private AssetId _spy;
    private bool _submitted;

    public static int BarCount { get; private set; }
    public static bool WasReady { get; private set; }
    public static double LastRsi { get; private set; }

    [BarField(Name = "RSI_2", ReadOnly = true)]
    [BarIndicator(typeof(RSI), 2)]
    public partial double Rsi { get; }

    public static void Reset()
    {
        BarCount = 0;
        WasReady = false;
        LastRsi = 0d;
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
        if (!bar.RsiIsReady)
            return;

        WasReady = true;
        LastRsi = bar.Rsi;

        if (!_submitted && bar.Rsi < 50d)
        {
            _submitted = true;
            bar.Buy(new Qty(1m), Execution.Limit().At(new Price((decimal)bar.Rsi + 50m, Currency.USD)));
        }
    }
}
