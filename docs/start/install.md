# Install

Rhodium is used from this repo as .NET file-based apps. The verified start path runs cookbook files directly with `dotnet run` and installs Rhodium packages from NuGet.

## Requirements

- .NET SDK with `net10.0` support.
- The Rhodium docs repo checked out locally.
- Rhodium NuGet packages at version `0.1.0`.

Check the SDK:

```bash
dotnet --list-sdks
```

The cookbook files set their target framework at the top:

```csharp
#:property TargetFramework=net10.0
```

## File-app references

Rhodium GettingStarted examples are single `.cs` files under `cookbook/GettingStarted` in the [GitHub cookbook folder](https://github.com/HPD-AI/Rhodium/tree/main/cookbook). Each file references the published package it needs:

```csharp
#:package Rhodium.Simulation@0.1.0
#:property TargetFramework=net10.0
```

Use `Rhodium.Simulation` for the first backtest path. It brings in the platform, primitives, events, analytics, and simulation dependencies needed by the cookbook sequence.

Run a file app by passing the file path to `dotnet run`:

```bash
dotnet run cookbook/GettingStarted/01-first-backtest.cs
```

If you copy a cookbook file elsewhere, keep the package directive at the top and run the copied file the same way:

```bash
dotnet run 01-first-backtest.cs
```

Use the same command shape for the rest of the copyable start sequence:

```text
cookbook/GettingStarted/01-first-backtest.cs
cookbook/GettingStarted/02-first-strategy.cs
cookbook/GettingStarted/03-bar-indicator.cs
cookbook/GettingStarted/04-order-intents.cs
cookbook/GettingStarted/05-parameter-grid.cs
cookbook/GettingStarted/06-venue-config.cs
cookbook/GettingStarted/07-account-seed.cs
cookbook/GettingStarted/08-export-results.cs
```

## Generators and analyzers

Generated hooks, generated fields, indicator readiness flags, and `[Param]` strategy factories require the Rhodium generator and analyzer to be loaded by the consuming app.

The docs repo carries this in `Directory.Build.props`:

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

Keep the file apps under this repo, or provide the same analyzer references in the directory where you run them. Without those references the compiler will not emit Rhodium's generated strategy code.
