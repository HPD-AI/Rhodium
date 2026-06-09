# Reference

These pages summarize the current Rhodium public surface for strategy authors, simulation users, and generator diagnostics. They are compact reference pages; use the workflow docs and cookbook links when you need a worked example.

- [Public API surface](public-api-surface.md): packages, namespaces, and stable user-facing groups.
- [Attributes](attributes.md): generated field, indicator, window, and parameter attributes.
- [Strategy hooks](strategy-hooks.md): generated market hook signatures, generated context behavior, and virtual lifecycle/order hooks.
- [Execution spec](execution-spec.md): `ExecutionSpec`, `Execution`, order types, time-in-force, and execution intent fields.
- [Indicators](indicators.md): indicator interfaces, factories, generated indicator fields, and multi-output caveats.
- [Simulation config](simulation-config.md): `SimulationConfig`, run options, venue/instrument overrides, presets, and model knobs.
- [Diagnostics](diagnostics.md): runtime errors, simulation result diagnostics, modules, frames, and analysis surfaces.
- [Generator diagnostics](generator-diagnostics.md): `RHD###` generator/analyzer diagnostics and fixes.

Examples and workflow guidance live under:

- [Strategy authoring](../strategy-authoring/index.md)
- [Simulation](../simulation/index.md)
- [Market model](../market-model/index.md)
- [Data and connectivity](../data-and-connectivity/index.md)
- [First backtest](../start/first-backtest.md)
