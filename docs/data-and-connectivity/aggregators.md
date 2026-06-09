# Aggregators

Use `Rhodium.Data.Aggregators.BarAggregator` when you want to manually turn deterministic `Trade` values into time bars before feeding those bars into a simulation.

```csharp
var aggregator = BarAggregator.Minutes(1);
var bars = new List<Bar>();

foreach (var trade in trades)
{
    if (aggregator.TryAggregate(trade, out var completed))
        bars.Add(completed);
}

if (aggregator.Flush() is { } finalBar)
    bars.Add(finalBar);
```

Cookbook example: `cookbook/Data/aggregate-bars.cs` in the [Data cookbook](https://github.com/HPD-AI/Rhodium/tree/main/cookbook/Data)

The cookbook was run with filtered output:

```text
Bars: 2
Bar 1: O=100.00 H=101.00 L=99.00 C=99.00 V=23
Bar 2: O=102.00 H=103.00 L=102.00 C=103.00 V=10
```

## Public Aggregators

Aggregators implement:

```csharp
public interface IAggregator<TIn, TOut>
{
    bool TryAggregate(TIn input, out TOut aggregate);
    TOut? Flush();
    void Reset();
}
```

The public source-backed implementations are:

- `BarAggregator`: time-based OHLCV bars from trades.
- `TickBarAggregator`: bars from a fixed trade count.
- `VolumeBarAggregator`: bars from a volume threshold.
- `RenkoAggregator`: price bricks.

`BarAggregator` has constructors and factories:

```csharp
var custom = new BarAggregator(Duration.FromMinutes(5));
var oneMinute = BarAggregator.Minutes(1);
var hourly = BarAggregator.Hours(1);
var daily = BarAggregator.Daily();
```

`BarAggregator.Flush()` returns the current partial bar and resets the aggregator. `RenkoAggregator.Flush()` intentionally does not emit a partial brick.

## Feeding Bars Into Simulation

Aggregation is manual composition. After collecting bars, wrap them as events yourself:

```csharp
var events = bars.Select(bar => new BarClosed(spy, bar) { Time = bar.Time });

var plan = SimulationDataPlan
    .Create()
    .AddSource("aggregated-bars", events);
```

There is no source-backed automatic data-plan, catalog, or replay-source stage that aggregates trades into bars during replay. If you need that behavior, build it explicitly before creating the `SimulationDataPlan` or `SharedHistory`.
