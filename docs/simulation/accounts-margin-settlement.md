# Accounts, Margin, And Settlement

Account behavior is configured through run, venue, and account-seed setup. Start with initial cash for simple runs, then add account seeds, settlement, or margin only when the scenario needs those mechanics.

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithInitialCash(Money.USD(100_000m))
    .WithConfig(SimulationConfig.USEquities())
    .WithHistory(history)
    .Run();
```

`SimulationConfig.Instant()` settles immediately. `SimulationConfig.USEquities()` uses a US equities-style cash account configuration with T+1 settlement.

## Opening State

Use `WithInitialCash(...)` for default starting cash.

Use `WithAccountSeed(...)` when the opening state should be explicit and replay-visible:

```csharp
builder.WithAccountSeed(new AccountSeed(
    Venue.NASDAQ,
    [Money.USD(200m)],
    [new SeedPosition(spy, new Qty(1m), new Price(50m, Currency.USD))]));
```

Cash seeds become `AccountTransferCompleted` cash deposits. Position seeds become settled asset deposits. Both can produce simulator events and account statements before the first order.

The simulation path consumes completed account-transfer events from replay and seed bootstrapping. A user-facing strategy command workflow for account transfers is not documented here.

## Cash Accounts And Settlement

Cash accounts reserve full upfront cash for buys. Sells of spot or linear instruments can require settled custody, depending on the configured unsettled-sale policy.

Delayed sell proceeds create settlement events:

```csharp
var scheduled = result.SimulatorEvents.OfType<SettlementScheduled>();
var released = result.SimulatorEvents.OfType<SettlementReleased>();
var venue = result.Diagnostics.Venues.Single();

Console.WriteLine(venue.PendingSettlement);
Console.WriteLine(venue.Cash);
```

The cookbook settlement path sells a seeded SPY position with T+1 settlement. Its verified output includes a `100.00 USD` settlement scheduled for `2024-01-08`, then released, with final venue cash of `100.00 USD`.

Settlement calendars and T+N behavior are available through `SettlementParams`. Keep holiday and venue-calendar assumptions tied to the configured `ClearingCalendar` and to tests for the scenario you are documenting.

## Margin Accounts

Set margin behavior through `SimulationConfig`:

```csharp
var config = SimulationConfig.Instant() with
{
    AccountType = AccountType.Margin,
    Margin = MarginParams.Leverage(4m)
};

var result = Rhodium.Simulation.Rhodium
    .Simulate<LeveragedBuyStrategy>()
    .WithInitialCash(Money.USD(300m))
    .WithConfig(config)
    .WithHistory(history)
    .Run();
```

Margin accounts reserve initial margin rather than full notional and can reject orders when buying power is insufficient. Short-sale behavior depends on the configured short-sale and borrow policy.

During replay, margin processing emits `MarginStatusSnapshot` events. Breaches can emit `MarginCallIssued`, then risk-limit and liquidation behavior after the grace period depending on the liquidation policy.

```csharp
var marginCalls = result.SimulatorEvents.OfType<MarginCallIssued>();
var breached = result.SimulatorEvents
    .OfType<MarginStatusSnapshot>()
    .Where(static status => status.IsMaintenanceBreached);
```

The cookbook margin path buys SPY with leverage, replays a mark drop, and verifies two fills, one margin call, one maintenance breach snapshot, and no final positions after liquidation.

Do not treat this as broker parity. Margin fractions, short borrow, option margin, strategy offsets, and liquidation are modeled, but broker-specific portfolio-margin methodology is not promised by these docs.

## Result Surfaces

Read account effects from:

- `result.AccountStatements`: cash, available cash, pending settlement, reserved cash, market value, equity, PnL, positions, and open orders.
- `result.SimulatorEvents`: settlement, custody, transfer, margin, lifecycle, financing, and account diagnostic events.
- `result.Diagnostics.Venues`: final venue cash, available cash, reserved cash, pending settlement and delivery counts, account type, base currency, and order counters.
- `result.Runs.Single().FinalSnapshot.GetPositions()`: strategy-local final positions.

FX conversion is not automatic when cash movement currency differs from the account currency.

Cookbook examples:

- `cookbook/GettingStarted/07-account-seed.cs`
- `cookbook/Simulation/margin-and-settlement.cs`
