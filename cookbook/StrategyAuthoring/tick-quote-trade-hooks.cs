#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0

// This sample implements quote and trade hooks without generated fields.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Primitives;
using Rhodium.Simulation;

QuoteTradeHookStrategy.Reset();

var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
var timestamp = DualTimestamp.Synchronized(default);
var history = SharedHistory.Load([
    new QuoteReceived(instrument, new Quote(
        new Price(100m, Currency.USD),
        new Price(100.05m, Currency.USD),
        new Qty(500m),
        new Qty(600m),
        timestamp)),
    new TradeOccurred(instrument, new Trade(
        new Price(100.02m, Currency.USD),
        new Qty(25m),
        Side.Buy,
        timestamp))
]);

_ = Rhodium.Simulation.Rhodium
    .Simulate<QuoteTradeHookStrategy>()
    .WithHistory(history)
    .Run();

Console.WriteLine($"Quotes: {QuoteTradeHookStrategy.QuoteCount}");
Console.WriteLine($"Trades: {QuoteTradeHookStrategy.TradeCount}");
Console.WriteLine($"Last spread: {QuoteTradeHookStrategy.LastSpread}");
Console.WriteLine($"Last trade size: {QuoteTradeHookStrategy.LastTradeSize}");

public sealed partial class QuoteTradeHookStrategy : Strategy
{
    private AssetId _spy;

    public static int QuoteCount { get; private set; }
    public static int TradeCount { get; private set; }
    public static Price LastSpread { get; private set; }
    public static Qty LastTradeSize { get; private set; }

    public static void Reset()
    {
        QuoteCount = 0;
        TradeCount = 0;
        LastSpread = Price.Zero;
        LastTradeSize = Qty.Zero;
    }

    protected override void OnInitialize(in SetupContext setup)
    {
        _spy = setup.AddEquity("SPY");
    }

    partial void OnQuote(ref QuoteContext quote)
    {
        if (quote.AssetId != _spy)
            return;

        QuoteCount++;
        LastSpread = quote.Spread;
    }

    partial void OnTrade(ref TradeContext trade)
    {
        if (trade.AssetId != _spy)
            return;

        TradeCount++;
        LastTradeSize = trade.Size;
    }
}
