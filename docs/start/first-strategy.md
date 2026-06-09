# First strategy

A Rhodium strategy is a `partial` class deriving from `Strategy`. Generated strategy code is created by the Rhodium source generator, so the file app must be under a directory with the analyzer references described in [Install](install.md).

```csharp
public sealed partial class FirstStrategy : Strategy
{
    private AssetId _spy;

    [BarField(ReadOnly = true)]
    public partial double Close { get; }

    protected override void OnInitialize(in SetupContext setup)
    {
        _spy = setup.AddEquity("SPY");
    }

    partial void OnBar(ref BarContext bar)
    {
        if (bar.AssetId != _spy)
            return;

        bar.Buy(new Qty(1m), Execution.Market());
    }
}
```

## What runs when

`OnInitialize` runs before replay dispatch. Use it to register instruments and schedules. Instrument registration is setup-only; the strategy universe is fixed after initialization.

`OnBar` is a generated partial hook. It runs when a `BarClosed` event reaches a registered asset. The generator creates the concrete context members for your strategy, including generated fields, indicator readiness flags, and generated bar context order helpers.

`[BarField(ReadOnly = true)]` asks the generator to project the bar field onto the hook context. Read it from the hook context, for example `bar.Close`.

## Rules to keep

- Mark the strategy class `partial`.
- Put instrument registration in `OnInitialize`.
- Declare generated hooks as `partial void` with one matching `ref` context parameter.
- Declare generated bar fields as partial properties with `[BarField]`.
- Read generated fields from the context, such as `bar.Close` or `bar.Rsi`.
- Use generated strategy-facing order helpers such as `Buy`, `Sell`, `Cancel`, and `Modify`; they produce `OrderIntent`s, not guaranteed fills.
- Keep hot-path hook bodies allocation-free in debug builds.

Copyable strategy examples:

- `cookbook/GettingStarted/01-first-backtest.cs`
- `cookbook/GettingStarted/02-first-strategy.cs`
- `cookbook/GettingStarted/03-bar-indicator.cs`
- `cookbook/GettingStarted/04-order-intents.cs`
- `cookbook/GettingStarted/05-parameter-grid.cs`

The verified start sequence continues through venue configuration, account seeding, and result export:

- `cookbook/GettingStarted/06-venue-config.cs`
- `cookbook/GettingStarted/07-account-seed.cs`
- `cookbook/GettingStarted/08-export-results.cs`

Continue with [Strategy authoring](../strategy-authoring/index.md) for generated fields, indicators, hooks, parameters, and diagnostics.
