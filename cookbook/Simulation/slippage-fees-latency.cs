#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0
// This sample applies latency, slippage, and maker/taker fees to simulated fills.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

var history = SharedHistory.Load([
    CreateBarClosed(123m, Instant.Epoch),
    CreateTradeOccurred(123m, 1m, Side.Sell, Instant.FromUnixSeconds(1))
]);
var config = SimulationConfig.Instant() with
{
    Latency = new LatencyParams(Duration.FromMillis(2), Duration.FromMillis(1)),
    Slippage = SlippageParams.VolumeProportional(bpsPerLotSize: 10m),
    Fees = FeeParams.MakerTaker(makerBps: 5m, takerBps: 11m)
};

var result = Rhodium.Simulation.Rhodium
    .Simulate<CostedFillStrategy>()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.FastVectorApproximation)
    .WithConfig(config)
    .Run();

var fill = result.ExecutionEvents.OfType<OrderFilled>().Single();

Console.WriteLine($"Fill price: {fill.FillPrice.Value:N3}");
Console.WriteLine($"Commission: {fill.Commission.Amount:N7}");
Console.WriteLine($"Latency samples: {result.Diagnostics.Latency.CommandCount}");

static BarClosed CreateBarClosed(decimal close, Instant time)
{
    var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
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

static TradeOccurred CreateTradeOccurred(decimal price, decimal size, Side aggressorSide, Instant time)
{
    var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
    var trade = new Trade(
        new Price(price, Currency.USD),
        new Qty(size),
        aggressorSide,
        DualTimestamp.Synchronized(time));

    return new TradeOccurred(instrument, trade);
}

public sealed partial class CostedFillStrategy : Strategy
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
        bar.Buy(new Qty(1m), Execution.Limit().At(new Price(123m, Currency.USD)));
    }
}
