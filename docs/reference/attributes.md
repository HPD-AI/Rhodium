# Attributes

All strategy generation attributes in `Rhodium.Platform.Attributes` target properties. Generated field properties and the containing strategy type must be `partial`; the containing type must derive from `Strategy`.

## Field Attributes

| Attribute | Shape | Generated effect |
|:--|:--|:--|
| `BarFieldAttribute` | `Name`, `ReadOnly = false` | Bar-frequency generated field. Standard read-only names `Open`, `High`, `Low`, `Close`, and `Volume` read market fields; mutable fields use portfolio storage and context setters. |
| `TickFieldAttribute` | `Name`, `ReadOnly = false` | Tick-frequency generated field with typed `TickContext` accessors. |
| `QuoteFieldAttribute` | `Name`, `ReadOnly = false` | Quote-frequency generated field with typed `QuoteContext` accessors. |
| `TradeFieldAttribute` | `Name`, `ReadOnly = false` | Trade-frequency generated field with typed `TradeContext` accessors. |
| `BookFieldAttribute` | `Name`, `ReadOnly = false` | Book-snapshot generated field with typed `BookSnapshotContext` accessors. |

`Name` overrides the tensor field name. Generated properties are context-only; read them from hook contexts such as `bar.Close`, not from the strategy instance.

## Indicator Attributes

`BarIndicatorAttribute(Type indicatorType, params object[] @params)` allocates one bar indicator per asset during generated initialization, updates it before `OnBar`, stores the current value into the generated field, and emits `PropertyIsReady`.

Requirements:

- Pair with `[BarField(ReadOnly = true)]`.
- Use `Source = BarSource.Close | Open | High | Low | Volume | Bar`.
- Use `Param`, `Param0` through `Param7` to bind constructor arguments to `[Param]` properties.
- Use `[BarIndicatorGroup]` only for source-backed generated group views.

`TickIndicatorAttribute(Type indicatorType, params object[] @params)` allocates one tick indicator per asset, updates from the generated tick frame, stores the current value, and emits readiness.

Requirements:

- Pair with `[TickField(ReadOnly = true)]`.
- `TickSource` exposes `Book`, `Quote`, `Trade`, and `Depth`.
- Current generator evidence shows tick updates build a top-of-book `TickFrame`; do not rely on source-specific tick update paths unless source/tests are extended.

## Indicator Groups

`BarIndicatorGroupAttribute(Type indicatorType, params object[] parameters)` declares a generated multi-output bar indicator view. The generator recognizes `MACD`, `BollingerBands`, `Stochastic`, `Aroon`, `DonchianChannel`, and `KeltnerChannel` as multi-output indicators, so using those with scalar `[BarIndicator]` reports `RHD003`.

Generated group view outputs are currently implemented for:

| Indicator | Generated view outputs |
|:--|:--|
| `MACD` | `Value`, `Signal`, `Histogram` |
| `BollingerBands` | `Upper`, `Middle`, `Lower` |
| `Stochastic` | `K`, `D` |

`Aroon`, `DonchianChannel`, and `KeltnerChannel` are detected as multi-output, so scalar `[BarIndicator]` still reports `RHD003`. Current source does not generate usable group view output members for those three types, so do not document `[BarIndicatorGroup]` as a working generated-field workflow for them yet. Use the indicator implementation directly until generator output support lands.

## Windows

`WindowAttribute(params int[] lengths)` allocates rolling tensor history per asset and exposes generated context values as `WindowedDouble`.

Rules:

- Use only on `[BarField(ReadOnly = true)] public partial double ... { get; }`.
- Every length must be positive.
- Context usage is `bar.Close.Window(length)`.

## Parameters

`ParamAttribute` marks init-only strategy properties for generated variants.

Rules:

- Supported property types are `int`, `long`, `double`, `decimal`, `bool`, `string`, and enums.
- Properties must use `{ get; init; }`.
- `Name` can provide a public parameter name different from the property name.
- Generated factories implement `IStrategyParameterFactory<TStrategy>` and static `CreateVariant(ParameterSet)`.

See [parameters and grids](../strategy-authoring/parameters-and-grids.md) for grid construction and [generator diagnostics](generator-diagnostics.md) for failure modes.
