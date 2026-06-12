#:package Rhodium.Simulation@0.5.0
#:property TargetFramework=net10.0

// This sample builds a deterministic simulation data plan from named event sources.

using HPD.Events;
using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;
using Rhodium.Simulation.Data;

var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
var start = Instant.FromUnixSeconds(1_700_000_000);

var quotes = new[]
{
    CreateQuoteReceived(instrument, start, bid: 99.95m, ask: 100.05m)
};

var bars = new[]
{
    CreateBarClosed(instrument, start + Duration.FromMinutes(1), close: 100m),
    CreateBarClosed(instrument, start + Duration.FromMinutes(2), close: 101m)
};

var plan = SimulationDataPlan
    .Create(ReplayReadOptions.All with { Limit = 3 })
    .AddSource("quotes-fixture", quotes, priority: 0)
    .AddSource("bars-fixture", bars, priority: 10);

var iterator = new SimulationDataIterator(plan);
var result = Rhodium.Simulation.Rhodium
    .Simulate<DataPlanStrategy>()
    .WithData(iterator)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .Run();

Console.WriteLine($"Sources: {plan.SourceCount}");
Console.WriteLine($"Provenance: {string.Join(", ", iterator.Provenance.Select(p => p.SourceId))}");
Console.WriteLine($"Order intents: {result.OrderIntents.Count}");
Console.WriteLine($"Filled: {result.ExecutionEvents.OfType<OrderFilled>().Count()}");
Console.WriteLine($"Final position: {result.Runs.Single().FinalSnapshot.GetPositions().ToArray().Single().Quantity.Value:N0}");

static QuoteReceived CreateQuoteReceived(
    Instrument instrument,
    Instant time,
    decimal bid,
    decimal ask)
{
    var timestamp = DualTimestamp.Synchronized(time);
    return new QuoteReceived(
        instrument,
        new Quote(
            new Price(bid, Currency.USD),
            new Price(ask, Currency.USD),
            new Qty(100m),
            new Qty(100m),
            timestamp));
}

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

    return new BarClosed(instrument, bar);
}

public sealed partial class DataPlanStrategy : Strategy
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
