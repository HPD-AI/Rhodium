# First file app

Rhodium cookbook examples use .NET file-based apps. A file app is a single `.cs` file with package references at the top, then ordinary executable C#.

From `cookbook/GettingStarted` in the [GitHub cookbook folder](https://github.com/HPD-AI/Rhodium/tree/main/cookbook), use this shape:

```csharp
#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0

using Rhodium.Events;
using Rhodium.Platform;
using Rhodium.Platform.Attributes;
using Rhodium.Primitives;
using Rhodium.Simulation;
```

Reference `Rhodium.Simulation` for backtest examples. The package brings in the platform, primitives, events, analytics, and execution simulation dependencies needed by the first-run path.

## Source generators in file apps

Generated strategies require Rhodium's source generator and analyzer to be loaded by the consuming app. The docs repo includes `Directory.Build.props` for this:

```xml
<PackageReference Include="Rhodium.Generators"
                  Version="0.1.0"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false"
                  PrivateAssets="all" />
<PackageReference Include="Rhodium.Analyzers"
                  Version="0.1.0"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false"
                  PrivateAssets="all" />
```

Without those analyzer references, generated hooks, generated fields, readiness flags, order helpers, and generated `[Param]` factories are not emitted. The compiler then reports missing partial implementation parts or missing partial method defining declarations.

## Run the first example

Run the first copyable cookbook:

```text
cookbook/GettingStarted/01-first-backtest.cs
```

From the Rhodium docs repo, run:

```bash
dotnet run cookbook/GettingStarted/01-first-backtest.cs
```

Continue by replacing the filename with the next cookbook in order:

```text
02-first-strategy.cs
03-bar-indicator.cs
04-order-intents.cs
05-parameter-grid.cs
06-venue-config.cs
07-account-seed.cs
08-export-results.cs
```
