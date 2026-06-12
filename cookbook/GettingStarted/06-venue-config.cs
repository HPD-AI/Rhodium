#:package Rhodium.Simulation@0.5.0
#:property TargetFramework=net10.0

// This sample overrides the simulated venue account and matching settings.

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;

var result = Rhodium.Simulation.Rhodium
    .Simulate<VenueConfigStrategy>()
    .WithHistory(SharedHistory.Load([CreateBarClosed(123m)]))
    .WithVenue(
        Venue.NASDAQ,
        initialCash: Money.USD(1_000m),
        baseCurrency: Currency.USD,
        accountType: AccountType.Cash,
        matchingFidelity: MatchingFidelity.FastVectorApproximation)
    .Run();

var venue = result.Diagnostics.Venues.Single();

Console.WriteLine($"Venue: {venue.Venue}");
Console.WriteLine($"Account type: {venue.AccountType}");
Console.WriteLine($"Base currency: {venue.BaseCurrency}");
Console.WriteLine($"Fills: {venue.FilledOrders}");
Console.WriteLine($"Final cash: {venue.Cash}");

static BarClosed CreateBarClosed(decimal close)
{
    var instrument = new Instrument(new Asset("SPY", AssetClass.Equity), Venue.NASDAQ);
    var bar = new Bar(
        new Price(close, Currency.USD),
        new Price(close + 1m, Currency.USD),
        new Price(close - 1m, Currency.USD),
        new Price(close, Currency.USD),
        new Qty(10_000m),
        default,
        Duration.FromMinutes(1));

    return new BarClosed(instrument, bar);
}

public sealed partial class VenueConfigStrategy : Strategy
{
    private AssetId _spy;
    private bool _submitted;

    [BarField(ReadOnly = true)]
    public partial double Close { get; }

    protected override void OnInitialize(in SetupContext setup)
    {
        _spy = setup.AddEquity("SPY");
    }

    partial void OnBar(ref BarContext bar)
    {
        if (_submitted || bar.AssetId != _spy)
            return;

        _submitted = true;
        bar.Buy(new Qty(1m), Execution.Market());
    }
}
