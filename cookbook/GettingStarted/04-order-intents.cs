#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0

// This sample uses generated order helpers for submit, cancel, modify, and fill follow-up intents.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

IntentStrategy.Reset();

var history = SharedHistory.Load([
    CreateBarClosed(100m),
    CreateBarClosed(101m)
]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<IntentStrategy>()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .Run();

Console.WriteLine($"Order intents: {result.OrderIntents.Count}");
Console.WriteLine($"Accepted: {result.ExecutionEvents.OfType<OrderAccepted>().Count()}");
Console.WriteLine($"Modified: {result.ExecutionEvents.OfType<OrderModified>().Count()}");
Console.WriteLine($"Cancelled: {result.ExecutionEvents.OfType<OrderCancelled>().Count()}");
Console.WriteLine($"Fills: {result.ExecutionEvents.OfType<OrderFilled>().Count()}");
Console.WriteLine($"Follow-up sells: {IntentStrategy.FollowUpSells}");

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

public sealed partial class IntentStrategy : Strategy
{
    private AssetId _spy;
    private bool _submitted;
    private bool _cancelled;
    private bool _modified;
    private bool _sold;

    public static int FollowUpSells { get; private set; }

    [BarField(ReadOnly = true)]
    public partial double Close { get; }

    public static void Reset()
    {
        FollowUpSells = 0;
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
        bar.Buy(new Qty(1m), Execution.Market());
        bar.Buy(new Qty(1m), Execution.Limit().At(new Price(90m, Currency.USD)));
        bar.Buy(new Qty(1m), Execution.Limit().At(new Price(91m, Currency.USD)));
    }

    protected override void OnOrderAccepted(ref OrderContext order)
    {
        if (order.AssetId != _spy)
            return;

        if (!_cancelled)
        {
            _cancelled = true;
            order.Cancel("strategy cancel");
            return;
        }

        if (!_modified)
        {
            _modified = true;
            order.Modify(newLimitPrice: new Price(101m, Currency.USD));
        }
    }

    protected override void OnOrderFilled(ref FillContext fill)
    {
        if (_sold || fill.AssetId != _spy || fill.Side != Side.Buy)
            return;

        _sold = true;
        FollowUpSells++;
        fill.Sell(fill.FilledQty, Execution.Market());
    }
}
