# Instruments And Contracts

Rhodium separates product identity from the full contract definition.

```csharp
var asset = new Asset("SPY", AssetClass.Equity);
var venue = Venue.NYSE;
var instrument = new Instrument(asset, venue);

var contract = Contracts.Equity("SPY", Venue.NYSE, Currency.USD);
```

`Asset` is the symbol, asset class, and optional underlying symbol. `Venue` names
the execution or data venue. `Instrument` is `Asset + Venue`.

`InstrumentContract` is the canonical product description. It includes:

- identity and symbol metadata
- trading grid and constraints
- economic exposure, lifecycle, settlement, margin, fees, and financing
- payoff terms, venue rules, and data semantics
- legs, package terms, and tags

Contracts are explicit product descriptions, not just symbols.

## Recipes

Use recipe helpers when the product details cannot be inferred from identity
alone.

```csharp
var equity = Contracts.Equity("AAPL", Venue.NASDAQ, Currency.USD);

var btc = Contracts.CryptoSpot(
    "BTC/USD",
    Venue.Coinbase,
    baseCurrency: Currency.BTC,
    quoteCurrency: Currency.USD,
    tick: 0.01m,
    lot: 0.0001m);

var es = Contracts.Future(
    "ESM6",
    Venue.CME,
    underlying: new Instrument(new Asset("ES", AssetClass.Index), Venue.CME),
    quoteCurrency: Currency.USD,
    tick: 0.25m,
    lot: 1m,
    multiplier: 50m,
    expiry: Instant.FromDateTimeOffset(new DateTimeOffset(2026, 6, 19, 21, 0, 0, TimeSpan.Zero)));
```

`Contracts.FromIdentity(instrument, quoteCurrency)` is intentionally limited to
identity classes that are unambiguous from identity alone, such as equities,
indexes, and observables. For crypto, options, futures, spreads, perps, binary
contracts, CFDs, tokenized assets, and other structured products, use an
explicit recipe.

## Grids And Semantics

The contract grid is what strategy and simulation code use for tick conversion
and size rounding.

```csharp
var tick = contract.Grid.ToTick(new Price(500.25m, Currency.USD));
var roundedSize = contract.Grid.RoundSize(new Qty(10.7m));
```

`DataSemantics` tells the runtime how to treat the contract's data:

- `DataSemantics.Tradable` represents tradable market prices.
- `DataSemantics.Observable` represents market-data or reference objects.
- `DataSemantics.Custom` represents custom signal or mark-driving data.

Observable contracts are reference/data objects. Do not treat every
`Instrument` as directly executable; execution support belongs to the
contract's `VenueRules` and data semantics.

## Options

Option details are typed. `OptionTerms` carries the underlying instrument,
option kind, exercise style, strike terms, activation, expiration, settlement,
premium currency, and multiplier. Validation checks positive multipliers and
units, activation before expiration, Bermudan exercise dates, and positive
strike terms.

Prefer typed option recipes and terms over loose tags when strategy or
simulation behavior depends on option semantics.
