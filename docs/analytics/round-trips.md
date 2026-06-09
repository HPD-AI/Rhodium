# Round Trips

`RoundTrip` is the completed-trade unit used by tear-sheet analytics. It represents an entry fill matched with an opposite-side exit fill for the same instrument.

Use the completed simulation result when you want the built-in summaries:

```csharp
var sheet = result.Runs.Single().TearSheet;
```

Use `RoundTripBuilder` when you need custom trade-level analysis:

```csharp
var fills = result.ExecutionEvents.OfType<OrderFilled>();
var roundTrips = RoundTripBuilder.FromFills(fills).ToArray();
var sheet = TearSheet.Calculate(roundTrips, initialCapital);
```

## Matching Rules

`RoundTripBuilder.FromFills(...)` sorts fills by venue, symbol, then time. It FIFO-matches opposite-side fills per instrument.

It supports long and short trades, partial exits, same-side adds, multiple instruments, and proportional commission allocation. Open or unmatched fills are left unmatched and do not produce `RoundTrip` records.

`RoundTripBuilder.FromOrders(...)` filters filled orders, converts them to synthetic `OrderFilled` records using filled quantity, average fill price or zero, total commission, variant id, and response timestamp, then delegates to `FromFills(...)`.

Simulation results already use this path: each run's tear sheet is built from that strategy's fills, converted to completed FIFO round trips, then passed to `TearSheet.Calculate(...)`.

## RoundTrip Fields

A `RoundTrip` stores:

- `Instrument`
- `Side`
- `Quantity`
- `EntryPrice`
- `ExitPrice`
- `EntryTime`
- `ExitTime`
- `Commission`

It computes:

- `GrossPnL`
- `NetPnL`
- `ReturnPct`
- `HoldingPeriod`
- `IsWin`
- `IsLoss`
- `IsBreakeven`
- `Notional`

`ReturnPct` is zero when the entry price is zero.

## Parameter Finalists

Round-trip analytics often feed a two-pass scan workflow. After a broad grid run, take finalist rows and rebuild an exact-row grid:

```csharp
var finalists = result.TopByTotalReturn(5).ToParameterGrid();
```

The rebuilt grid preserves completed run rows. It is not the original Cartesian grid definition and cannot be extended with new axes.

See [tear sheets](tear-sheets.md), [batch analysis](batch-analysis.md), and [result analysis](../simulation/result-analysis.md).
