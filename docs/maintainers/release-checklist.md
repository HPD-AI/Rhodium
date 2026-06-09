# Release Checklist

Current source evidence supports a certification evidence workflow. The public
docs and cookbooks are staged for the first NuGet consumption path at package
version `0.1.0`.

## Where To Look

Use the implementation repo scripts and status docs:

- `eng/ci/verify-rhodium.cs`
- `eng/ci/certify-rhodium-release.cs`
- `docs/Rhodium.CertificationStatus.md`
- `docs/Rhodium.TargetExternalCertificationRunbook.md`

Required retained local artifacts are:

- `vector-smoke-report.json`
- `replay-certification-smoke.json`
- `rhodium-certification-manifest.json`
- external parity artifacts in the same evidence bundle

## Checks Before Release Evidence Claims

Strict verifier shape:

```bash
dotnet run /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci/verify-rhodium.cs --keep-reports --require-clean-git --require-target-hardware --external-parity-manifest <path> --require-external-parity --require-release-evidence --report-dir <dir>
```

Wrapper:

```bash
dotnet run /Users/ewoof/Desktop/HPD-OS/HPD-AI-Framework/dotnet/shared/src/Rhodium/eng/ci/certify-rhodium-release.cs -- --spec <PATH> --report-dir <PATH>
```

The wrapper builds `external-parity-manifest.json` from the spec, then runs the
strict verifier. Release evidence validation requires clean tracked git, retained
reports, target hardware, external parity, explicit `--report-dir`, the external
parity manifest inside that report directory, and no skipped local gates.

## Keep User Docs Consistent

Update certification and install docs if release evidence requirements or
project consumption paths change. Keep public claims tied to retained reports
and the exact proof level that passed: local certification, target-hardware
certification, or named broker/venue parity certification.

## NuGet 0.1.0 Docs Contract

Before publishing `0.1.0`, keep the package names and versions aligned across:

- `Directory.Build.props`
- `cookbook/**/*.cs` file-app `#:package` directives
- `docs/start/install.md`
- `docs/start/first-file-app.md`

The current public examples expect these package IDs:

```text
Rhodium.Simulation@0.1.0
Rhodium.Data@0.1.0
Rhodium.Generators 0.1.0
Rhodium.Analyzers 0.1.0
```

If the published package IDs differ, update the docs before release.

## Do Not Promise

Do not claim signed artifacts, publication scripts, or a long-term semantic
version policy until those are present in source evidence. Project files show
target frameworks and dependencies, but release automation, package
license/readme/tags, and publication scripts still need explicit evidence.
