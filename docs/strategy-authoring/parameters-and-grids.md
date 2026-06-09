# Parameters and grids

Strategy parameters are init-only properties marked with `[Param]`.

```csharp
public sealed partial class GridStrategy : Strategy
{
    [Param] public int Lots { get; init; }
    [Param] public int ExitAfterBars { get; init; }

    [BarField(ReadOnly = true)]
    public partial double Close { get; }

    partial void OnBar(ref BarContext bar)
    {
        if (bar.PositionQuantity == 0m)
            bar.Buy(new Qty(Lots), Execution.Market());
    }
}
```

Supported parameter types are `int`, `long`, `double`, `decimal`, `bool`, `string`, and enums. Parameter properties must be init-only.

Use `[Param(Name = "...")]` when the grid key should differ from the property name:

```csharp
[Param(Name = "lots")]
public int Lots { get; init; }
```

## Generated variants

When a strategy has parameters, the generator emits `IStrategyParameterFactory<TStrategy>` support and a static `CreateVariant(ParameterSet parameters)` method. The generated method reads values with `ParameterSet.GetRequired<T>(gridName, propertyName)`, so missing values or incompatible value types fail before the variant is registered and run.

```csharp
var parameters = new ParameterSet(new Dictionary<string, object>
{
    [nameof(GridStrategy.Lots)] = 2,
    [nameof(GridStrategy.ExitAfterBars)] = 5
});

var strategy = GridStrategy.CreateVariant(parameters);
```

Use property names for grid keys unless `[Param(Name = "...")]` supplies a custom key.

## Cartesian grids

`ParameterGrid.Create().Add(...)` builds a Cartesian product:

```csharp
var grid = ParameterGrid.Create()
    .Add(nameof(GridStrategy.Lots), 1, 2, 3)
    .Add(nameof(GridStrategy.ExitAfterBars), 2, 4);
```

This grid has six variants. Variant parameter sets are produced by combining every value on every axis.

`Add` rules:

- The parameter name must be non-empty.
- Each axis must contain at least one value.
- The same parameter name cannot be added twice.
- Values must be assignable to the target `[Param]` property type.

## Exact-row grids

Use `ParameterGrid.FromParameterSets(...)` when you want exact rows instead of a Cartesian product:

```csharp
var grid = ParameterGrid.FromParameterSets([
    new ParameterSet(new Dictionary<string, object>
    {
        [nameof(GridStrategy.Lots)] = 1,
        [nameof(GridStrategy.ExitAfterBars)] = 2
    }),
    new ParameterSet(new Dictionary<string, object>
    {
        [nameof(GridStrategy.Lots)] = 5,
        [nameof(GridStrategy.ExitAfterBars)] = 8
    })
]);
```

Exact-row grids preserve the supplied rows and cannot be extended with `.Add(...)`. Calling `.Add(...)` on an exact-row grid throws `InvalidOperationException`.

## Register all variants

`StrategyGrid<TStrategy>` registers each variant by calling the generated `TStrategy.CreateVariant(parameters)` and records a `VariantDescriptor` for each registered strategy id:

```csharp
var strategyGrid = new StrategyGrid<GridStrategy>(grid);
var ids = strategyGrid.RegisterAll(tree, depth: 0);

foreach (var variant in strategyGrid.Variants)
{
    var lots = variant.Parameters.Get<int>(nameof(GridStrategy.Lots));
}
```

The simulation builder also accepts grids:

```csharp
var result = Rhodium.Simulation.Rhodium
    .Simulate<GridStrategy>()
    .WithHistory(history)
    .WithGrid(grid)
    .Run();
```

## Param-bound indicators

Indicator constructor arguments can bind to parameters. Use the `[Param]` property name, or its custom `[Param(Name = "...")]` key:

```csharp
[Param] public int Period { get; init; }

[BarField(ReadOnly = true)]
[BarIndicator(typeof(RSI), Param = nameof(Period))]
public partial double Rsi { get; }
```

The generator passes the strategy property value into the indicator constructor for each generated variant.

## Common failures

- Missing grid value: `Parameter grid is missing value for strategy parameter 'Name'.`
- Incompatible grid value type: the value cannot be assigned to the strategy parameter property type.
- Unsupported `[Param]` type: generator diagnostic `RHD014`.
- Non-init parameter property: generator diagnostic `RHD017`.
- Param-bound indicator references a missing `[Param]`: generator diagnostic `RHD013`.

Cookbook example:

```text
cookbook/GettingStarted/05-parameter-grid.cs
```
