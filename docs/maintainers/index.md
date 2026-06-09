# Maintainers

These pages are for contributors changing the Rhodium implementation, not for
announcing release status. Treat the implementation repo as truth:

- Source: `/Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/src`
- Tests: `/Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/test`
- Benchmarks and gates: `/Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/benchmarks` and `/Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci`
- Source status docs: `/Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/docs`

## Pages

- [Repository layout](repository-layout.md): projects, tests, benchmarks, and CI helpers.
- [Source generator](source-generator.md): generated strategy fields, hooks, parameters, and diagnostics.
- [Analyzers](analyzers.md): unsafe access and generated registration diagnostics.
- [Tests as spec](tests-as-spec.md): the local test matrix and memory checks.
- [Benchmarks](benchmarks.md): BenchmarkDotNet coverage and smoke gates.
- [Certification](certification.md): local verifier, reports, and external parity boundaries.
- [Release checklist](release-checklist.md): evidence bundle expectations.

## Ground Rules

Before changing docs, check the relevant source, tests, and status documents.
Keep user docs consistent with `docs/reference/generator-diagnostics.md`,
`docs/reference/public-api-surface.md`, `docs/start/install.md`, and the topic
pages that describe the behavior being changed.

Do not promise broker or venue parity from local tests alone. Do not invent CI,
package publishing, release automation, certification status, or benchmark
commands that are not present in source or status docs.
