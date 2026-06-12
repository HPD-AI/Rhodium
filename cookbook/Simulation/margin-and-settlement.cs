#:package Rhodium.Simulation@0.5.0
#:property TargetFramework=net10.0
// This sample shows supported account state: margin admission, margin liquidation, and T+ settlement.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

var spy = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
var tPlusOne = SettlementParams.TPlus(1, ClearingCalendar.ForVenue(Venue.NASDAQ));

var settlementResult = Rhodium.Simulation.Rhodium
    .Simulate<SellSeededPositionStrategy>()
    .WithInitialCash(Money.USD(0m))
    .WithConfig(SimulationConfig.Instant() with
    {
        AccountType = AccountType.Cash,
        Settlement = tPlusOne
    })
    .WithAccountSeed(new AccountSeed(
        Venue.NASDAQ,
        [],
        [new SeedPosition(spy, new Qty(1m), new Price(50m, Currency.USD))]))
    .WithHistory(SharedHistory.Load([
        CreateBarClosed(spy, 100m, Utc(2024, 1, 5, 15, 30, 0)),
        CreateBarClosed(spy, 101m, tPlusOne.GetSettlementTime(Utc(2024, 1, 5, 15, 30, 0)))
    ]))
    .Run();

var marginResult = Rhodium.Simulation.Rhodium
    .Simulate<LeveragedBuyStrategy>()
    .WithInitialCash(Money.USD(300m))
    .WithConfig(SimulationConfig.Instant() with
    {
        AccountType = AccountType.Margin,
        Margin = MarginParams.Leverage(4m)
    })
    .WithHistory(SharedHistory.Load([
        CreateBarClosed(spy, 100m, Instant.FromUnixSeconds(1)),
        CreateBarClosed(spy, 50m, Instant.FromUnixSeconds(2))
    ]))
    .Run();

var settlementScheduled = settlementResult.SimulatorEvents.OfType<SettlementScheduled>().Single();
var settlementReleased = settlementResult.SimulatorEvents.OfType<SettlementReleased>().Single();
var marginStatuses = marginResult.SimulatorEvents.OfType<MarginStatusSnapshot>().ToArray();

Console.WriteLine($"Settlement scheduled: {settlementScheduled.Amount}");
Console.WriteLine($"Settlement date: {settlementScheduled.SettlesAt.ToDateTimeOffset():yyyy-MM-dd}");
Console.WriteLine($"Settlement released: {settlementReleased.Amount}");
Console.WriteLine($"Cash after release: {settlementResult.Diagnostics.Venues.Single().Cash}");
Console.WriteLine($"Margin fills: {marginResult.ExecutionEvents.OfType<OrderFilled>().Count()}");
Console.WriteLine($"Margin call issued: {marginResult.SimulatorEvents.OfType<MarginCallIssued>().Count()}");
Console.WriteLine($"Maintenance breached: {marginStatuses.Count(static status => status.IsMaintenanceBreached)}");
Console.WriteLine($"Final positions: {marginResult.Runs.Single().FinalSnapshot.GetPositions().ToArray().Length}");

static Instant Utc(int year, int month, int day, int hour, int minute, int second)
    => Instant.FromDateTimeOffset(new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero));

static BarClosed CreateBarClosed(Instrument instrument, decimal close, Instant time)
{
    var bar = new Bar(
        new Price(close, Currency.USD),
        new Price(close + 1m, Currency.USD),
        new Price(close - 1m, Currency.USD),
        new Price(close, Currency.USD),
        new Qty(10_000m),
        time,
        Duration.FromMinutes(1));

    return new BarClosed(instrument, bar) { Time = time };
}

public sealed partial class SellSeededPositionStrategy : Strategy
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
        bar.Sell(new Qty(1m), Execution.Market());
    }
}

public sealed partial class LeveragedBuyStrategy : Strategy
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
        bar.Buy(new Qty(10m), Execution.Market());
    }
}
