# Execution Spec

`ExecutionSpec` is the modern strategy-facing execution intent. Strategies usually create one through the `Execution` factory and pass it to context order helpers such as `bar.Buy(qty, Execution.Market())`.

`ExecutionSpec` is intent data. Factory methods such as `Execution.Twap()`, `Execution.Vwap()`, and `Execution.Pov()` set algorithm fields; this reference does not claim full simulator slicing semantics beyond fields visible in source/tests.

## Enums

| Enum | Values |
|:--|:--|
| `OrderType` | `Market`, `Limit`, `StopMarket`, `StopLimit`, `MarketIfTouched`, `LimitIfTouched`, `MarketToLimit`, `TrailingStopMarket`, `TrailingStopLimit` |
| `TimeInForce` | `Day`, `GTC`, `IOC`, `FOK`, `GTD` |
| `ExecutionLimitPriceMode` | `None`, `Explicit`, `Bid`, `Ask`, `Mid` |
| `ExecutionAlgorithm` | `None`, `Twap`, `Vwap`, `Pov` |
| `TrailingOffsetType` | `Price`, `Ticks`, `Percent` |

## Fields

`ExecutionSpec` exposes:

- `OrderType`
- `LimitPrice`
- `LimitPriceMode`
- `TimeInForce`
- `PostOnly`
- `MaxSlippageTicks`
- `Algorithm`
- `Horizon`
- `Interval`
- `ParticipationRate`
- `StopPrice`
- `GoodTilDate`
- `DisplayQuantity`
- `TrailingOffset`
- `TrailingOffsetType`

## Factory Methods

Use `Rhodium.Primitives.Execution`:

| Factory | Effect |
|:--|:--|
| `Execution.Market()` | Market order intent. |
| `Execution.Limit()` | Limit order intent without an explicit price until chained with price mode/price. |
| `Execution.MarketToLimit()` | `OrderType.MarketToLimit`. |
| `Execution.StopMarket(price)` | Stop-market trigger in `StopPrice`. |
| `Execution.StopLimit(stop, limit)` | Stop-limit with explicit limit price. |
| `Execution.TrailingStop(offset, offsetType = Price)` | Trailing stop-market. |
| `Execution.TrailingStopLimit(offset, offsetType, limitPrice)` | Trailing stop-limit. |
| `Execution.MarketIfTouched(triggerPrice)` | Market-if-touched trigger in `StopPrice`. |
| `Execution.LimitIfTouched(triggerPrice, limitPrice)` | Limit-if-touched trigger plus explicit limit price. |
| `Execution.Twap()` | Market order intent with `Algorithm = Twap`. |
| `Execution.Vwap()` | Market order intent with `Algorithm = Vwap`. |
| `Execution.Pov(participationRate = 0.1m)` | Market order intent with `Algorithm = Pov` and `ParticipationRate`. |

## Fluent Methods

Price and mode:

- `AtBid()`
- `AtAsk()`
- `AtMid()`
- `At(Price)`

Time-in-force:

- `GoodTilCancelled()`
- `GoodTil(Instant)`
- `ImmediateOrCancel()`

Flags and limits:

- `WithPostOnly()`
- `WithMaxSlippageTicks(int)`
- `Display(Qty)`
- `WithStopPrice(Price)`

Algorithm fields:

- `Over(Duration)`
- `Every(Duration)`
- `MaxParticipation(decimal)`

Source tests verify that `GoodTil(...)` sets `TimeInForce.GTD`, preserves `GoodTilDate`, and survives chaining through methods such as `AtBid()`, `Display(...)`, `WithPostOnly()`, and `WithMaxSlippageTicks(...)`.

## Legacy Command Helpers

Direct `SubmitOrder` factories still exist and are tested for market/limit/iceberg/stop/trailing/MIT/LIT orders, variant ids, numeric tags, and string algorithm ids (`TWAP`, `VWAP`, `POV`). Keep that distinct from `ExecutionSpec.ExecutionAlgorithm`.

See [orders from contexts](../strategy-authoring/orders-from-contexts.md), [fills, fees, slippage, latency](../simulation/fills-fees-slippage-latency.md), and `order intents cookbook` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted).
