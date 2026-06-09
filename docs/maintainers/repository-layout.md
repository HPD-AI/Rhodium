# Repository Layout

Use the implementation root:
`/Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium`.

## Where To Look

`src/` contains 18 project directories:

`Rhodium.Analytics`, `Rhodium.Analyzers`, `Rhodium.Connectivity`,
`Rhodium.Control`, `Rhodium.Data`, `Rhodium.Events`, `Rhodium.Generators`,
`Rhodium.HFT`, `Rhodium.Indicators`, `Rhodium.Kernel`, `Rhodium.Options`,
`Rhodium.Platform`, `Rhodium.Primitives`, `Rhodium.Quant`, `Rhodium.Risk`,
`Rhodium.Simulation`, `Rhodium.Tensor`, and `Rhodium.Unsafe`.

`test/` contains 16 xUnit projects matching most runtime areas. There is no
standalone `Rhodium.Analyzers.Tests` or `Rhodium.Unsafe.Tests` project; current
analyzer coverage appears in `Rhodium.Generators.Tests`.

`benchmarks/Rhodium.Benchmarks` is a `net10.0` BenchmarkDotNet executable.
`eng/ci/` contains runnable C# scripts for verification, benchmark build,
dispatch allocation checks, memory leak tests, external parity manifest build,
and release certification.

## Checks Before Changing Layout

Prefer project or script commands. No `.sln`, `.slnx`, or `global.json` was
found under the implementation root.

Useful entry points:

```bash
dotnet run /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci/verify-rhodium.cs
dotnet test /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/test/<Project>/<Project>.csproj
```

## Keep User Docs Consistent

Update `docs/reference/public-api-surface.md` when public project surfaces move.
Update `docs/start/install.md` when file-app references, target frameworks, or
analyzer/generator references change.

## Do Not Promise

Do not claim solution-level build commands, package layout, CI workflow names, or
release packaging until those are present in the implementation repo.
