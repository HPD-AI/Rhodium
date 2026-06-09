# Prices, Quantities, And Money

Rhodium starts with small typed value objects. They make strategy code explicit
without pretending to be a full risk or accounting engine.

```csharp
var qty = new Qty(10m);
var px = new Price(191.23m, Currency.USD);
var notional = Money.USD(qty.Value * px.Value);
```

`Qty` is a decimal quantity wrapper. It supports arithmetic, comparisons, sign
helpers (`IsZero`, `IsPositive`, `IsNegative`, `Abs`, `Negate`), and implicit
conversion from `decimal` and `int`.

`Price` is a decimal price with an optional `Currency`. It supports arithmetic
and comparisons. Operators keep the left-hand currency; do not rely on `Price`
or `Money` operators to enforce currency compatibility.

`Money` is an amount plus a `Currency`. It supports addition, subtraction,
scaling, negation, and sign helpers. Use it for cash, commissions, realized P&L,
account seeds, and result inspection.

## Ticks

`TickPrice` is the integer tick representation used by book and HFT-facing code.
Convert through a known tick size, or through a contract grid.

```csharp
var contract = Contracts.Equity("AAPL", Venue.NASDAQ, Currency.USD, tick: 0.01m, lot: 1m);

TickPrice tick = contract.Grid.ToTick(new Price(191.23m, Currency.USD));
Price roundTrip = contract.Grid.FromTick(tick, Currency.USD);
```

For low-level code that already has a tick size:

```csharp
var tick = TickPrice.FromPrice(new Price(100.05m, Currency.USD), tickSize: 0.01m);
var price = tick.ToPrice(Currency.USD);
```

## Time

Market time is nanosecond-based.

- `Instant` stores nanoseconds since the Unix epoch.
- `Duration` stores a nanosecond interval.
- `DualTimestamp` carries exchange time and local receipt time.

`DualTimestamp.FeedLatency` is `LocalTime - ExchangeTime`. Quotes expose the
same value as `Staleness`.

```csharp
var exchangeTime = Instant.FromUnixSeconds(1);
var localTime = exchangeTime + Duration.FromMillis(2);
var timestamp = new DualTimestamp(exchangeTime, localTime);

Duration feedLatency = timestamp.FeedLatency;
```

Use `Instant` and `Duration` when building bars, event timestamps, GTD orders,
latency settings, TWAP/VWAP horizons, and replay inputs.
