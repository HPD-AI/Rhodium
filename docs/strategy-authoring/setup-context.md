# Setup context

`SetupContext` is the cold-path setup surface passed to `OnInitialize`. It is a `readonly ref struct`, so use it only inside the override and keep the values you need later.

```csharp
public sealed partial class SetupStrategy : Strategy
{
    private AssetId _spy;

    protected override void OnInitialize(in SetupContext setup)
    {
        _spy = setup.AddEquity("SPY");
        setup.ScheduleEvery("rebalance", Duration.FromMinutes(5));
    }
}
```

Rhodium calls user `OnInitialize`, captures registered assets and schedules, then runs generated initialization for tensor fields, portfolio fields, indicators, and window state. Hot hooks run after this setup phase.

## Values available

`SetupContext` exposes the current setup universe and tensor basis:

```csharp
var size = setup.UniverseSize;
var basis = setup.Basis;
```

Use these for setup decisions only. Do not store `SetupContext`; store stable results such as returned `AssetId` values.

## Register instruments

Use setup helpers to add instruments:

```csharp
private AssetId _spy;
private AssetId _spyAlt;

protected override void OnInitialize(in SetupContext setup)
{
    _spy = setup.AddEquity("SPY");
    _spyAlt = setup.AddEquity("SPY", variantOffset: 1);

    var instrument = new Instrument(new Asset("QQQ", AssetClass.Equity), Venue.NASDAQ);
    var qqq = setup.AddInstrument(instrument);

    var btc = setup.AddInstrument(Contracts.CryptoSpot(
        "BTCUSD",
        Venue.Coinbase,
        Currency.BTC,
        Currency.USD,
        tick: 0.01m,
        lot: 0.00000001m));

    var contract = Contracts.Equity("MSFT", Venue.NASDAQ, Currency.USD);
    var msft = setup.AddInstrument(contract);
}
```

When a hot hook needs to know which asset is firing, compare against the stored id:

```csharp
partial void OnBar(ref BarContext bar)
{
    if (bar.AssetId != _spy)
        return;

    bar.Buy(new Qty(1m), Execution.Market());
}
```

Calling instrument registration outside `OnInitialize` throws `InvalidOperationException` with the message `Instruments can only be added during OnInitialize.`

## Register schedules

Schedules are declared during setup:

```csharp
protected override void OnInitialize(in SetupContext setup)
{
    _spy = setup.AddEquity("SPY");

    setup.ScheduleAt("open-check", openInstant);
    setup.ScheduleEvery(
        "rebalance",
        Duration.FromMinutes(5),
        startAt: openInstant,
        stopAt: closeInstant);
}
```

`ScheduleAt` registers a one-shot timer. `ScheduleEvery` registers a recurring timer. Schedule names must be non-empty, and recurring intervals must be positive.

In simulation, registered schedules bind to the session clock after strategy initialization. Timer events are queued as scheduled lifecycle events and delivered to `OnScheduled(ref TimerContext timer)` during session processing.

```csharp
protected override void OnScheduled(ref TimerContext timer)
{
    if (timer.Name != "rebalance")
        return;

    var qty = timer.GetPositionQty(_spy);
    if (qty == 0m)
        timer.Buy(_spy, new Qty(1m), Execution.Market());
}
```

Public scheduling is setup-only. Market hooks, order hooks, fill hooks, timer hooks, and group hooks should react to schedules that were already registered, not create new schedules.

## Setup-time generated registration

Generated initialization registers indicator fields and strategy portfolio fields after `OnInitialize` returns. The generated registration methods are compiler-facing implementation details. Manually calling `__GeneratedRegisterIndicator` or `__GeneratedRegisterPortfolioField` is rejected by analyzer diagnostics and runtime guards restrict registration to initialization.

Cookbook examples:

```text
cookbook/GettingStarted/02-first-strategy.cs
cookbook/GettingStarted/03-bar-indicator.cs
cookbook/StrategyAuthoring/scheduled-timers.cs
```
