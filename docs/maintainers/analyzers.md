# Analyzers

Analyzer source lives in `src/Rhodium.Analyzers` and targets `netstandard2.0`.
Current analyzer tests live in `test/Rhodium.Generators.Tests`, not in a
standalone analyzer test project.

## Where To Look

`UnsafeAccessAnalyzer` emits warning `RHD001` when assemblies under `Rhodium.*`,
except `Rhodium.Unsafe`, `Rhodium.Tensor`, and `Rhodium.Kernel`, reference
`Rhodium.Unsafe.*` directly.

`GeneratedRegistrationAnalyzer` emits error `RHD019` when user code invokes
generated registration helpers on a `Strategy`-derived type:

- `__GeneratedRegisterIndicator`
- `__GeneratedRegisterPortfolioField`

Tests asserting these diagnostics are in
`test/Rhodium.Generators.Tests/StrategyGeneratorTests.cs`.

## Checks Before Changing Analyzer Rules

Run:

```bash
dotnet test /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/test/Rhodium.Generators.Tests/Rhodium.Generators.Tests.csproj
```

For unsafe access rule changes, also inspect project references in `src/` so the
exception list still reflects intended layering.

## Keep User Docs Consistent

Update `docs/reference/generator-diagnostics.md` when `RHD001` or `RHD019`
severity, trigger conditions, or fix guidance changes. Keep strategy-authoring
docs aligned with the rule that generated registration helpers are generator
owned.

## Do Not Promise

Do not claim every analyzer rule has its own project-level test suite. Do not
weaken the unsafe boundary in docs unless the source analyzer and tests changed.
