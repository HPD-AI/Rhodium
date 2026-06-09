import { defineConfig } from 'vitepress'

const link = (text, path) => ({ text, link: path })
const repositoryName = process.env.GITHUB_REPOSITORY?.split('/')[1]
const base = process.env.GITHUB_ACTIONS && repositoryName ? `/${repositoryName}/` : '/'

const sidebar = [
  {
    text: 'Start Here',
    collapsed: false,
    items: [
      link('Overview', '/'),
      link('Docs Index', '/docs/'),
      link('Install', '/docs/start/install'),
      link('First File App', '/docs/start/first-file-app'),
      link('First Backtest', '/docs/start/first-backtest'),
      link('First Strategy', '/docs/start/first-strategy')
    ]
  },
  {
    text: 'Strategy Authoring',
    collapsed: false,
    items: [
      link('Overview', '/docs/strategy-authoring/'),
      link('Setup Context', '/docs/strategy-authoring/setup-context'),
      link('Generated Fields', '/docs/strategy-authoring/generated-fields'),
      link('Event Hooks', '/docs/strategy-authoring/event-hooks'),
      link('Strategy Lifecycle', '/docs/strategy-authoring/strategy-lifecycle'),
      link('Orders From Contexts', '/docs/strategy-authoring/orders-from-contexts'),
      link('Parameters And Grids', '/docs/strategy-authoring/parameters-and-grids'),
      link('Indicators And Windows', '/docs/strategy-authoring/indicators-and-windows'),
      link('Hot Path Rules', '/docs/strategy-authoring/hot-path-rules')
    ]
  },
  {
    text: 'Simulation',
    collapsed: false,
    items: [
      link('Overview', '/docs/simulation/'),
      link('Simulation Builder', '/docs/simulation/simulation-builder'),
      link('Matching Fidelity', '/docs/simulation/matching-fidelity'),
      link('Fills, Fees, Slippage, Latency', '/docs/simulation/fills-fees-slippage-latency'),
      link('Venues And Instruments', '/docs/simulation/venues-and-instruments'),
      link('Streaming Data', '/docs/simulation/streaming-data'),
      link('Shared History', '/docs/simulation/shared-history'),
      link('Accounts, Margin, Settlement', '/docs/simulation/accounts-margin-settlement'),
      link('Options Lifecycle', '/docs/simulation/options-lifecycle'),
      link('Result Analysis', '/docs/simulation/result-analysis'),
      link('Diagnostics And Frames', '/docs/simulation/diagnostics-and-frames'),
      link('Modules', '/docs/simulation/modules')
    ]
  },
  {
    text: 'Market Model',
    collapsed: true,
    items: [
      link('Overview', '/docs/market-model/'),
      link('Prices, Quantities, Money', '/docs/market-model/prices-quantities-money'),
      link('Instruments And Contracts', '/docs/market-model/instruments-and-contracts'),
      link('Market Events', '/docs/market-model/market-events'),
      link('Bars, Quotes, Trades, Books', '/docs/market-model/bars-quotes-trades-books'),
      link('Asset Variants', '/docs/market-model/asset-variants'),
      link('Orders And Positions', '/docs/market-model/orders-and-positions')
    ]
  },
  {
    text: 'Data And Analytics',
    collapsed: true,
    items: [
      link('Data And Connectivity', '/docs/data-and-connectivity/'),
      link('Simulation Data Plans', '/docs/data-and-connectivity/simulation-data-plans'),
      link('Replay Connectors', '/docs/data-and-connectivity/replay-connectors'),
      link('Aggregators', '/docs/data-and-connectivity/aggregators'),
      link('Routing Policies', '/docs/data-and-connectivity/routing-policies'),
      link('Financing Feeds', '/docs/data-and-connectivity/financing-feeds'),
      link('Data Providers', '/docs/data-and-connectivity/data-providers'),
      link('Analytics', '/docs/analytics/'),
      link('Tear Sheets', '/docs/analytics/tear-sheets'),
      link('Batch Analysis', '/docs/analytics/batch-analysis'),
      link('Exporters', '/docs/analytics/exporters'),
      link('Round Trips', '/docs/analytics/round-trips')
    ]
  },
  {
    text: 'Reference',
    collapsed: true,
    items: [
      link('Reference Index', '/docs/reference/'),
      link('Public API Surface', '/docs/reference/public-api-surface'),
      link('Attributes', '/docs/reference/attributes'),
      link('Strategy Hooks', '/docs/reference/strategy-hooks'),
      link('Execution Spec', '/docs/reference/execution-spec'),
      link('Indicators', '/docs/reference/indicators'),
      link('Simulation Config', '/docs/reference/simulation-config'),
      link('Diagnostics', '/docs/reference/diagnostics'),
      link('Generator Diagnostics', '/docs/reference/generator-diagnostics')
    ]
  },
  {
    text: 'Internals And Maintainers',
    collapsed: true,
    items: [
      link('Engine Internals', '/docs/engine-internals/'),
      link('Architecture', '/docs/engine-internals/architecture'),
      link('Event Ordering', '/docs/engine-internals/event-ordering'),
      link('Market Kernel', '/docs/engine-internals/market-kernel'),
      link('Maintainers', '/docs/maintainers/'),
      link('Repository Layout', '/docs/maintainers/repository-layout'),
      link('Source Generator', '/docs/maintainers/source-generator'),
      link('Analyzers', '/docs/maintainers/analyzers'),
      link('Tests As Spec', '/docs/maintainers/tests-as-spec'),
      link('Benchmarks', '/docs/maintainers/benchmarks'),
      link('Certification', '/docs/maintainers/certification'),
      link('Release Checklist', '/docs/maintainers/release-checklist')
    ]
  }
]

export default defineConfig({
  title: 'Rhodium',
  description: 'Source-backed docs for Rhodium trading simulation and strategy authoring.',
  base,
  srcDir: '.',
  outDir: './.site-dist',
  cacheDir: './.site-cache',
  cleanUrls: true,
  lastUpdated: true,
  ignoreDeadLinks: [],

  themeConfig: {
    logo: '/stock-symbol.svg',

    nav: [
      link('Start', '/docs/start/'),
      link('Strategy', '/docs/strategy-authoring/'),
      link('Simulation', '/docs/simulation/'),
      link('Market Model', '/docs/market-model/'),
      link('Reference', '/docs/reference/'),
      link('Cookbook', 'https://github.com/HPD-AI/Rhodium/tree/main/cookbook')
    ],

    sidebar,

    outline: {
      level: [2, 3],
      label: 'On this page'
    },

    search: {
      provider: 'local'
    },

    footer: {
      message: 'Source-backed Rhodium documentation.',
      copyright: 'Copyright © 2026 HPD AI'
    }
  },

  markdown: {
    theme: {
      light: 'github-light',
      dark: 'github-dark'
    },
    lineNumbers: true
  },

  head: [
    ['link', { rel: 'icon', type: 'image/svg+xml', href: '/stock-symbol.svg' }],
    ['meta', { name: 'theme-color', content: '#166534' }],
    ['meta', { property: 'og:title', content: 'Rhodium' }],
    ['meta', { property: 'og:description', content: 'Trading simulation and strategy docs for Rhodium.' }]
  ]
})
