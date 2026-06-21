# Options And Lifecycle

Rhodium can simulate option expiry outcomes when option contracts, replay marks or settlement references, and lifecycle configuration give the venue enough information to process expiry.

```csharp
var option = Contracts.OptionContract(
    "SPY261218C00100000",
    new Venue("CBOE"),
    underlying,
    Currency.USD,
    tick: 0.01m,
    lot: 1m,
    multiplier: 100m,
    strike: new Price(100m, Currency.USD),
    expiry,
    OptionRight.Call,
    ExerciseStyle.European,
    exercisePolicy: OptionExercisePolicy.CashSettledAtExpiry);
```

Register contracts in strategy setup or through instrument configuration so the simulation can understand the instrument contract.

```csharp
protected override void OnInitialize(in SetupContext setup)
{
    setup.AddInstrument(underlyingContract);
    _option = setup.AddInstrument(option);
}
```

## Expiry Inputs

Option lifecycle processing is expiry-oriented. The scheduler runs when expiring contracts reach their due timestamp during replay drains.

Expiry can use:

- settlement reference data
- market marks
- lifecycle config overrides
- assignment inputs
- replayed lifecycle events such as `SettlementReferencePricePublished` and `OptionAssignmentNoticePublished`

Long options can expire worthless, remain unexercised when policy says not to exercise, cash settle, or physically deliver. Short options can be assigned, expire unassigned, cash settle, physically deliver, or split assigned and unassigned quantities when pro-rata assignment applies.

## Cash Settlement

The cookbook cash-settlement example buys one SPY call at `2.00 USD`, replays an expiry mark of `105.00 USD` against a `100.00 USD` strike, and reads the lifecycle event:

```csharp
var settlement = result.SimulatorEvents
    .OfType<OptionLifecycleApplied>()
    .Single(static evt => evt.LifecycleKind == OptionLifecycleKind.CashSettlement);

Console.WriteLine(settlement.CashFlow);
Console.WriteLine(settlement.ReferenceSource);
```

The verified output includes:

- one option fill
- `OptionLifecycleKind.CashSettlement`
- `500.00 USD` settlement cash flow
- `MarketMark` reference source
- final cash of `100,300.00 USD`
- realized PnL of `300.00 USD`

Account application removes completely settled option positions, applies cash flow, updates realized PnL, and emits `OptionLifecycleApplied`.

## Blocked Lifecycle

If expiry lacks the reference data or mark required by policy, lifecycle can be blocked:

```csharp
var blocked = result.SimulatorEvents
    .OfType<OptionLifecycleApplied>()
    .Single(static evt => evt.LifecycleKind == OptionLifecycleKind.Blocked);
```

The cookbook blocked thread verifies `OptionLifecycleKind.Blocked`, reference source `None`, and one open position remaining in the final snapshot.

## Assignment And Physical Delivery

`SimulationLifecycleConfig` can provide settlement reference prices and assignment input. Assignment inputs support random-selection and pro-rata-style paths, with optional assignment rules and reasons.

Physical delivery is modeled by applying deliverable positions and settled custody when the lifecycle result calls for it. Projection tests cover strategy-local basis updates for physical delivery, but these docs do not claim a full broker or OCC operational workflow.

Manual early exercise and user-triggered exercise commands are not documented as public workflows here. Keep examples expiry-oriented unless source and tests for a manual workflow are added.

Cookbook example:

- `cookbook/Simulation/options-lifecycle.cs`
