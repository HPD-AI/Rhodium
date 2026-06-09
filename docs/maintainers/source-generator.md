# Source Generator

The source generator lives in
`src/Rhodium.Generators/StrategyGenerator.cs` and targets `netstandard2.0`.

## Where To Look

Generator tests live in
`test/Rhodium.Generators.Tests/StrategyGeneratorTests.cs`. They cover generated
fields, hooks, parameters, windows, registration behavior, and the current
diagnostics.

The generator recognizes generated strategy hooks named `OnBar`, `OnTick`,
`OnQuote`, `OnTrade`, `OnBookSnapshot`, `OnBookLevelDelta`, and
`OnBookLevelDeltas`.

Current generator diagnostics in source and tests are:

`RHD002`, `RHD003`, `RHD004`, `RHD005`, `RHD012`, `RHD013`, `RHD014`, `RHD015`,
`RHD016`, `RHD017`, and `RHD018`.

No `RHD006` through `RHD011` definitions were found in the current source.

## Checks Before Changing Generator Behavior

Run the generator test project:

```bash
dotnet test /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/test/Rhodium.Generators.Tests/Rhodium.Generators.Tests.csproj
```

For broader impact, run the local verifier:

```bash
dotnet run /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci/verify-rhodium.cs
```

## Keep User Docs Consistent

Update `docs/reference/generator-diagnostics.md` when diagnostic ids, meanings,
or fixes change. Update strategy-authoring docs and
`docs/reference/public-api-surface.md` when generated hooks, context names,
fields, windows, or parameter factories change.

## Do Not Promise

Do not reserve undocumented diagnostic ids as implemented. Do not describe
generated APIs as manually callable if tests or analyzers require the generator
to own registration.
