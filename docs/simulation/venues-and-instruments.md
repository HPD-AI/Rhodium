# Venues And Instruments

Simulations create venue and instrument state as replay events and strategy orders touch them. Add explicit venue or instrument configuration when the defaults are not enough for the scenario you are testing.

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithVenue(SimulationVenueConfig.For(Venue.NASDAQ) with
    {
        InitialCash = Money.USD(50_000m),
        AccountType = AccountType.Cash,
        MatchingFidelity = MatchingFidelity.QueueAccurate
    })
    .WithHistory(history)
    .Run();
```

Empty replay can produce empty venue diagnostics. A configured venue becomes visible once replay events, account seeds, or order flow create or touch the simulated exchange.

## Venue Configuration

`SimulationVenueConfig` sets behavior for one venue. Public knobs include:

- `Venue`
- `InitialCash`
- `BaseCurrency`
- `AccountType`
- `Config`
- `MatchingFidelity`
- `OrderPolicy`
- `SimulationPolicy`
- `InstrumentConfigs`

Use venue config for account-level choices such as cash versus margin, venue-specific starting cash, or a venue-level matching override.

```csharp
builder.WithVenue(SimulationVenueConfig.For(Venue.NASDAQ) with
{
    InitialCash = Money.USD(10_000m),
    BaseCurrency = Currency.USD,
    AccountType = AccountType.Margin,
    Config = SimulationConfig.Instant() with
    {
        Margin = MarginParams.Leverage(2m)
    }
});
```

If a venue sets only `BaseCurrency`, the default initial cash amount is re-denominated into that currency. Rhodium does not automatically perform FX conversion for account cash movements in a different currency; configure and seed accounts in the currency you intend to simulate.

## Instrument Configuration

`SimulationInstrumentConfig` overrides behavior for one instrument under its venue. It includes:

- `Instrument`
- required `InstrumentContract`
- optional `Config`
- optional `MatchingFidelity`
- `InitialStatus`
- `OrderPolicy`
- `SimulationPolicy`

Use it when an instrument needs a specific contract, tick/lot behavior, status, policy, or fidelity mode:

```csharp
builder.WithInstrument(new SimulationInstrumentConfig(
    Instrument: option.Instrument,
    Contract: option)
{
    MatchingFidelity = MatchingFidelity.QueueAccurate,
    InitialStatus = MarketStatus.Open
});
```

Instrument settings are the most specific. Venue settings override run defaults for that venue. Run settings are the fallback.

## Account Seeds

`WithInitialCash(...)` initializes venue account cash. `WithAccountSeed(...)` creates explicit opening account state before replay begins:

```csharp
builder.WithAccountSeed(new AccountSeed(
    Venue.NASDAQ,
    [Money.USD(200m)],
    [new SeedPosition(spy, new Qty(1m), new Price(50m, Currency.USD))]));
```

Cash seeds are projected as completed cash deposits. Position seeds are projected as settled asset deposits and register the runtime contract for the seeded instrument. These transfers are visible in `result.SimulatorEvents` and can produce account statements before the first strategy order.

Seeds without a `StrategyId` target root strategies. Use targeted seeds when a multi-strategy or multi-variant run needs different opening state.

Cookbook examples:

- `cookbook/GettingStarted/06-venue-config.cs`
- `cookbook/GettingStarted/07-account-seed.cs`
- `cookbook/Simulation/margin-and-settlement.cs`
