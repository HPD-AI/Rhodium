#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0

// This sample fires a setup schedule from replay time and uses timer order helpers.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Primitives;
using Rhodium.Simulation;

ScheduledTimerStrategy.Reset();

var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
var history = SharedHistory.Load([
    CreateBarClosed(instrument, 100m, unixSeconds: 1),
    CreateBarClosed(instrument, 101m, unixSeconds: 6),
    CreateBarClosed(instrument, 102m, unixSeconds: 11)
]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<ScheduledTimerStrategy>()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .Run();

Console.WriteLine($"Schedule: {ScheduledTimerStrategy.LastScheduleName}");
Console.WriteLine($"Scheduled calls: {ScheduledTimerStrategy.ScheduledCount}");
Console.WriteLine($"Last scheduled: {ScheduledTimerStrategy.LastScheduledAt}");
Console.WriteLine($"Order intents: {result.OrderIntents.Count}");
Console.WriteLine($"Fills: {result.ExecutionEvents.OfType<OrderFilled>().Count()}");

static BarClosed CreateBarClosed(Instrument instrument, decimal close, long unixSeconds)
{
    var time = Instant.FromUnixSeconds(unixSeconds);
    var bar = new Bar(
        new Price(close, Currency.USD),
        new Price(close + 1m, Currency.USD),
        new Price(close - 1m, Currency.USD),
        new Price(close, Currency.USD),
        new Qty(10_000m),
        time,
        Duration.FromMinutes(1));

    return new BarClosed(instrument, bar);
}

public sealed partial class ScheduledTimerStrategy : Strategy
{
    private AssetId _spy;
    private bool _submitted;

    public static int ScheduledCount { get; private set; }
    public static string LastScheduleName { get; private set; } = "";
    public static Instant LastScheduledAt { get; private set; }

    public static void Reset()
    {
        ScheduledCount = 0;
        LastScheduleName = "";
        LastScheduledAt = default;
    }

    protected override void OnInitialize(in SetupContext setup)
    {
        _spy = setup.AddEquity("SPY");
        setup.ScheduleEvery("rebalance", Duration.FromSeconds(5));
    }

    protected override void OnScheduled(ref TimerContext timer)
    {
        if (timer.Name != "rebalance")
            return;

        ScheduledCount++;
        LastScheduleName = timer.Name;
        LastScheduledAt = timer.Time;

        if (!_submitted)
        {
            _submitted = true;
            timer.Buy(_spy, new Qty(1m), Execution.Market());
        }
    }
}
