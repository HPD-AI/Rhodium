# Strategy Hooks

Rhodium strategies combine virtual hooks on `Strategy` with generator-recognized `partial void` market hooks. Market hooks are hot-path hooks; setup and lifecycle hooks are authored as overrides.

## Generated Market Hooks

The generator recognizes these signatures:

```csharp
partial void OnBar(ref BarContext bar);
partial void OnTick(ref TickContext tick);
partial void OnQuote(ref QuoteContext quote);
partial void OnTrade(ref TradeContext trade);
partial void OnBookSnapshot(ref BookSnapshotContext book);
partial void OnBookLevelDelta(ref BookLevelDeltaContext book);
partial void OnBookLevelDeltas(ref BookLevelDeltasContext book);
```

Recognition requires:

- Method name exactly as shown.
- Exactly one `ref` parameter.
- Matching context type name.
- `partial void`; a matching non-partial hook reports `RHD018`.
- The containing strategy type derives from `Strategy` and is `partial`.

Hook-only strategies still compile and receive generated nested contexts even when they declare no generated fields.

## Generated Market Contexts

The public `BarContext`, `TickContext`, `QuoteContext`, `TradeContext`, and `BookSnapshotContext` files are marker ref structs. The usable contexts are generated as nested ref structs inside each partial strategy and shadow those marker type names in hook signatures. `BookLevelDeltaContext` and `BookLevelDeltasContext` are generated-only nested context names; they do not have standalone public marker files.

Generated market contexts share:

- `AssetId`, `StrategyId`, and `PositionQuantity`.
- Generated field accessors and `PropertyFor(AssetId)` cross-asset accessors.
- Mutable `SetPropertyFor(AssetId, value)` where the generated field is mutable.
- Order helpers: `Buy`, `Sell`, `Cancel`, `Modify`, `Flatten`, and `TargetQuantity`.

Event-specific generated properties include:

| Context | Additional generated properties |
|:--|:--|
| `TickContext` | `Frame`, `BidTick`, `AskTick`, `BidSize`, `AskSize`, `BookSpreadTicks`, `MidPrice`, `MicroPrice` |
| `QuoteContext` | `Event`, `Quote`, `Bid`, `Ask`, `BidSize`, `AskSize`, `Mid`, `Spread`, `SpreadBps`, `BidTick`, `AskTick`, `SpreadTicks` |
| `TradeContext` | `Event`, `Trade`, `Price`, `Size`, `AggressorSide`, `PriceTick` |
| `BookSnapshotContext` | `Event`, `Book`, `BestBid`, `BestAsk`, nullable `Bid`, `Ask`, `Mid`, `Spread`, `TopLevelImbalance` |
| `BookLevelDeltaContext` | `Event`, `Delta`, `Side`, `Price`, `Size`, `Action`, `VenueSequence` |
| `BookLevelDeltasContext` | `Event`, `IReadOnlyList<BookLevelDelta> Deltas`, `Count` |

Do not document or depend on generated compiler-facing methods such as `__GeneratedRunBars`, `__GeneratedRunTick`, or `__GeneratedInitialize` as authored user APIs.

## Virtual Strategy Hooks

Override these on `Strategy` when needed:

```csharp
protected override void OnInitialize(in SetupContext setup) { }
public override void OnError(Exception ex) { }
protected override void OnStart(ref LifecycleContext lifecycle) { }
protected override void OnStop(ref LifecycleContext lifecycle) { }
protected override void OnScheduled(ref TimerContext timer) { }
protected override void OnOrderAccepted(ref OrderContext order) { }
protected override void OnOrderModified(ref OrderContext order) { }
protected override void OnOrderRejected(ref OrderContext order) { }
protected override void OnOrderCancelled(ref OrderContext order) { }
protected override void OnOrderExpired(ref OrderContext order) { }
protected override void OnOrderFilled(ref FillContext fill) { }
protected override void OnPositionOpened(ref PositionContext position) { }
protected override void OnPositionChanged(ref PositionContext position) { }
protected override void OnPositionClosed(ref PositionContext position) { }
protected override void OnGroup(ref GroupContext group) { }
```

## Standalone Contexts

`SetupContext` is a cold-path context:

- `UniverseSize`, `Basis`
- `AddEquity(symbol)`, `AddEquity(symbol, variantOffset)`
- `AddInstrument(Instrument, variantOffset = 0)`
- `AddInstrument(InstrumentContract, variantOffset = 0)`
- `ScheduleAt(name, fireAt)`
- `ScheduleEvery(name, interval, startAt = null, stopAt = null)`

Other public contexts:

- `TimerContext`: `Name`, `Time`, `StrategyId`, `GetPositionQty`, and order helpers.
- `LifecycleContext`: lifecycle `Event`.
- `OrderContext`: `StrategyId`, `OrderId`, `Status`, `VariantId`, nullable `AssetId`, `Reason`, `Cancel`, `Modify`.
- `FillContext`: fill fields plus order helpers.
- `PositionContext`: `StrategyId`, `AssetId`, `Kind`, `Previous`, `Current`.
- `GroupContext` and `ChildContext`: child snapshots and group allocation, cap, pause/resume, and apply helpers.

See [event hooks](../strategy-authoring/event-hooks.md), [setup context](../strategy-authoring/setup-context.md), and [orders from contexts](../strategy-authoring/orders-from-contexts.md).
