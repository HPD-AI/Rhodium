# Bars, Quotes, Trades, And Books

This page covers market-data primitives. Events that carry them are covered in
[Market Events](market-events.md).

## Quotes

`Quote` is a two-sided market with a `DualTimestamp`.

```csharp
var exch = Instant.FromUnixSeconds(1);
var quote = new Quote(
    new Price(100.00m, Currency.USD),
    new Price(100.05m, Currency.USD),
    new Qty(500m),
    new Qty(300m),
    new DualTimestamp(exch, exch + Duration.FromMillis(2)));

var mid = quote.Mid;
var spread = quote.Spread;
var staleness = quote.Staleness;
var bidTick = quote.BidTick(0.01m);
```

Quotes expose `Mid`, `Spread`, `SpreadBps`, `Staleness`, and bid/ask tick
conversion.

## Trades And Ticks

`Trade` records price, size, aggressor side, and a `DualTimestamp`.

```csharp
var trade = new Trade(
    new Price(100.50m, Currency.USD),
    new Qty(25m),
    Side.Buy,
    DualTimestamp.Synchronized(Instant.FromUnixSeconds(1)));

var tradeTick = trade.PriceTick(0.01m);
```

`Tick` is the lower-level tick-by-tick primitive. It carries a price, size,
`TickType`, and `DualTimestamp`, and also exposes tick conversion.

## Bars

`Bar` is OHLCV with an `Instant Time` and `Duration Period`.

```csharp
var bar = Bar.Create(
    new Price(100.50m, Currency.USD),
    new Qty(25m),
    trade.Time.ExchangeTime,
    Duration.FromMinutes(1));

var updated = bar.Update(new Price(100.75m, Currency.USD), new Qty(10m), Instant.FromUnixSeconds(2));
```

Bars expose derived candle values such as `Typical`, `Median`, `Range`, `Body`,
`UpperWick`, `LowerWick`, `IsBullish`, `IsBearish`, `IsDoji`, `Change`, and
`ChangeAbs`.

The base `Bar` type does not promise that `Time` is always the open or close
boundary. Current trade aggregators use exchange timestamps. Time bars align the
period end to a duration grid, emit the previous bar when the next trade reaches
or passes the period end, and use the last included trade as the bar `Time`.
Tick bars and volume bars also aggregate trades with exchange timestamps and use
`Duration.Zero` as their period.

## L2 Books

`Book` is a normalized L2 snapshot with ordered bid and ask levels.

```csharp
var book = new Book
{
    Instrument = instrument,
    Time = Instant.FromUnixSeconds(1),
    Bids = [new Level(new Price(100.00m, Currency.USD), new Qty(500m))],
    Asks = [new Level(new Price(100.05m, Currency.USD), new Qty(300m))]
};

var bid = book.Bid;
var ask = book.Ask;
var imbalance = book.Imbalance(levels: 1);
var vwap = book.VwapToFill(Side.Buy, new Qty(100m));
```

The first bid and ask levels are the best bid and ask. Books expose best
levels, bid/ask prices, mid, spread, side depth, imbalance, and VWAP-to-fill.

`BookLevelDelta` is price-level data: side, price, size, action, venue sequence,
and flags. It is not an L3 individual order.

## Fixed Depth And L3

Fixed-depth events carry top-N bid and ask levels, including `BookDepth10Received`
for a top-10 snapshot.

L3 market-by-order data is separate from L2 level depth. `BookOrderAdded`,
`BookOrderModified`, `BookOrderDeleted`, and `BookOrderExecuted` carry
individual external book order changes. Advanced HFT surfaces such as
`IMarketQuery` expose order, size, queue-depth, and estimated queue-position
queries when a strategy or fill model needs market-by-order detail.

Tick-based depth implementations expose `IHftDepth`: best bid/ask ticks,
quantity-at-tick, ordered level copying, updates, and clearing. Most strategy
authors can stay with contexts and L2/L3 events; use `IHftDepth` when working on
fill, slippage, latency, or book-fidelity internals.
