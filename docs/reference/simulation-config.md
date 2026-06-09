# Simulation Config

`SimulationConfig` configures replay-based execution behavior: latency, queue, fees, slippage, price improvement, account/margin/settlement, lifecycle, bar ordering, fill behavior, and deterministic seed.

## Run Options

`SimulationRunOptions` defaults:

- `Config = SimulationConfig.Instant()`
- `MatchingFidelity = MatchingFidelity.QueueAccurate`
- `InitialCash = Money.USD(100_000m)`
- `MaxDegreeOfParallelism = 1`
- `MaxSameTimestampIterations = 128`
- `ReadOptions = ReplayReadOptions.All`
- empty venue, account seed, and module lists
- `FrameMode = SimulationFrameMode.Disabled`

Builder entry points include:

- `Rhodium.Simulate<TStrategy>()`
- `WithHistory(...)`
- `WithData(SimulationDataIterator)`
- `WithData(SimulationDataPlan)`
- `WithMatchingFidelity(...)`
- `WithConfig(...)`
- `WithInitialCash(...)`
- `WithAccountSeed(...)`, `WithAccountSeeds(...)`
- `WithVenue(...)`
- `WithInstrument(...)`
- `WithSessionModule(...)`, `WithVenueModule(...)`, `WithInstrumentModule(...)`
- `WithFrameMode(...)`
- `WithMaxDegreeOfParallelism(...)`
- `WithGrid(...)`
- `Run()`, `RunAsync()`

## Config Fields

Required fields:

- `Latency`
- `QueueModel`
- `Fees`

Defaulted fields:

| Field | Default |
|:--|:--|
| `Slippage` | `SlippageParams.None` |
| `PriceImprovement` | `PriceImprovementParams.None` |
| `FillBehavior` | `FillBehavior.NoPartialFill` |
| `RequiredDepth` | `DepthLevel.L2_MarketByPrice` |
| `AccountType` | `AccountType.Cash` |
| `Margin` | `MarginParams.RegT()` |
| `Settlement` | `SettlementParams.Immediate()` |
| `Lifecycle` | `SimulationLifecycleConfig.Default` |
| `InitialMarketStatus` | `MarketStatus.Open` |
| `BarOrdering` | `BarOrderingMode.Fixed` |
| `FillModel` | `DefaultFillModel` |
| `Seed` | `0` |

`Seed` is the explicit source default. Seed expansion exists downstream for deterministic PRNG state, but the `SimulationConfig` property default itself is `0`.

## Presets

| Preset | Intended use |
|:--|:--|
| `SimulationConfig.Instant()` | Strategy-logic testing: zero latency, always-front queue, zero fees. |
| `SimulationConfig.CryptoFuturesRealistic()` | Liquid crypto futures: 500 microsecond latency, liquid queue, Binance futures fees, partial fills on trade. |
| `SimulationConfig.Conservative()` | Risk assessment: 10 ms latency, risk-averse queue, higher percentage fees, volume-proportional slippage, no partial fills. |
| `SimulationConfig.IlliquidMarket()` | Low-liquidity markets: 1 ms latency, illiquid queue, 10/20 bps maker/taker fees, volume slippage, partial fills. |
| `SimulationConfig.USEquities()` | US equities: 100 microsecond latency, power quadratic queue, fixed USD 0.50 fee, T+1 NYSE settlement. |

## Venue And Instrument Overrides

`SimulationVenueConfig` can override run defaults per venue:

- `Venue`
- optional `InitialCash`, `BaseCurrency`, `AccountType`
- optional `Config`, `MatchingFidelity`
- `OrderPolicy`, `SimulationPolicy`
- instrument configs

`SimulationInstrumentConfig` can override venue/run defaults per instrument:

- `Instrument`
- optional `Contract`
- optional `Config`, `MatchingFidelity`
- `InitialStatus`
- `OrderPolicy`, `SimulationPolicy`

## Execution Model Knobs

Latency:

- `LatencyParams(EntryMean, ResponseMean, StdDevFraction)`

Queue:

- `QueueModelType`: `AlwaysFront`, `RiskAverse`, `PowerProbabilistic`, `PowerProbabilistic2`, `PowerProbabilistic3`, `LogProbabilistic`, `LogProbabilistic2`.
- Factories include `AlwaysFront`, `Tail`, `DeterministicTail`, `RiskAverse`, `PowerQuadratic`, `PowerCubic`, `PowerAsymmetric`, `Logarithmic`, `RealisticLiquid`, `RealisticIlliquid`.

Fees:

- `FeeModelType`: `PercentageOfValue`, `PerQuantity`, `PerTrade`, `TieredByVolume`, `Directional`, `ContractTerms`.
- Factories/presets include `Zero`, `ContractTerms`, `MakerTaker`, `PerLot`, `Fixed`, `Directional`, `Tiered`, `BinanceFutures`, `CoinbaseAdvanced`, `InteractiveBrokers`, and `TieredFeeSchedule.BinanceFuturesVIP`.

Slippage and price improvement:

- `SlippageModelType`: `None`, `VolumeProportional`, `VolatilityAdjusted`.
- `PriceImprovementModelType`: `None`, `FixedBps`.
- Slippage is unfavorable to side; price improvement is favorable to side.

Fills:

- `FillBehavior`: `NoPartialFill`, `FillOnTouch`, `PartialFillOnTrade`.
- `BarOrderingMode`: `Fixed`, `Adaptive`.
- `DepthLevel`: `L1_TopOfBook`, `L2_MarketByPrice`.
- Fill model interfaces/classes: `IFillModel`, `DefaultFillModel`, `SizeAwareFillModel`.

## Fidelity And Policies

`MatchingFidelity` values:

- `FastVectorApproximation`
- `QueueAccurate`
- `MarketByOrder`

Policy surfaces:

- `SimulationOrderPolicy`: allowed order types/TIF, `AllowPostOnly`, min quantity, min notional.
- `SimulationVenuePolicy`: bar/trade execution, liquidity consumption, triggered order rejection behavior, contingent orders, market acks, cash borrowing, frozen account, price protection, reduce-only, OTO trigger behavior.

## Accounts, Settlement, Lifecycle

Accounts:

- `AccountType.Cash`
- `AccountType.Margin`
- `MarginParams.RegT()`
- `MarginParams.Leverage(decimal)`

Settlement:

- `SettlementParams.Immediate()`
- `SettlementParams.CalendarDays(...)`
- `SettlementParams.TPlus(...)`
- `SettlementParams.TPlusForVenue(...)`
- `SettlementParams.FromContract(...)`
- `SettlementParams.FromTerms(...)`
- `WithUnsettledSalePolicy(...)`

Calendars:

- `ClearingCalendar.Weekdays`
- `ClearingCalendar.AlwaysOpen`
- `ClearingCalendar.USEquities`
- `ClearingCalendar.USFutures`
- `ClearingCalendar.Crypto`
- `ClearingCalendar.ForVenue(...)`

Lifecycle:

- `SimulationLifecycleConfig.Default`
- `WithSettlementReferencePrice(...)`
- `WithAssignmentInput(...)`
- `WithMissingReferencePricePolicy(...)`
- `MissingReferencePricePolicy.BlockLifecycle`
- `MissingReferencePricePolicy.Throw`

See [simulation builder](../simulation/simulation-builder.md), [fills, fees, slippage, latency](../simulation/fills-fees-slippage-latency.md), [accounts, margin, settlement](../simulation/accounts-margin-settlement.md), [options lifecycle](../simulation/options-lifecycle.md), and [simulation data plans](../data-and-connectivity/simulation-data-plans.md).
