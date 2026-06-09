# Hot-path rules

Market, lifecycle, timer, execution, position, and group hooks run on guarded dispatch paths. Write them as allocation-free code after warmup.

Hot hooks include:

```csharp
partial void OnBar(ref BarContext bar);
partial void OnTick(ref TickContext tick);
partial void OnQuote(ref QuoteContext quote);
partial void OnTrade(ref TradeContext trade);
partial void OnBookSnapshot(ref BookSnapshotContext book);
partial void OnBookLevelDelta(ref BookLevelDeltaContext book);
partial void OnBookLevelDeltas(ref BookLevelDeltasContext book);

protected override void OnScheduled(ref TimerContext timer) { }
protected override void OnOrderFilled(ref FillContext fill) { }
protected override void OnPositionChanged(ref PositionContext position) { }
protected override void OnGroup(ref GroupContext group) { }
```

## What the debug guard does

In debug builds, Rhodium warms a guarded path on its first call. On later calls for that guard, it compares `GC.GetAllocatedBytesForCurrentThread()` before and after dispatch. If bytes increase, Rhodium throws `HotPathAllocationException`.

Execution hooks warm by execution and position-transition path. Lifecycle and timer hooks share lifecycle guarding. Market-data hooks use market guarding.

Do not promise this as a release-build enforcement mechanism. The checks are debug-only, but production strategy code should still follow the allocation-free rule.

## Write hot hooks like this

Prefer fields initialized in `OnInitialize`, value operations, `ref struct` contexts, spans, and primitive comparisons:

```csharp
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
    if (_submitted || bar.AssetId != _spy || bar.Close <= 0d)
        return;

    _submitted = true;
    bar.Buy(new Qty(1m), Execution.Market());
}
```

For grouped strategies, iterate spans directly:

```csharp
protected override void OnGroup(ref GroupContext group)
{
    for (var i = 0; i < group.ChildIds.Length; i++)
    {
        var child = group.Child(i);
        if (child.GrossExposure > 1_000_000m)
            group.Pause(child.StrategyId);
    }
}
```

## Avoid in hot hooks

- `new` reference types.
- LINQ.
- Closures or lambdas that allocate.
- String formatting or interpolation.
- Boxing, including interface calls over value types when they box.
- Growing `List<T>`, `Dictionary<TKey,TValue>`, or other managed collections.
- Logging calls that allocate messages, scopes, or structured payloads.
- Capturing or storing ref-struct contexts.

Allocate and configure reusable state during `OnInitialize` or outside the strategy run. In hot hooks, update fields and emit order or allocation intents through contexts.

## Invariant failures

Some failures are strategy execution invariants. They propagate rather than going through `OnError(Exception ex)`.

Examples:

- `HotPathAllocationException` after debug warmup detects allocations.
- `UniverseTopologyChangedException` when the market universe version differs from the initialized strategy version.
- Bounded command or order-intent buffer overflow.

Use `OnError` for ordinary hook exceptions, logging, and local state marking. Do not rely on it to recover from invariant failures.

## Rules that surface as errors

- Instrument registration outside `OnInitialize` throws `InvalidOperationException`.
- Generated tensor and portfolio field registration is initialization-only.
- Schedule names must be non-empty.
- Recurring schedule intervals must be positive.
- Universe topology changes after initialization require reinitialization.
- Group allocation command buffers are bounded.
- Order-intent buffers are bounded.

Cookbook examples with hot hooks:

```text
cookbook/GettingStarted/04-order-intents.cs
cookbook/StrategyAuthoring/generated-fields.cs
cookbook/StrategyAuthoring/tick-quote-trade-hooks.cs
cookbook/StrategyAuthoring/windowed-fields.cs
```
