#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0
#:package Rhodium.Data@0.1.0

// This sample aggregates deterministic trade prints with the public Rhodium.Data BarAggregator.

using Rhodium.Data.Aggregators;
using Rhodium.Primitives;

var aggregator = BarAggregator.Minutes(1);
var start = Instant.FromUnixSeconds(1_700_000_040);
var completedBars = new List<Bar>();

foreach (var trade in new[]
{
    CreateTrade(start + Duration.FromSeconds(0), price: 100m, size: 10m),
    CreateTrade(start + Duration.FromSeconds(15), price: 101m, size: 5m),
    CreateTrade(start + Duration.FromSeconds(45), price: 99m, size: 8m),
    CreateTrade(start + Duration.FromMinutes(1), price: 102m, size: 4m),
    CreateTrade(start + Duration.FromSeconds(75), price: 103m, size: 6m)
})
{
    if (aggregator.TryAggregate(trade, out var bar))
        completedBars.Add(bar);
}

if (aggregator.Flush() is { } finalBar)
    completedBars.Add(finalBar);

Console.WriteLine($"Bars: {completedBars.Count}");

for (var i = 0; i < completedBars.Count; i++)
{
    var bar = completedBars[i];
    Console.WriteLine(
        $"Bar {i + 1}: O={bar.Open.Value:N2} H={bar.High.Value:N2} L={bar.Low.Value:N2} C={bar.Close.Value:N2} V={bar.Volume.Value:N0}");
}

static Trade CreateTrade(Instant time, decimal price, decimal size)
{
    return new Trade(
        new Price(price, Currency.USD),
        new Qty(size),
        Side.Buy,
        DualTimestamp.Synchronized(time));
}
