#:package Rhodium.Simulation@0.5.0
#:property TargetFramework=net10.0

// This sample submits one strategy order and prints the resulting fill and position.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

var history = SharedHistory.Load([
    CreateBarClosed(123m),
    CreateBarClosed(124m)
]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<FirstStrategy>()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .Run();

var fills = result.ExecutionEvents.OfType<OrderFilled>().ToArray();
var positionQty = result.Runs[0].FinalSnapshot.GetPositions().ToArray().Single().Quantity;

Console.WriteLine($"Order intents: {result.OrderIntents.Count}");
Console.WriteLine($"Fills: {fills.Length}");
Console.WriteLine($"Last fill: {fills[^1].Side} {fills[^1].FilledQty} @ {fills[^1].FillPrice}");
Console.WriteLine($"Final position: {positionQty}");

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

public sealed partial class FirstStrategy : Strategy
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
        if (_submitted || bar.AssetId != _spy)
            return;

        _submitted = true;
        bar.Buy(new Qty(1m), Execution.Market());
    }
}
