# Rhodium

Rhodium is a .NET trading simulation and strategy framework built around generated strategy hooks, deterministic replay, and source-checked market/execution models.

Start here:

- [Docs index](docs/index.md)
- [Getting started](docs/start/index.md)
- [First file app](docs/start/first-file-app.md)
- [First backtest](docs/start/first-backtest.md)
- [Strategy authoring](docs/strategy-authoring/index.md)

Runnable examples live in [cookbook folder](https://github.com/HPD-AI/Rhodium/tree/main/cookbook). The cookbook uses .NET file-based apps so each example can be copied and run as a single `.cs` file.

## Docs Site

This repo uses VitePress for the local docs website:

```bash
npm install
npm run dev
```

Build output goes to `.site-dist`.
