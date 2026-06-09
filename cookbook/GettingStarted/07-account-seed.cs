#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0

// This sample seeds cash and settled custody before replay begins.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);

var cashSeedResult = Rhodium.Simulation.Rhodium
    .Simulate<SeedBuyStrategy>()
    .WithInitialCash(Money.USD(0m))
    .WithAccountSeed(new AccountSeed(Venue.NASDAQ, [Money.USD(200m)], []))
    .WithHistory(SharedHistory.Load([CreateBarClosed(instrument, 123m)]))
    .Run();

var positionSeedResult = Rhodium.Simulation.Rhodium
    .Simulate<SeedSellStrategy>()
    .WithInitialCash(Money.USD(0m))
    .WithAccountSeed(new AccountSeed(
        Venue.NASDAQ,
        [],
        [new SeedPosition(instrument, new Qty(1m), new Price(50m, Currency.USD))]))
    .WithHistory(SharedHistory.Load([CreateBarClosed(instrument, 123m)]))
    .Run();

var cashTransfers = cashSeedResult.SimulatorEvents.OfType<AccountTransferCompleted>().ToArray();
var positionTransfers = positionSeedResult.SimulatorEvents.OfType<AccountTransferCompleted>().ToArray();
var cashFills = cashSeedResult.ExecutionEvents.OfType<OrderFilled>().ToArray();
var positionFills = positionSeedResult.ExecutionEvents.OfType<OrderFilled>().ToArray();
var cashStatement = cashSeedResult.AccountStatements.Last();
var positionCount = positionSeedResult.Runs[0].FinalSnapshot.GetPositions().ToArray().Length;

Console.WriteLine($"Cash seed fills: {cashFills.Length}");
Console.WriteLine($"Cash seed transfers: {cashTransfers.Length}");
Console.WriteLine($"Cash statements: {cashSeedResult.AccountStatements.Count}");
Console.WriteLine($"Cash final statement: {cashStatement.Cash}");
Console.WriteLine($"Cash final venue cash: {cashSeedResult.Diagnostics.Venues.Single().Cash}");
Console.WriteLine($"Position seed fills: {positionFills.Length}");
Console.WriteLine($"Position seed transfers: {positionTransfers.Length}");
Console.WriteLine($"Position final count: {positionCount}");
Console.WriteLine($"Position final venue cash: {positionSeedResult.Diagnostics.Venues.Single().Cash}");

static BarClosed CreateBarClosed(Instrument instrument, decimal close)
{
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

public sealed partial class SeedBuyStrategy : Strategy
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

public sealed partial class SeedSellStrategy : Strategy
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
