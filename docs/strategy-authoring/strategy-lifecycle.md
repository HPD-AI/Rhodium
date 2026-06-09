# Strategy lifecycle

Lifecycle, timer, order, fill, position, and group hooks are `Strategy` virtual overrides. They are not generated partial hooks.

```csharp
protected override void OnStart(ref LifecycleContext lifecycle) { }
protected override void OnStop(ref LifecycleContext lifecycle) { }
protected override void OnScheduled(ref TimerContext timer) { }

protected override void OnOrderAccepted(ref OrderContext order) { }
protected override void OnOrderModified(ref OrderContext order) { }
protected override void OnOrderRejected(ref OrderContext order) { }
protected override void OnOrderCancelled(ref OrderContext order) { }
protected override void OnOrderExpired(ref OrderContext order) { }
protected override void OnOrderFilled(ref FillContext fill) { }

protected override void OnPositionOpened(ref PositionContext position) { }
protected override void OnPositionChanged(ref PositionContext position) { }
protected override void OnPositionClosed(ref PositionContext position) { }
protected override void OnGroup(ref GroupContext group) { }
public override void OnError(Exception ex) { }
```

## Run order

Initialization runs in this order:

1. `OnInitialize(in SetupContext setup)`.
2. Generated initialization for tensor fields, portfolio fields, indicators, and windows.
3. Runtime snapshot capacity checks.

Market dispatch updates generated state before user market hooks:

- Tick path updates tick indicators, then calls `OnTick`.
- Bar path updates bar indicators, multi-output indicator groups, and windows, then calls `OnBar`.
- Quote, trade, and book paths create event contexts and call generated hooks for registered assets in the event asset range.
- Group hooks run after generated tick or bar logic only when the portfolio context has child strategy ids.

## Start, stop, and scheduled

`OnStart` receives `SessionStarted`. `OnStop` receives `SessionEnded`. `OnScheduled` receives scheduled lifecycle events created from schedules declared during setup.

```csharp
private AssetId _spy;

protected override void OnInitialize(in SetupContext setup)
{
    _spy = setup.AddEquity("SPY");
    setup.ScheduleEvery("rebalance", Duration.FromMinutes(5));
}

protected override void OnScheduled(ref TimerContext timer)
{
    if (timer.Name != "rebalance")
        return;

    timer.Flatten(_spy);
}
```

`TimerContext` exposes `Name`, `Time`, optional `StrategyId`, `GetPositionQty`, and explicit-asset `Buy`, `Sell`, `Cancel`, `Modify`, and `Flatten`. Timer order helpers require an `AssetId`; there is no current market-event asset in a timer hook.

## Order and fill hooks

Order lifecycle hooks receive `OrderContext`. It includes `StrategyId`, `OrderId`, `Status`, `VariantId`, optional `AssetId`, optional `Reason`, and cancel/modify helpers.

```csharp
protected override void OnOrderAccepted(ref OrderContext order)
{
    if (order.AssetId != _spy)
        return;

    order.Modify(newLimitPrice: new Price(101m, Currency.USD));
}
```

`FillContext` includes `StrategyId`, `OrderId`, `AssetId`, `Side`, `FilledQty`, `FillPrice`, `Commission`, `Position`, and follow-up `Buy`, `Sell`, `Cancel`, and `Modify` helpers.

```csharp
protected override void OnOrderFilled(ref FillContext fill)
{
    if (fill.AssetId == _spy && fill.Side == Side.Buy)
        fill.Sell(fill.FilledQty, Execution.Market());
}
```

These helpers emit order intents into the portfolio context for runtime or simulation processing. They do not submit directly to a broker or exchange.

## Position hooks

Position hooks run after execution processing when the position transition kind is opened, changed, or closed:

```csharp
protected override void OnPositionOpened(ref PositionContext position) { }
protected override void OnPositionChanged(ref PositionContext position) { }
protected override void OnPositionClosed(ref PositionContext position) { }
```

Use them for state updates that depend on committed position transitions rather than raw order events.

## Group hooks

`OnGroup(ref GroupContext group)` is for parent or meta strategies with child strategy ids. It returns without calling user code for leaf strategies.

```csharp
protected override void OnGroup(ref GroupContext group)
{
    group.AllocateEqual();

    for (var i = 0; i < group.ChildIds.Length; i++)
    {
        var child = group.Child(i);
        if (child.GrossExposure > 1_000_000m)
            group.Pause(child.StrategyId);
    }
}
```

`GroupContext` exposes `StrategyId`, `ParentId`, `ChildIds`, `Children`, allocation weight, max capital, pause state, child lookup, and helpers such as `SetAllocation`, `SetMaxCapital`, `Pause`, `Resume`, `Apply`, `AllocateEqual`, `CapGrossExposure`, and `AllocateInverseVolatility`.

Group commands are buffered and applied by the engine after the current phase. Buffers are bounded; overflow throws.

## Error hook

`OnError(Exception ex)` is called by dispatch loops for ordinary exceptions thrown from strategy hooks. Use it for logging, telemetry, or marking local state.

`OnError` is not a retry policy and does not swallow strategy execution invariant failures. `StrategyExecutionInvariantException` subclasses, including hot-path allocation and universe-topology failures, propagate rather than being routed through `OnError`.

Cookbook examples:

```text
cookbook/GettingStarted/04-order-intents.cs
cookbook/StrategyAuthoring/scheduled-timers.cs
```
