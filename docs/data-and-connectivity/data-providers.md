# Data Providers

Rhodium exposes provider and connector interfaces, but the inspected source does not include built-in live broker/provider integrations. Document live providers at the interface boundary unless your project supplies a concrete connector.

## Connector Interface

Implement `IConnector` to stream market data and accept order commands:

```csharp
public sealed class MyConnector : IConnector
{
    public ExchangeId Exchange { get; }
    public IRateLimiter RateLimiter { get; }
    public bool IsConnected { get; private set; }

    public Task StartAsync(
        IEnumerable<Subscription> subscriptions,
        IEventPublisher events,
        CancellationToken ct)
    {
        // Push FinanceEvent instances into events.
        throw new NotImplementedException();
    }

    public Task SubmitOrderAsync(SubmitOrder command, CancellationToken ct) => throw new NotImplementedException();
    public Task CancelOrderAsync(CancelOrder command, CancellationToken ct) => throw new NotImplementedException();
    public Task ModifyOrderAsync(ModifyOrder command, CancellationToken ct) => throw new NotImplementedException();
    public void Dispose() { }
}
```

`StartAsync(...)` receives subscriptions and pushes `FinanceEvent` instances into the supplied publisher. Order methods are expected to emit order lifecycle or fill events through the same event path.

## Subscriptions

```csharp
var subscriptions = new[]
{
    new Subscription(spy, SubscriptionType.Trades),
    new Subscription(spy, SubscriptionType.Quotes),
    new Subscription(spy, SubscriptionType.Depth, SubscriptionDepth.L2_20),
    new Subscription(spy, SubscriptionType.Bars)
};
```

Subscription types are:

- `Trades`
- `Quotes`
- `Depth`
- `Bars`

Depth values are:

- `Top`
- `L2_5`
- `L2_10`
- `L2_20`
- `Full`

`TradingHost.BuildSubscriptions()` creates trades, quotes, L2_20 depth, and bars subscriptions for variant-0 instruments in the runtime batch map.

## Payload Normalization

Use `INormalizer` for hot-path exchange payload parsing:

```csharp
public interface INormalizer
{
    ExchangeId Exchange { get; }
    int Normalize(ReadOnlySpan<byte> rawPayload, Span<FinanceEvent> outputBuffer);
    IReadOnlyList<FinanceEvent> Normalize(ReadOnlySpan<byte> rawPayload);
}
```

The span overload lets callers provide the output buffer. The convenience overload may allocate.

## Instrument Metadata Normalization

`ProviderInstrumentNormalizer` is a static helper for cold-path provider metadata:

```csharp
var result = ProviderInstrumentNormalizer.Normalize(providerMetadata);
var contract = ProviderInstrumentNormalizer.NormalizeOrThrow(providerMetadata);
```

It maps provider instrument metadata into `InstrumentContract` and reports `ProviderInstrumentDiagnostic` issues when fields are missing, ambiguous, or unsupported.

## Data Provider Interface

Implement `Rhodium.Data.IDataProvider` to fetch historical market data from an external source:

```csharp
public interface IDataProvider
{
    string Name { get; }

    IAsyncEnumerable<Bar> GetBarsAsync(
        Instrument instrument,
        Duration period,
        DateRange range,
        CancellationToken ct = default);

    IAsyncEnumerable<Trade> GetTradesAsync(
        Instrument instrument,
        DateRange range,
        CancellationToken ct = default);

    IAsyncEnumerable<Quote> GetQuotesAsync(
        Instrument instrument,
        DateRange range,
        CancellationToken ct = default);
}
```

`Name` identifies the provider. The fetch methods stream historical bars, trades, and quotes for the requested instrument and date range. The inspected source documents example provider names in comments, but no built-in live/vendor `IDataProvider` implementations were found.

## Rate Limiters

Connectors expose `IRateLimiter`:

```csharp
public interface IRateLimiter
{
    bool TryAcquire(int permits = 1);
    Task WaitAsync(int permits = 1, CancellationToken ct = default);
    int AvailablePermits { get; }
}
```

Source-backed implementations include `NoopRateLimiter.Instance` and `TokenBucketRateLimiter`.

## Provider Boundary

`ExchangeId` examples in source comments include venues such as Binance and Alpaca, and routing/replay policy catalogs include venue presets. Those are not live connector implementations. A production provider integration must supply its own `IConnector`, normalizer, subscription mapping, rate limiter, account/order behavior, and operational error handling.
