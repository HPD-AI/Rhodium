# Market Events

Market events are typed wrappers around market values. They preserve the domain
shape that strategies and replay ordering care about.

```csharp
var trade = new Trade(
    new Price(100.50m, Currency.USD),
    new Qty(25m),
    Side.Buy,
    DualTimestamp.Synchronized(Instant.FromUnixSeconds(1)));

var evt = new TradeOccurred(instrument, trade)
{
    Time = trade.Time.ExchangeTime
};
```

## Event Types

Market events include:

- `QuoteReceived`
- `TradeOccurred`
- `BarClosed`
- `BookSnapshotReceived`
- `BookLevelDeltaReceived`
- `BookLevelDeltasReceived`
- `BookOrderAdded`, `BookOrderModified`, `BookOrderDeleted`, `BookOrderExecuted`
- `BookDepthSnapshotReceived` and `BookDepth10Received`

The primitive value is separate from the event. For example, `Quote` is market
data; `QuoteReceived` is the replayable finance event that carries an
`Instrument` and that quote.

## Channels

`FinanceEvent` inherits from the HPD event base. Its `Time` property bridges
`ExchangeTimestampNs` and the base event `Timestamp`. `Sequence` is optional
ordering metadata assigned by coordination infrastructure, not by ordinary event
producers.

Finance events route by domain:

- Market data and diagnostics use the streaming channel.
- Execution and lifecycle events use the synchronous channel.
- Control events use the control channel.

## Replay Ordering

Same-timestamp finance replay ordering is deterministic. The default priority is:

1. lifecycle and status
2. book updates
3. quotes
4. trades
5. bars
6. execution
7. control
8. diagnostics
9. unknown events

The replay key also preserves source priority, source ordinal, source sequence,
and event sequence number.

```csharp
var replay = ReplayTimeline<FinanceEvent>.Create()
    .AddSource("bars", [barEvent])
    .AddSource("trades", [tradeEvent])
    .AddSource("quotes", [quoteEvent])
    .WithOrdering(FinanceReplayOrderingPolicy.Default);
```

Use separate typed events instead of collapsing quotes, trades, bars, and books
into a generic tick event. Strategy hooks, replay behavior, and diagnostics all
benefit from the distinction.
