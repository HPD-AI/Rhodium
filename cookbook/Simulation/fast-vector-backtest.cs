#:package Rhodium.Simulation@0.5.0
#:property TargetFramework=net10.0
// This sample shows fast vector matching for quick screening backtests.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

FastVectorFillStrategy.Reset();

var history = SharedHistory.Load([CreateBarClosed(123m)]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<FastVectorFillStrategy>()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .Run();

var fill = result.ExecutionEvents.OfType<OrderFilled>().Single();

Console.WriteLine($"Order intents: {result.OrderIntents.Count}");
Console.WriteLine($"Accepted: {result.ExecutionEvents.OfType<OrderAccepted>().Count()}");
Console.WriteLine($"Filled: {result.ExecutionEvents.OfType<OrderFilled>().Count()}");
Console.WriteLine($"Fill price: {fill.FillPrice.Value:N2}");

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

public sealed partial class FastVectorFillStrategy : Strategy
{
    private AssetId _spy;
    private bool _submitted;

    public static int FillCount { get; private set; }

    [BarField(ReadOnly = true)]
    public partial double Close { get; }

    public static void Reset()
    {
        FillCount = 0;
    }

    protected override void OnInitialize(in SetupContext setup)
    {
        _spy = setup.AddEquity("SPY");
    }

    partial void OnBar(ref BarContext bar)
    {
        if (_submitted || bar.AssetId != _spy)
            return;

        _submitted = true;
        bar.Buy(new Qty(1m), Execution.Limit().At(new Price(123m, Currency.USD)));
    }

    protected override void OnOrderFilled(ref FillContext fill)
    {
        FillCount++;
    }
}
