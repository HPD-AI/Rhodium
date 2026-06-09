# Market model

Rhodium's market model is the layer between strategy code, replayed market data,
simulated venues, and result analysis. Use these pages when you need to know what
a generated hook receives, what an order helper emits, what a venue reports back,
or what a simulation result preserves.

The model has four practical layers:

- Values: `Qty`, `Price`, `TickPrice`, `Currency`, `Money`, `Instant`,
  `Duration`, and `DualTimestamp`.
- Products: `Asset`, `Venue`, `Instrument`, and the fuller
  `InstrumentContract` description.
- Market data and events: quotes, trades, bars, L2 books, L2 deltas, L3
  market-by-order events, and fixed-depth snapshots.
- Strategy and execution state: generated context objects, `OrderIntent`,
  mutable venue `Order` state, execution events, positions, and simulation
  results.

Start here:

- [Prices, Quantities, And Money](prices-quantities-money.md)
- [Instruments And Contracts](instruments-and-contracts.md)
- [Bars, Quotes, Trades, And Books](bars-quotes-trades-books.md)
- [Market Events](market-events.md)
- [Asset Variants](asset-variants.md)
- [Orders And Positions](orders-and-positions.md)

The most important distinction is physical identity versus runtime routing.
`Instrument` and `InstrumentContract` describe market products. `AssetId` is a
virtual runtime slot used by generated strategies and simulation state; it is not
a physical security, listing, or contract.

Strategy hooks usually work with context objects such as `BarContext`,
`QuoteContext`, `TradeContext`, `TickContext`, `BookSnapshotContext`, and book
delta contexts. Their order helpers emit `OrderIntent`s. Simulated venues turn
those intents into order state and execution events such as `OrderAccepted`,
`OrderFilled`, `OrderRejected`, `OrderCancelled`, and `OrderExpired`.

For runnable end-to-end examples, use the existing cookbooks rather than adding
market-model cookbooks here:

- `Order intents` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted)
- `Parameter grid` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted)
- `First backtest` in the [GettingStarted cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/GettingStarted)
