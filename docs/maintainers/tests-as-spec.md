# Tests As Spec

The in-tree tests are behavioral specs for supported Rhodium semantics. They do
not prove full external broker or venue parity.

## Where To Look

Tests live under
`/Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/test`.
The local verifier runs 16 projects covering primitives, events, tensor, HFT,
kernel, control, platform, generators/analyzers, analytics, simulation,
connectivity, risk, quant, options, indicators, and data.

Memory-specific tests are in
`test/Rhodium.Kernel.Tests/MemoryLeakTests.cs` with
`[Trait("Category", "MemoryLeak")]`.

## Checks Before Changing Behavior

Full local gate:

```bash
dotnet run /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci/verify-rhodium.cs
```

Single project:

```bash
dotnet test /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/test/<Project>/<Project>.csproj
```

Memory helper:

```bash
dotnet run /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci/run-memory-leak-tests.cs
```

The verifier invokes test projects one by one with `dotnet test`, `--nologo`,
`-m:1`, `/nodeReuse:false`, `/clp:ErrorsOnly`, and minimal console logging.

## Keep User Docs Consistent

When tests define or narrow behavior, update the related user topic page and
`docs/reference/public-api-surface.md` if public API semantics changed.
Generator and analyzer test changes may also require
`docs/reference/generator-diagnostics.md`.

## Do Not Promise

Do not describe local tests as venue-grade certification. External parity
requires named broker or exchange datasets and retained comparison evidence.
