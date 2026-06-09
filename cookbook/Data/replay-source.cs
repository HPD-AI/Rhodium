#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0

// This sample wraps an HPD replay source in a Rhodium simulation data plan.

using HPD.Events;
using HPD.Events.Core;
using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;
using Rhodium.Simulation.Data;

var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
var start = Instant.FromUnixSeconds(1_700_000_000);
var replaySource = new EnumerableReplaySource<FinanceEvent>(
[
    CreateBarClosed(instrument, start + Duration.FromMinutes(0), close: 100m),
    CreateBarClosed(instrument, start + Duration.FromMinutes(1), close: 101m),
    CreateBarClosed(instrument, start + Duration.FromMinutes(2), close: 102m)
]);

var readOptions = new ReplayReadOptions(
    From: start.ToDateTimeOffset(),
    To: (start + Duration.FromMinutes(2)).ToDateTimeOffset(),
    EventFlowId: null,
    Limit: null);

var replayed = await replaySource.ReadAsync(readOptions).ToArrayAsync();
var plan = SimulationDataPlan
    .Create(readOptions)
    .AddSource("bars-replay-source", replaySource);

var result = await Rhodium.Simulation.Rhodium
    .Simulate<ReplaySourceStrategy>()
    .WithData(plan)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .RunAsync();

Console.WriteLine($"Replayed events: {replayed.Length}");
Console.WriteLine($"Plan sources: {plan.SourceCount}");
Console.WriteLine($"Order intents: {result.OrderIntents.Count}");
Console.WriteLine($"Filled: {result.ExecutionEvents.OfType<OrderFilled>().Count()}");

static BarClosed CreateBarClosed(Instrument instrument, Instant time, decimal close)
{
    var bar = new Bar(
        new Price(close, Currency.USD),
        new Price(close + 1m, Currency.USD),
        new Price(close - 1m, Currency.USD),
        new Price(close, Currency.USD),
        new Qty(10_000m),
        time,
        Duration.FromMinutes(1));

    return new BarClosed(instrument, bar)
    {
        Time = time
    };
}

public sealed partial class ReplaySourceStrategy : Strategy
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
