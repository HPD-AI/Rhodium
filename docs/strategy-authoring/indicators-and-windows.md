# Indicators and windows

Indicator-backed generated fields let a strategy declare streaming indicators beside the fields that consume them.

```csharp
[BarField(Name = "RSI_2", ReadOnly = true)]
[BarIndicator(typeof(RSI), 2)]
public partial double Rsi { get; }
```

Read the indicator value from the hook context:

```csharp
partial void OnBar(ref BarContext bar)
{
    if (!bar.RsiIsReady) return;
    if (bar.Rsi < 30) bar.Buy(new Qty(1m));
}
```

## Indicator rules

- A `[BarIndicator]` property must also have `[BarField(ReadOnly = true)]`.
- A `[TickIndicator]` property must also have `[TickField(ReadOnly = true)]`.
- Single-output indicators use `[BarIndicator]` or `[TickIndicator]`.
- Multi-output bar indicators use `[BarIndicatorGroup]`.
- Indicator arguments are passed to the indicator constructor.
- Indicator arguments can bind to `[Param]` properties using `Param`, `Param0`, ... `Param7`.

Bar indicator source defaults to close price. You can choose another source:

```csharp
[BarIndicator(typeof(ATR), 14, Source = BarSource.Bar)]
```

Tick indicator source defaults to book data.

## Windows

`[Window]` creates rolling access over a read-only bar `double` field:

```csharp
[BarField(ReadOnly = true)]
[Window(3, 10)]
public partial double Close { get; }
```

Inside `OnBar`, the generated property is a `WindowedDouble`. It can be used as a `double`, and it exposes `Window(length)` for rolling statistics.

```csharp
partial void OnBar(ref BarContext bar)
{
    var window = bar.Close.Window(3);
    var mean = window.Mean();
    var stdDev = window.StdDev();
}
```

Window lengths must be positive. `[Window]` is only supported on read-only bar `double` fields.

Examples to copy:

```text
cookbook/GettingStarted/03-bar-indicator.cs
cookbook/StrategyAuthoring/windowed-fields.cs
```
