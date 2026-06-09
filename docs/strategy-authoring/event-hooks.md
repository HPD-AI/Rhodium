# Event hooks

Generated event hooks are `partial void` methods. Rhodium emits the defining declaration and dispatch code when it sees the matching user declaration.

```csharp
partial void OnBar(ref BarContext bar)
{
    if (bar.PositionQuantity == 0m)
        bar.Buy(new Qty(1m));
}
```

## Generated hooks

```csharp
partial void OnBar(ref BarContext bar);
partial void OnTick(ref TickContext tick);
partial void OnQuote(ref QuoteContext quote);
partial void OnTrade(ref TradeContext trade);
partial void OnBookSnapshot(ref BookSnapshotContext book);
partial void OnBookLevelDelta(ref BookLevelDeltaContext book);
partial void OnBookLevelDeltas(ref BookLevelDeltasContext book);
```

The signature matters. The generator recognizes a hook only when it has exactly one `ref` parameter with the matching context type.

## What contexts include

All generated contexts include:

- `AssetId`
- `StrategyId`
- `PositionQuantity`
- order helpers such as `Buy`, `Sell`, `Flatten`, `TargetQuantity`, `Cancel`, and `Modify`
- generated field accessors for that context frequency

Event-specific contexts add event data:

- `QuoteContext`: `Quote`, `Bid`, `Ask`, `BidSize`, `AskSize`, `Mid`, `Spread`, `SpreadTicks`.
- `TradeContext`: `Trade`, `Price`, `Size`, `AggressorSide`, `PriceTick`.
- `BookSnapshotContext`: `Book`, `BestBid`, `BestAsk`, `Mid`, `Spread`, `TopLevelImbalance`.
- `BookLevelDeltaContext`: `Delta`, `Side`, `Price`, `Size`, `Action`, `VenueSequence`.
- `BookLevelDeltasContext`: `Deltas`, `Count`.
- `TickContext`: top-of-book frame values such as `BidTick`, `AskTick`, `BookSpreadTicks`, `MidPrice`, and `MicroPrice`.

## When hooks run

- `OnBar` runs for registered assets when bars are replayed.
- `OnTick` runs over registered assets from current top-of-book state.
- Quote, trade, and book hooks run for registered assets inside the event's asset range.
- Hooks are hot-path code. In debug builds, Rhodium warms guarded paths and then throws `HotPathAllocationException` if managed allocations occur.

Lifecycle and execution hooks are virtual overrides, not generated partial hooks:

```csharp
protected override void OnScheduled(ref TimerContext timer) { }
protected override void OnOrderFilled(ref FillContext fill) { }
protected override void OnPositionOpened(ref PositionContext position) { }
```

Examples to copy:

```text
cookbook/StrategyAuthoring/tick-quote-trade-hooks.cs
cookbook/StrategyAuthoring/book-hooks.cs
```
