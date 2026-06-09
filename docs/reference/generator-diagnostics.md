# Generator diagnostics

Rhodium strategy authoring uses a source generator and analyzers. These diagnostics are user-facing because they usually point to a strategy declaration rule.

| Code | Severity | Meaning | Fix |
|:-----|:---------|:--------|:----|
| `RHD001` | Warning | Safe Rhodium assemblies directly reference `Rhodium.Unsafe.*`. | Move unsafe access behind the intended unsafe/runtime layer. |
| `RHD002` | Error | An indicator attribute is missing the matching read-only generated field attribute. | Add `[BarField(ReadOnly = true)]` or `[TickField(ReadOnly = true)]` as appropriate. |
| `RHD003` | Error | A generator-detected multi-output indicator was declared as a scalar `[BarIndicator]`. | Use `[BarIndicatorGroup]` only for source-backed generated group views: `MACD`, `BollingerBands`, and `Stochastic`. `Aroon`, `DonchianChannel`, and `KeltnerChannel` are detected by this diagnostic but do not currently have usable generated group view members. |
| `RHD004` | Error | A type declaring generated fields is not `partial`. | Mark the strategy class `partial`. |
| `RHD005` | Error | A generated property is not `partial`. | Mark the generated property `partial`. |
| `RHD012` | Error | Generated fields or hooks were declared outside a `Strategy`. | Derive the containing type from `Rhodium.Platform.Strategy`. |
| `RHD013` | Error | A param-bound indicator references a missing `[Param]`. | Add the matching `[Param]` property or fix the parameter name. |
| `RHD014` | Error | A `[Param]` property uses an unsupported type. | Use `int`, `long`, `double`, `decimal`, `bool`, `string`, or an enum. |
| `RHD015` | Error | `[Window]` was used on an unsupported field. | Use `[Window]` only on a read-only bar `double` field. |
| `RHD016` | Error | A window length is not positive. | Use positive window lengths. |
| `RHD017` | Error | A `[Param]` property is not init-only. | Declare the property with `init`. |
| `RHD018` | Error | A generated hook method is not `partial`. | Declare generated hooks as `partial void`. |
| `RHD019` | Error | Manual calls to generated registration helpers were found. | Let the generator call registration helpers. Do not call `__GeneratedRegisterIndicator` or `__GeneratedRegisterPortfolioField`. |

Common runtime errors:

- `Instruments can only be added during OnInitialize.` Register instruments from `OnInitialize(in SetupContext setup)`.
- Generated strategy properties are context-only. Read them from hook contexts such as `bar.Rsi`, not from the strategy instance.
- `HotPathAllocationException` can appear in debug builds after warm-up when guarded strategy paths allocate managed memory. The current message can name `OnTick()` even when another guarded path triggered it.

Source evidence:

- `Rhodium.Generators/StrategyGenerator.cs`
- `Rhodium.Analyzers/UnsafeAccessAnalyzer.cs`
- `Rhodium.Analyzers/GeneratedRegistrationAnalyzer.cs`
- `Rhodium.Generators.Tests/StrategyGeneratorTests.cs`
