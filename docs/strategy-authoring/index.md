# Strategy authoring

A Rhodium strategy is a `partial` class deriving from `Rhodium.Platform.Strategy`. User code is split into a cold setup override and hot event hooks.

```csharp
using Rhodium.Indicators.Streaming;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;

public sealed partial class RsiStrategy : Strategy
{
    private AssetId _spy;

    [BarField(Name = "RSI_14", ReadOnly = true)]
    [BarIndicator(typeof(RSI), 14)]
    public partial double Rsi { get; }

    protected override void OnInitialize(in SetupContext setup)
    {
        _spy = setup.AddEquity("SPY");
    }

    partial void OnBar(ref BarContext bar)
    {
        if (bar.AssetId != _spy || !bar.RsiIsReady)
            return;

        if (bar.Rsi < 30)
            bar.Buy(new Qty(1m), Execution.Market());
    }
}
```

Generated strategy fields are context-only. Read `bar.Rsi`, `tick.SpreadTicks`, `quote.Close`, `book.TopLevelImbalance`, or a generated explicit accessor such as `GetRsi(id, ref portfolio)`. Do not read generated field properties from the strategy instance.

## Cold path

`OnInitialize(in SetupContext setup)` runs before generated initialization. Use it to register instruments and schedules:

```csharp
protected override void OnInitialize(in SetupContext setup)
{
    _spy = setup.AddEquity("SPY");
    setup.ScheduleEvery("rebalance", Duration.FromMinutes(5));
}
```

`SetupContext` is stack-only setup state. Keep returned `AssetId` values in fields if hot hooks need to filter assets later. Instrument and schedule registration are public setup-time operations; do not add schedules dynamically from market, order, fill, timer, or group hooks.

## Hot hooks

Generated market-data hooks are partial methods:

```csharp
partial void OnBar(ref BarContext bar);
partial void OnTick(ref TickContext tick);
partial void OnQuote(ref QuoteContext quote);
partial void OnTrade(ref TradeContext trade);
partial void OnBookSnapshot(ref BookSnapshotContext book);
partial void OnBookLevelDelta(ref BookLevelDeltaContext book);
partial void OnBookLevelDeltas(ref BookLevelDeltasContext book);
```

The signature must be exact: `partial void`, one `ref` parameter, and the matching context type.

Lifecycle, order, fill, position, timer, and group hooks are virtual overrides on `Strategy`, not generated partial methods:

```csharp
protected override void OnStart(ref LifecycleContext lifecycle) { }
protected override void OnStop(ref LifecycleContext lifecycle) { }
protected override void OnScheduled(ref TimerContext timer) { }
protected override void OnOrderFilled(ref FillContext fill) { }
protected override void OnPositionOpened(ref PositionContext position) { }
protected override void OnGroup(ref GroupContext group) { }
```

## Context-first API

Generated contexts carry the current `AssetId`, `StrategyId`, `PositionQuantity`, generated field accessors, indicator readiness flags, cross-asset accessors, and order helpers such as `Buy`, `Sell`, `Cancel`, `Modify`, `Flatten`, and `TargetQuantity`.

Those helpers emit order intents into Rhodium runtime state. In simulation, the runtime drains and processes those intents through the configured matching model; they are not direct broker or exchange submissions from user code.

## Field and parameter rules

- Strategy classes with generated fields or generated hooks must be `partial`.
- Generated properties must be `partial`.
- Indicator-backed fields require a matching read-only generated field attribute.
- `[Window]` applies to read-only bar `double` fields with positive lengths.
- `[Param]` properties must be init-only and use supported scalar or enum types.
- Param-bound indicators may reference `[Param]` names through `Param`, `Param0`, ..., `Param7`.
- Hot hooks should be allocation-free after warmup. Debug builds detect managed allocations on guarded paths and throw `HotPathAllocationException`; release builds should still follow the rule but do not promise that check.

## Next pages

- [Setup context](setup-context.md): instruments, schedules, and setup-only rules.
- [Strategy lifecycle](strategy-lifecycle.md): start, stop, scheduled, order, fill, position, group, and error hooks.
- [Generated fields](generated-fields.md): field attributes and context accessors.
- [Event hooks](event-hooks.md): market hook signatures and event context payloads.
- [Indicators and windows](indicators-and-windows.md): streaming indicators and rolling windows.
- [Orders from contexts](orders-from-contexts.md): order helpers, execution specs, book hooks, and lifecycle order callbacks.
- [Parameters and grids](parameters-and-grids.md): `[Param]`, generated variants, Cartesian grids, and exact rows.
- [Hot-path rules](hot-path-rules.md): allocation discipline and invariant failures.

Cookbook examples to copy:

```text
cookbook/GettingStarted/04-order-intents.cs
cookbook/GettingStarted/05-parameter-grid.cs
cookbook/StrategyAuthoring/generated-fields.cs
cookbook/StrategyAuthoring/tick-quote-trade-hooks.cs
cookbook/StrategyAuthoring/windowed-fields.cs
cookbook/StrategyAuthoring/book-hooks.cs
cookbook/StrategyAuthoring/scheduled-timers.cs
```
