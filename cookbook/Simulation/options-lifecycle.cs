#:package Rhodium.Simulation@0.5.0
#:property TargetFramework=net10.0
// This sample shows supported option lifecycle simulation: fill, expiry, cash settlement, and blocked expiry.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

RuntimeOptionExpiryStrategy.Reset();
BlockedOptionExpiryStrategy.Reset();

var settledResult = Rhodium.Simulation.Rhodium
    .Simulate<RuntimeOptionExpiryStrategy>()
    .WithInitialCash(Money.USD(100_000m))
    .WithHistory(SharedHistory.Load([
        CreateBarClosed(RuntimeOptionExpiryStrategy.Option.Instrument, 2m, Instant.FromUnixSeconds(1)),
        CreateBarClosed(RuntimeOptionExpiryStrategy.Underlying, 105m, RuntimeOptionExpiryStrategy.Expiry)
    ]))
    .Run();

var blockedResult = Rhodium.Simulation.Rhodium
    .Simulate<BlockedOptionExpiryStrategy>()
    .WithInitialCash(Money.USD(100_000m))
    .WithHistory(SharedHistory.Load([
        CreateBarClosed(BlockedOptionExpiryStrategy.Option.Instrument, 2m, Instant.FromUnixSeconds(1)),
        CreateBarClosed(BlockedOptionExpiryStrategy.Option.Instrument, 2m, BlockedOptionExpiryStrategy.Expiry)
    ]))
    .Run();

var settlement = settledResult.SimulatorEvents
    .OfType<OptionLifecycleApplied>()
    .Single(static evt => evt.LifecycleKind == OptionLifecycleKind.CashSettlement);
var blocked = blockedResult.SimulatorEvents
    .OfType<OptionLifecycleApplied>()
    .Single(static evt => evt.LifecycleKind == OptionLifecycleKind.Blocked);
var finalStatement = settledResult.AccountStatements.Last();

Console.WriteLine($"Option fills: {settledResult.ExecutionEvents.OfType<OrderFilled>().Count()}");
Console.WriteLine($"Settlement kind: {settlement.LifecycleKind}");
Console.WriteLine($"Settlement cash flow: {settlement.CashFlow}");
Console.WriteLine($"Settlement reference: {settlement.ReferenceSource}");
Console.WriteLine($"Final cash: {finalStatement.Cash}");
Console.WriteLine($"Realized PnL: {finalStatement.RealizedPnL}");
Console.WriteLine($"Blocked kind: {blocked.LifecycleKind}");
Console.WriteLine($"Blocked reference: {blocked.ReferenceSource}");
Console.WriteLine($"Blocked open positions: {blockedResult.Runs.Single().FinalSnapshot.GetPositions().ToArray().Length}");

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

public sealed partial class RuntimeOptionExpiryStrategy : Strategy
{
    public static readonly Instant Expiry = Instant.FromUnixSeconds(1_796_016_000);
    public static readonly Instrument Underlying = new(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
    public static readonly InstrumentContract UnderlyingContract = Contracts.Equity("SPY", Venue.NASDAQ, Currency.USD);
    public static readonly InstrumentContract Option = Contracts.OptionContract(
        "SPY261218C00100000",
        new Venue("CBOE"),
        Underlying,
        Currency.USD,
        tick: 0.01m,
        lot: 1m,
        multiplier: 100m,
        strike: new Price(100m, Currency.USD),
        Expiry,
        OptionRight.Call,
        ExerciseStyle.European,
        exercisePolicy: OptionExercisePolicy.CashSettledAtExpiry);

    private AssetId _option;
    private bool _submitted;

    [BarField(ReadOnly = true)]
    public partial double Close { get; }

    public static void Reset()
    {
    }

    protected override void OnInitialize(in SetupContext setup)
    {
        setup.AddInstrument(UnderlyingContract);
        _option = setup.AddInstrument(Option);
    }

    partial void OnBar(ref BarContext bar)
    {
        if (_submitted || bar.AssetId != _option || bar.Close != 2.0)
            return;

        _submitted = true;
        bar.Buy(new Qty(1m), Execution.Market());
    }
}

public sealed partial class BlockedOptionExpiryStrategy : Strategy
{
    public static readonly Instant Expiry = RuntimeOptionExpiryStrategy.Expiry;
    public static readonly Instrument Underlying = new(new Asset("QQQ", AssetClass.Equity), Venue.NASDAQ);
    public static readonly InstrumentContract Option = Contracts.OptionContract(
        "QQQ261218C00100000",
        new Venue("CBOE"),
        Underlying,
        Currency.USD,
        tick: 0.01m,
        lot: 1m,
        multiplier: 100m,
        strike: new Price(100m, Currency.USD),
        Expiry,
        OptionRight.Call,
        ExerciseStyle.European,
        exercisePolicy: OptionExercisePolicy.CashSettledAtExpiry);

    private AssetId _option;
    private bool _submitted;

    [BarField(ReadOnly = true)]
    public partial double Close { get; }

    public static void Reset()
    {
    }

    protected override void OnInitialize(in SetupContext setup)
    {
        _option = setup.AddInstrument(Option);
    }

    partial void OnBar(ref BarContext bar)
    {
        if (_submitted || bar.AssetId != _option || bar.Close != 2.0)
            return;

        _submitted = true;
        bar.Buy(new Qty(1m), Execution.Market());
    }
}
