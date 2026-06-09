# Generated fields

Generated fields are strategy properties that Rhodium turns into per-asset tensor fields and context accessors.

Declare them as `partial` properties on a `partial Strategy` class:

```csharp
public sealed partial class SignalStrategy : Strategy
{
    [BarField(ReadOnly = false)]
    public partial double Signal { get; set; }

    partial void OnBar(ref BarContext bar)
    {
        bar.Signal = 1.0;
    }
}
```

Do not read or write the generated property through the strategy instance. The generated property body throws because it is context-only. Use the hook context instead: `bar.Signal`, `tick.SpreadTicks`, `quote.MyField`, or `trade.MyField`.

## Field attributes

Use one field attribute per event frequency:

```csharp
[BarField(Name = "Close", ReadOnly = true)]
[TickField(Name = "SpreadTicks", ReadOnly = true)]
[QuoteField(Name = "QuoteScore", ReadOnly = false)]
[TradeField(Name = "TradeScore", ReadOnly = false)]
[BookField(Name = "BookScore", ReadOnly = false)]
```

`Name` controls the backing field name. If omitted, Rhodium uses the property name.

`ReadOnly` controls whether generated context setters exist:

- `ReadOnly = true`: context exposes a getter only.
- `ReadOnly = false`: context exposes a getter and setter.

## Context accessors

Inside the matching hook, read or write the current asset directly:

```csharp
partial void OnBar(ref BarContext bar)
{
    bar.Signal = bar.PositionQuantity == 0m ? 1.0 : 0.0;
}
```

For cross-asset reads, use the generated `For` accessor:

```csharp
if (bar.SignalFor(_spy) > bar.SignalFor(_qqq))
    bar.Buy(_spy, new Qty(1m));
```

Writable fields also get cross-asset setters:

```csharp
bar.SetSignalFor(_spy, 1.0);
```

## Rules that compile-time diagnostics enforce

- The containing strategy type must be `partial`.
- The generated property must be `partial`.
- Generated fields must be declared on a type deriving from `Strategy`.
- Indicator-backed generated fields must be read-only and must use the matching field frequency.

See [Generator diagnostics](../reference/generator-diagnostics.md) for error codes.

Example to copy:

```text
cookbook/StrategyAuthoring/generated-fields.cs
```
