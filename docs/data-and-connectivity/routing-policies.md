# Routing Policies

`TradingHost` can route market order intents across venue-specific connectors when cross-venue routing is enabled and the host has the instrument, quote, connector, fee, and policy information it needs.

```csharp
var host = new TradingHost(connectorsByVenue)
{
    UseCrossVenueBestMarketRouting = true,
    UseCrossVenueMarketSweepRouting = true,
    CrossVenueQuoteMaxAge = Duration.FromSeconds(1),
    CrossVenueRoutingPolicies = VenueRoutingPolicyCatalog.KnownVenues(),
    CrossVenueRoutingFees = feesByVenue
};

host.RegisterStrategy<MyStrategy>(depth: 1);
await host.RunAsync(ct);
```

Routing belongs to live/host connectivity. It is separate from simulation data plans and from `ReplayConnector` legacy policy.

## Host Routing Knobs

`TradingHost` exposes:

- `UseCrossVenueBestMarketRouting`
- `UseCrossVenueMarketSweepRouting`
- `CrossVenueQuoteMaxAge`
- `CrossVenueRoutingFees`
- `CrossVenueRoutingPolicies`
- `UseParallelDispatch`, `ParallelThreshold`, and `MaxDegreeOfParallelism`

A host can be constructed with one default `IConnector` or with a dictionary of venue-specific connectors.

## Best-Market Routing

Best-market routing applies only to market orders when `UseCrossVenueBestMarketRouting` is enabled.

The host considers instruments that share the same asset and variant id. A candidate must have:

- a registered connector for the target venue, unless a default connector is available
- a usable, fresh quote within `CrossVenueQuoteMaxAge`
- an allowed market time-in-force
- passing minimum quantity and minimum notional checks
- compatible quote currency once a best quote is selected

Buy routing minimizes effective price times quantity plus taker fee. Sell routing maximizes proceeds minus taker fee. Fee scoring uses `CrossVenueRoutingFees` and registered `InstrumentContract` data.

If best-market routing is disabled, or if an order is not eligible, explicit order intents resolve to their original instrument.

## Market Sweep Routing

Sweep routing applies only to market orders when `UseCrossVenueMarketSweepRouting` is enabled.

The host sorts candidates by the same fee-aware score, slices order quantity against top-of-book available quantity, respects `VenueRoutingPolicy.MaxMarketSweepQuantity`, and puts residual quantity back onto the first generated order.

## Venue Policies

`VenueRoutingPolicy` controls eligibility:

```csharp
var policy = new VenueRoutingPolicy
{
    AllowBestVenueMarketRouting = true,
    AllowMarketSweepRouting = true,
    MinMarketRoutingQuantity = new Qty(1m),
    MaxMarketSweepQuantity = new Qty(100m)
};
```

Available properties include:

- `AllowBestVenueMarketRouting`
- `AllowMarketSweepRouting`
- `AllowedMarketTimeInForce`
- `MinMarketRoutingQuantity`
- `MinMarketRoutingNotional`
- `MaxMarketSweepQuantity`

`VenueRoutingPolicyCatalog` includes presets:

- `BinanceCrypto()`
- `CoinbaseCrypto()`
- `InteractiveBrokersListedEquity()`
- `CryptoSpot()`
- `USListedEquities()`
- `KnownVenues()`

Bundled routing dataset ids include:

- `routing-crypto-spot`
- `routing-us-listed-equities`

Policy feed loaders can read bundled, text, or file feeds. Feed columns include venue, best-routing flag, sweep flag, allowed time-in-force set, minimum quantity, minimum notional amount/currency, and maximum sweep quantity.

## Diagnostics

The host emits `CrossVenueArbitrageOpportunity` diagnostics when fresh quotes for the same asset cross across venues after basic quote usability and age checks.
