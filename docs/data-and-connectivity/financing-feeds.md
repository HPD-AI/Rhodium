# Financing Feeds

Use `FinancingChargeFeed` to parse deterministic financing commands from direct feeds, rate curves, or position-rate feeds.

```csharp
var commands = FinancingChargeFeed.FromBundledFinancingFeed(
    "financing-crypto-funding",
    defaultStrategyId: strategyId,
    defaultVariantId: 0);
```

Direct financing feed rows are account-slice commands. Rate-curve and position-rate feeds compute command amounts from rates and bases. The feed methods return `FinancingChargeCommand`; they do not directly return replay events.

## Bundled Datasets

Bundled financing dataset ids are:

- `financing-crypto-funding`
- `financing-cash-borrow`
- `financing-rate-curves`

Use:

```csharp
var ids = FinancingChargeFeed.BundledDatasetIds;
var text = FinancingChargeFeed.BundledFinancingFeedDataset("financing-cash-borrow");
```

## Feed Loaders

Direct financing command loaders:

```csharp
FinancingChargeFeed.FromBundledFinancingFeed(datasetId, strategyId, variantId);
FinancingChargeFeed.FromFinancingFeedFile(path, strategyId, variantId);
FinancingChargeFeed.FromFinancingFeed(text, strategyId, variantId);
```

Rate-curve loaders:

```csharp
FinancingChargeFeed.FromBundledRateCurveFeed(datasetId, strategyId, variantId);
FinancingChargeFeed.FromRateCurveFeedFile(path, strategyId, variantId);
FinancingChargeFeed.FromRateCurveFeed(text, strategyId, variantId);
```

Position-rate loaders:

```csharp
FinancingChargeFeed.FromPositionRateFeed(text, positions, strategyId, variantId);
FinancingChargeFeed.FromPositionRateFeed(text, financingBases, strategyId, variantId);
```

Supported charge types in parsed feeds are:

- `CashInterestCredit`
- `CashInterestDebit`
- `BorrowFee`
- `PerpetualFunding`
- `ForexRollover`

## Applying Commands

`ReplayConnector.ApplyFinancingChargeAsync(...)` can apply a `FinancingChargeCommand` in the legacy replay connector context.

For current simulation sessions, convert the command into the replay event shape you want to inject:

```csharp
static FinancingChargeApplied ToEvent(FinancingChargeCommand command, Instant fallbackTime)
{
    var effectiveAt = command.EffectiveAt == default ? fallbackTime : command.EffectiveAt;
    return new FinancingChargeApplied(
        command.FinancingChargeId,
        command.ChargeType,
        command.StrategyId,
        command.VariantId,
        command.Amount,
        effectiveAt,
        command.Instrument,
        command.Quantity,
        command.Rate,
        command.ExternalReference)
    {
        Time = effectiveAt
    };
}
```

Then feed those `FinancingChargeApplied` events through `SimulationDataPlan` or `SharedHistory`.

## Simulation Effects

The replay event type is `FinancingChargeApplied`.

Positive `Amount` credits cash. Negative `Amount` debits cash. `SimulationAccount.ApplyFinancing(...)` adjusts the strategy/variant slice and account cash.

Instrument-level financing is validated against registered contract financing terms. Perpetual funding, forex rollover, and borrow fees must match the contract terms. Contracts without financing terms reject instrument financing.

In `SimulationSession`, incoming `FinancingChargeApplied` events are processed through the simulation event path. Instrument-backed financing can be routed to a simulated venue when the event has an instrument. Cash-only events without an instrument do not route through that venue path.

## Legacy Replay Helper

`ReplayConnector.ApplyFinancingChargeAsync(...)` is available only in the legacy replay connector context. Prefer explicit `FinancingChargeApplied` replay events in `SimulationDataPlan` or `SharedHistory` for new simulation examples.

See [replay connectors](replay-connectors.md) for the legacy caveat.
