# Asset Variants

`AssetId` is a virtual runtime slot.

```csharp
var spy = setup.AddEquity("SPY");
var spyFast = spy.WithVariant(1);
```

`WithVariant` derives another virtual slot for the same registered product. You
can pass that derived `AssetId` explicitly to route orders or position targets
to a sibling slot, but deriving it does not register it as a normal
generated-context dispatch asset. Register slots during setup when the strategy
should receive generated callbacks for them.

Do not call an `AssetId` a security, venue listing, contract, or physical
instrument identity.

## Where Variants Appear

Generated strategy contexts expose the current slot as `AssetId`.

```csharp
partial void OnBar(ref BarContext bar)
{
    if (bar.PositionQuantity == 0m)
        bar.Buy(new Qty(1m));

    var fastSlot = bar.AssetId.WithVariant(1);
    bar.TargetQuantity(fastSlot, new Qty(2m));
}
```

Order helpers use the current slot unless you pass an explicit `AssetId`.

```csharp
partial void OnTick(ref TickContext tick)
{
    var sibling = tick.AssetId.WithVariant(1);

    tick.Buy(new Qty(1m), Execution.Limit().AtBid().WithPostOnly());
    tick.Sell(sibling, new Qty(1m), Execution.Twap().Over(Duration.FromSeconds(30)));
}
```

Orders and execution events also carry variant information. `Order` has
`VariantId`; execution events such as `OrderAccepted`, `OrderFilled`, and
`OrderCancelled` include `VariantId` and may include an explicit `AssetId`.

## State Routing

Position state is intentionally scoped.

- Strategy runtime position slots are keyed by `StrategyId` and
  `AssetId.VirtualIndex`.
- Simulation account positions are keyed by `StrategyId`, `VariantId`, and
  `Instrument`.
- Market data can be shared across virtual slots; do not assume every variant creates
  isolated market data.

Use asset variants for strategy-specific views and same-instrument slots. Use
`Instrument` and `InstrumentContract` for product identity. Parameter grids are
strategy variants, modeled with `ParameterGrid` / `WithGrid` and reported
through `StrategyRunResult.VariantIndex`; they are separate from `AssetId`
virtual slots.

For a runnable grid example, see
`cookbook/GettingStarted/05-parameter-grid.cs` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted).
