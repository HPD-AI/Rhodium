# Indicators

Rhodium indicators live in `Rhodium.Indicators`; streaming implementations live under `Rhodium.Indicators.Streaming`. Strategies can use indicators directly or ask the source generator to allocate per-asset generated indicator fields.

## Interfaces

- `IIndicator<T>`: `Value`, `IsReady`, `Count`, `Reset()`.
- `IPriceIndicator`: updates from `decimal`.
- `IBarIndicator`: updates from `Bar`.
- `ITickIndicator`: updates from `in TickFrame`.
- Base classes: `PriceIndicatorBase`, `BarIndicatorBase`, `TickIndicatorBase`.

Readiness is indicator-specific warmup. Check `IsReady`; do not assume one universal count.

## Factory Categories

Use the `Indicators` static factory for common constructors:

- Basic/statistical: `SMA`, `EMA`, `RMA`, `WMA`, `LWMA`, `StdDev`, `ZScore`, `LinearReg`, `LinearRegSlope`, `Max`, `Min`, `Sum`, `EfficiencyRatio`.
- Advanced averages: `DEMA`, `TEMA`, `HMA`, `KAMA`, `TRIMA`, `ZLEMA`, `VIDYA`.
- Momentum and oscillators: `RSI`, `MACD`, `ROC`, `Momentum`, `TRIX`, `CMO`, `PsychologicalLine`, `PPO`, `DPO`, `Bias`, `VHF`, `AMAT`, `WilliamsR`, `CCI`, `Stochastic`, `UltimateOscillator`.
- Volatility and bands: `BollingerBands`, `ATR`, `VolatilityRatio`, `RVI`, `FuzzyVolatility`, `DonchianChannel`, `KeltnerChannel`, `KeltnerPosition`.
- Volume: `VWAP`, `OBV`, `MFI`, `AD`, `CMF`, `KlingerOscillator`, `Pressure`.
- Direction and price action: `ADX`, `PSAR`, `Aroon`, `AroonOsc`, `SuperTrend`, `Ichimoku`, `SwingHigh`, `SwingLow`, `PivotPoints`.

Tick/depth helpers such as spread, mid-price, and micro-price exist in source/tests but are not all exposed through the `Indicators` factory.

## Generated Indicator Fields

Scalar bar indicators:

```csharp
[BarField(ReadOnly = true)]
[BarIndicator(typeof(RSI), 14)]
public partial double Rsi { get; }
```

Rules:

- `[BarIndicator]` requires `[BarField(ReadOnly = true)]`.
- `[TickIndicator]` requires `[TickField(ReadOnly = true)]`.
- The property and containing strategy type must be `partial`.
- The containing type must derive from `Strategy`.
- Parameter-bound constructor arguments can use `Param`, `Param0` through `Param7`.

Generated indicator fields expose a readiness flag named `PropertyIsReady`, for example `bar.RsiIsReady`.

## Multi-Output Indicators

Some indicator implementations expose more than one useful output. The generator detects `MACD`, `BollingerBands`, `Stochastic`, `Aroon`, `DonchianChannel`, and `KeltnerChannel` as multi-output, so scalar `[BarIndicator]` usage reports `RHD003`.

Generated group view output members are source-backed for:

| Indicator | View outputs |
|:--|:--|
| `MACD` | `Value`, `Signal`, `Histogram` |
| `BollingerBands` | `Upper`, `Middle`, `Lower` |
| `Stochastic` | `K`, `D` |

Do not broadly assume generated view members for every detected multi-output type. `Aroon`, `DonchianChannel`, and `KeltnerChannel` have multi-output indicator properties in their implementations, but current source does not generate usable group view output members for those three types. Use them directly rather than through generated `[BarIndicatorGroup]` fields until generator support is added.

## Windows

Use `[Window]` on read-only bar `double` fields to expose rolling history in the generated context:

```csharp
[BarField(ReadOnly = true)]
[Window(20, 50)]
public partial double Close { get; }
```

Then access `bar.Close.Window(20)` from `OnBar`.

See [indicators and windows](../strategy-authoring/indicators-and-windows.md), [generated fields](../strategy-authoring/generated-fields.md), `bar indicator cookbook` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted), and `windowed fields cookbook` in the [StrategyAuthoring cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/StrategyAuthoring).
