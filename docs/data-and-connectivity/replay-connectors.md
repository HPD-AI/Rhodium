# Replay Connectors

`ReplayConnector` is public, but it is obsolete. Treat it as a legacy connector-shaped parity oracle for certification and behavior comparison, not as the recommended simulation/backtesting entry point.

For new simulation replay, prefer:

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<MyStrategy>()
    .WithData(plan)
    .Run();
```

See [simulation data plans](simulation-data-plans.md) and [streaming data](../simulation/streaming-data.md).

## Legacy Surface

`ReplayConnector` implements `IConnector` and accepts:

- `IAsyncEnumerable<FinanceEvent>` history
- optional `SimulationConfig`
- optional `IFillModel`
- optional `IRiskGuard`
- optional initial cash

It exposes connector operations:

```csharp
await connector.StartAsync(subscriptions, events, ct);
await connector.SubmitOrderAsync(command, ct);
await connector.CancelOrderAsync(command, ct);
await connector.ModifyOrderAsync(command, ct);
```

It also exposes replay-only helper commands for account transfers, corporate actions, and financing charges, including `ApplyFinancingChargeAsync(...)`.

## Subscriptions

`StartAsync(...)` receives `Subscription` records:

```csharp
var subscriptions = new[]
{
    new Subscription(spy, SubscriptionType.Trades),
    new Subscription(spy, SubscriptionType.Quotes),
    new Subscription(spy, SubscriptionType.Depth, SubscriptionDepth.L2_20),
    new Subscription(spy, SubscriptionType.Bars)
};
```

The replay connector initializes depth tracking for `Depth` and `Quotes` subscriptions. Subscription types are `Trades`, `Quotes`, `Depth`, and `Bars`; depth values include `Top`, `L2_5`, `L2_10`, `L2_20`, and `Full`.

## Event Processing

For each input event, the legacy replay path advances replay time, lets replay modules pre-process, updates market state such as status/depth, drains due delayed work, processes active algo orders, emits the market event, checks fills, runs timestamp modules once, and emits margin snapshots.

On finalization it cancels active algo orders, emits custody/account settlement outputs, flushes pending responses, resets modules, and marks itself disconnected.

These details are useful when comparing legacy connector behavior against `SimulationSession` and simulated venue behavior. They should not be treated as the architecture for new simulation examples.

## Replay Modules

Legacy modules implement `IReplaySimulationModule`:

```csharp
public interface IReplaySimulationModule
{
    void PreProcess(in FinanceEvent evt, ReplayModuleContext context, ReplayModuleSinks sinks);
    void Process(Instant now, ReplayModuleContext context, ReplayModuleSinks sinks);
    void Reset();
}
```

`ReplayModuleContext` exposes `Now`, market status, and depth lookup. `ReplayModuleSinks.Emit(...)` queues generated events into the replay processing path.

For new `SimulationSession` extensions, use the simulation module APIs instead of replay modules.

## Replay Order Policies

`ReplayVenueOrderPolicy` controls connector-side order admission before matching, risk, and account checks:

- allowed order types
- allowed time-in-force values
- post-only support
- minimum order quantity
- minimum order notional

`ReplayVenueOrderPolicyCatalog` includes crypto and US listed-equity presets and feed loaders. Bundled dataset ids include:

- `replay-order-crypto-spot`
- `replay-order-us-listed-equities`

Do not confuse replay order policies with `SimulationVenuePolicy`, which is the current simulation venue policy surface used by simulated venues and sessions.

## Financing In Legacy Replay

`ReplayConnector.ApplyFinancingChargeAsync(...)` turns a `FinancingChargeCommand` into replay-visible financing events and account effects.

For current simulation sessions, feed `FinancingChargeApplied` events through the replay stream or data plan. See [financing feeds](financing-feeds.md).
