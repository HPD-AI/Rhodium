# Benchmarks

Benchmark code lives in
`benchmarks/Rhodium.Benchmarks/Rhodium.Benchmarks.csproj`, a `net10.0`
BenchmarkDotNet executable.

## Where To Look

Current benchmark classes:

- `DispatchBenchmarks`: single-strategy sequential, 100-strategy sequential, and
  100-strategy parallel dispatch.
- `VectorSimulationBenchmarks`: event-major vector replay with `VariantCount`
  values `1_000` and `10_000`, and `BarCount` `100`.

Certification smokes are separate from ordinary BenchmarkDotNet runs. The local
verifier runs the benchmark executable with `--vector-smoke` for `10,000`
variants by `100` bars and validates a five-minute ceiling.

## Checks Before Changing Benchmarks

Build helper:

```bash
cd /Users/ewoof/Desktop/HPD-OS
dotnet run HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci/build-benchmarks.cs
```

Dispatch allocation/latency helper:

```bash
cd /Users/ewoof/Desktop/HPD-OS
dotnet run HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci/check-dispatch-allocations.cs
```

The dispatch helper filters `*HundredStrategiesParallel*`, uses a short
BenchmarkDotNet job, fails if mean exceeds `60 us`, and expects no reported
allocation (`0 B` or `-`).

## Keep User Docs Consistent

Update certification docs when smoke dimensions, report fields, or verifier
ceilings change. Update performance-related user docs only when benchmark source
and retained evidence support the statement.

## Do Not Promise

Do not claim target-hardware performance is certified unless
`--require-target-hardware` passed on a 64-logical-processor target or documented
equivalent host with retained reports.
