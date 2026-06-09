#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0

// This sample handles generated order-book hooks and buffers one order intent.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Primitives;
using Rhodium.Simulation;

BookHookStrategy.Reset();

var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
var history = SharedHistory.Load([
    CreateBookSnapshot(instrument),
    CreateBookLevelDelta(instrument),
    CreateBookLevelDeltas(instrument)
]);

var result = Rhodium.Simulation.Rhodium
    .Simulate<BookHookStrategy>()
    .WithHistory(history)
    .Run();

Console.WriteLine($"Snapshots: {BookHookStrategy.SnapshotCount}");
Console.WriteLine($"Level deltas: {BookHookStrategy.LevelDeltaCount}");
Console.WriteLine($"Delta batches: {BookHookStrategy.LevelDeltaBatchCount}");
Console.WriteLine($"Best bid: {BookHookStrategy.LastBestBid}");
Console.WriteLine($"Imbalance: {BookHookStrategy.LastImbalance:N2}");
Console.WriteLine($"Last action: {BookHookStrategy.LastAction}");
Console.WriteLine($"Last batch count: {BookHookStrategy.LastBatchCount}");
Console.WriteLine($"Order intents: {result.OrderIntents.Count}");

static BookSnapshotReceived CreateBookSnapshot(Instrument instrument)
{
    var book = new Book
    {
        Instrument = instrument,
        Time = Instant.FromUnixSeconds(1),
        Bids =
        [
            new Level(new Price(100.00m, Currency.USD), new Qty(700m), 4),
            new Level(new Price(99.95m, Currency.USD), new Qty(300m), 2)
        ],
        Asks =
        [
            new Level(new Price(100.05m, Currency.USD), new Qty(300m), 3),
            new Level(new Price(100.10m, Currency.USD), new Qty(600m), 5)
        ]
    };

    return new BookSnapshotReceived(instrument, book);
}

static BookLevelDeltaReceived CreateBookLevelDelta(Instrument instrument) =>
    new(
        instrument,
        new BookLevelDelta(
            Side.Buy,
            new Price(100.00m, Currency.USD),
            new Qty(750m),
            BookAction.Update,
            VenueSequence: 2));

static BookLevelDeltasReceived CreateBookLevelDeltas(Instrument instrument) =>
    new(
        instrument,
        [
            new BookLevelDelta(
                Side.Sell,
                new Price(100.05m, Currency.USD),
                new Qty(250m),
                BookAction.Update,
                VenueSequence: 3),
            new BookLevelDelta(
                Side.Buy,
                new Price(99.95m, Currency.USD),
                Qty.Zero,
                BookAction.Delete,
                VenueSequence: 4)
        ]);

public sealed partial class BookHookStrategy : Strategy
{
    private AssetId _spy;
    private bool _submitted;

    public static int SnapshotCount { get; private set; }
    public static int LevelDeltaCount { get; private set; }
    public static int LevelDeltaBatchCount { get; private set; }
    public static Price LastBestBid { get; private set; }
    public static decimal LastImbalance { get; private set; }
    public static BookAction LastAction { get; private set; }
    public static int LastBatchCount { get; private set; }

    public static void Reset()
    {
        SnapshotCount = 0;
        LevelDeltaCount = 0;
        LevelDeltaBatchCount = 0;
        LastBestBid = Price.Zero;
        LastImbalance = 0m;
        LastAction = default;
        LastBatchCount = 0;
    }

    protected override void OnInitialize(in SetupContext setup)
    {
        _spy = setup.AddEquity("SPY");
    }

    partial void OnBookSnapshot(ref BookSnapshotContext book)
    {
        if (book.AssetId != _spy)
            return;

        SnapshotCount++;
        LastBestBid = book.BestBid?.Price ?? Price.Zero;
        LastImbalance = book.TopLevelImbalance;

        if (!_submitted && book.TopLevelImbalance > 0m)
        {
            _submitted = true;
            book.Buy(new Qty(1m), Execution.Limit().AtBid());
        }
    }

    partial void OnBookLevelDelta(ref BookLevelDeltaContext book)
    {
        if (book.AssetId != _spy)
            return;

        LevelDeltaCount++;
        LastAction = book.Action;
    }

    partial void OnBookLevelDeltas(ref BookLevelDeltasContext book)
    {
        if (book.AssetId != _spy)
            return;

        LevelDeltaBatchCount++;
        LastBatchCount = book.Count;
    }
}
