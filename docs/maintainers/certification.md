# Certification

Local certification is automated by
`eng/ci/verify-rhodium.cs`. It proves the checked-out implementation against
in-tree gates; it does not prove broker or venue parity by itself.

## Where To Look

Primary status docs in the implementation repo:

- `docs/Rhodium.CertificationStatus.md`
- `docs/Rhodium.TargetExternalCertificationRunbook.md`

Verifier and helper scripts:

- `eng/ci/verify-rhodium.cs`
- `eng/ci/build-external-parity-manifest.cs`
- `eng/ci/certify-rhodium-release.cs`

Default verifier gates run all 16 test projects, vector smoke, replay
certification smoke, report-contract validation, certification manifest
write/validation, and Rhodium `bin`/`obj` cleanup. Reports are deleted unless
`--keep-reports` is supplied.

## Checks Before Changing Certification

Inspect current options and gates:

```bash
dotnet run /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci/verify-rhodium.cs -- --help
dotnet run /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci/verify-rhodium.cs --list-gates
```

Run and retain local reports:

```bash
dotnet run /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci/verify-rhodium.cs --keep-reports --report-dir artifacts/rhodium-certification
```

Current verifier constants include `10_000` vector variants, `100` bars, `8`
replay certification smoke scenarios, a five-minute vector smoke ceiling, and
`64` logical processors for target-hardware enforcement.

External parity enforcement requires passed fixture kinds:
`TradingCalendar`, `AccountStatement`, `MarginLiquidationFinancing`,
`MarketReplayExecution`, `VenueOrderPolicy`, and `CrossVenueRouting`, with
contained artifacts and SHA-256 digests.

## Keep User Docs Consistent

Keep this page aligned with `docs/Rhodium.CertificationStatus.md`,
`docs/Rhodium.TargetExternalCertificationRunbook.md`, benchmark smoke reports,
and any user-facing certification or diagnostics pages.

## Do Not Promise

Do not say a local verifier pass equals broker certification. External parity
requires named external datasets, retained artifacts, and an accepted parity
manifest. If certification status must be checked manually, say so.
