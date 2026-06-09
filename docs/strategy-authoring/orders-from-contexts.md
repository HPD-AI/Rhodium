# Orders from contexts

Generated market contexts and execution contexts expose order helpers. The helpers create order intents in the strategy portfolio context; the runtime or simulation later drains and processes those intents.

```csharp
partial void OnBar(ref BarContext bar)
{
    if (bar.PositionQuantity == 0m)
        bar.Buy(new Qty(1m), Execution.Market());
}
```

Do not treat context helpers as direct broker or exchange submissions. They are strategy intents.

## Market context helpers

Generated market contexts include helpers for the current context asset and overloads that take an explicit `AssetId`:

```csharp
bar.Buy(new Qty(1m), Execution.Market());
bar.Sell(new Qty(1m), Execution.Limit().AtAsk());
bar.Flatten();
bar.TargetQuantity(new Qty(10m));

bar.Buy(_spy, new Qty(1m), Execution.Market());
bar.Cancel(_spy, orderId, "stale");
bar.Modify(_spy, orderId, newLimitPrice: new Price(101m, Currency.USD));
```

`TargetQuantity` uses market delta intents internally; use `Buy` or `Sell` when a specific `ExecutionSpec` is needed.

Contexts also carry `AssetId`, `StrategyId`, `PositionQuantity`, generated field values, indicator readiness flags, and cross-asset generated field accessors.

## Execution specs

Use `Execution` builders to describe how the intent should execute:

```csharp
bar.Buy(new Qty(1m), Execution.Market());
bar.Buy(new Qty(1m), Execution.Limit().AtBid().WithPostOnly());
bar.Sell(new Qty(1m), Execution.Limit().At(new Price(101m, Currency.USD)));
bar.Sell(new Qty(1m), Execution.Limit().AtMid().GoodTilCancelled());
bar.Sell(new Qty(1m), Execution.Limit().AtAsk().ImmediateOrCancel());

bar.Buy(new Qty(100m), Execution.Twap().Over(Duration.FromMinutes(10)));
bar.Buy(new Qty(100m), Execution.Vwap());
bar.Buy(new Qty(100m), Execution.Pov(0.10m));
```

The `ExecutionSpec` is data on the order intent. The active runtime or simulation matching model decides how that intent turns into accepted, modified, rejected, cancelled, expired, and filled events.

## Order lifecycle hooks

Order and fill hooks are virtual overrides:

```csharp
protected override void OnOrderAccepted(ref OrderContext order) { }
protected override void OnOrderModified(ref OrderContext order) { }
protected override void OnOrderRejected(ref OrderContext order) { }
protected override void OnOrderCancelled(ref OrderContext order) { }
protected override void OnOrderExpired(ref OrderContext order) { }
protected override void OnOrderFilled(ref FillContext fill) { }
```

`OrderContext` supports cancel and modify:

```csharp
protected override void OnOrderAccepted(ref OrderContext order)
{
    if (order.AssetId != _spy)
        return;

    order.Modify(newLimitPrice: new Price(101m, Currency.USD));
}
```

If an `OrderContext` has no `AssetId`, use the explicit-asset overloads:

```csharp
order.Cancel(_spy, "manual asset id");
order.Modify(_spy, newQuantity: new Qty(2m));
```

`FillContext` supports follow-up order intents:

```csharp
protected override void OnOrderFilled(ref FillContext fill)
{
    if (fill.AssetId == _spy && fill.Side == Side.Buy)
        fill.Sell(fill.FilledQty, Execution.Market());
}
```

`FillContext` does not expose proposal-only APIs such as `EmitMetric`.

## Timer order helpers

`OnScheduled(ref TimerContext timer)` has no current market asset, so timer helpers require an `AssetId`:

```csharp
protected override void OnScheduled(ref TimerContext timer)
{
    if (timer.Name != "rebalance")
        return;

    if (timer.GetPositionQty(_spy) == 0m)
        timer.Buy(_spy, new Qty(1m), Execution.Market());
    else
        timer.Flatten(_spy);
}
```

`TimerContext` exposes `Name`, `Time`, optional `StrategyId`, `GetPositionQty`, `Buy`, `Sell`, `Cancel`, `Modify`, and `Flatten`.

## Book hooks

Book hooks are generated partial methods:

```csharp
partial void OnBookSnapshot(ref BookSnapshotContext book);
partial void OnBookLevelDelta(ref BookLevelDeltaContext book);
partial void OnBookLevelDeltas(ref BookLevelDeltasContext book);
```

They compile even if the strategy has no generated book fields.

`OnBookSnapshot` exposes the raw event, book payload, best bid and ask, nullable bid/ask/mid/spread values, top-level imbalance, generated book fields, cross-asset accessors, and the standard generated order helpers:

```csharp
partial void OnBookSnapshot(ref BookSnapshotContext book)
{
    if (book.AssetId != _spy || book.BestBid is null || book.BestAsk is null)
        return;

    if (book.TopLevelImbalance > 0.65)
        book.Buy(new Qty(1m), Execution.Limit().AtBid().WithPostOnly());
}
```

`OnBookLevelDelta` exposes `Event`, `Delta`, `Side`, `Price`, `Size`, `Action`, `VenueSequence`, and order helpers:

```csharp
partial void OnBookLevelDelta(ref BookLevelDeltaContext book)
{
    if (book.AssetId == _spy && book.Size.Value == 0m)
        book.Flatten();
}
```

`OnBookLevelDeltas` exposes `Event`, `Deltas`, `Count`, and order helpers:

```csharp
partial void OnBookLevelDeltas(ref BookLevelDeltasContext book)
{
    if (book.AssetId == _spy && book.Count > 10)
        book.Cancel(_spy, _workingOrderId, "book changed");
}
```

Cookbook examples:

```text
cookbook/GettingStarted/04-order-intents.cs
cookbook/StrategyAuthoring/tick-quote-trade-hooks.cs
cookbook/StrategyAuthoring/book-hooks.cs
```
