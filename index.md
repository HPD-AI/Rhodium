---
layout: home
---

<section class="rh-terminal-hero">
  <div class="rh-hero-copy">
    <p class="rh-eyebrow">Rhodium docs</p>
    <h1>Replay markets. Inspect fills. Ship strategies with evidence.</h1>
    <p>
      A source-backed documentation site for Rhodium's .NET trading simulation stack:
      file-based apps, generated strategy hooks, deterministic replay, matching fidelity,
      market models, analytics, and release proof.
    </p>
    <div class="rh-hero-actions">
      <a class="rh-primary-action" href="/docs/start/">Start with a file app</a>
      <a class="rh-secondary-action" href="/docs/simulation/matching-fidelity">Compare matching modes</a>
    </div>
  </div>

  <div class="rh-market-console" aria-label="Rhodium replay console preview">
    <div class="rh-console-top">
      <span>$RH SIM</span>
      <span>QUEUE ACCURATE</span>
      <span>RUN 01</span>
    </div>
    <div class="rh-console-grid">
      <div>
        <span class="rh-console-label">Replay</span>
        <strong>8 events</strong>
        <small>bars, trades, orders</small>
      </div>
      <div>
        <span class="rh-console-label">Fill</span>
        <strong>123.00</strong>
        <small>accepted, filled</small>
      </div>
      <div>
        <span class="rh-console-label">Evidence</span>
        <strong>CSV</strong>
        <small>metrics, account rows</small>
      </div>
    </div>
    <svg class="rh-replay-chart" viewBox="0 0 520 180" role="img" aria-label="Candlestick replay chart with order and fill markers">
      <defs>
        <linearGradient id="rhChartFill" x1="0" x2="0" y1="0" y2="1">
          <stop offset="0" stop-color="currentColor" stop-opacity="0.18" />
          <stop offset="1" stop-color="currentColor" stop-opacity="0.02" />
        </linearGradient>
      </defs>
      <g class="rh-chart-grid">
        <path d="M24 36H496M24 78H496M24 120H496M24 162H496" />
        <path d="M72 20V166M144 20V166M216 20V166M288 20V166M360 20V166M432 20V166" />
      </g>
      <path class="rh-chart-area" d="M28 133L70 116L112 126L154 92L196 102L238 76L280 84L322 58L364 68L406 44L492 51V166H28Z" />
      <path class="rh-chart-line" d="M28 133L70 116L112 126L154 92L196 102L238 76L280 84L322 58L364 68L406 44L492 51" />
      <g class="rh-candles">
        <path d="M68 82V138" /><rect x="60" y="104" width="16" height="26" />
        <path d="M140 72V132" /><rect x="132" y="86" width="16" height="34" />
        <path d="M212 65V121" /><rect x="204" y="88" width="16" height="22" />
        <path d="M284 54V112" /><rect x="276" y="66" width="16" height="30" />
        <path d="M356 42V103" /><rect x="348" y="60" width="16" height="27" />
        <path d="M428 36V96" /><rect x="420" y="48" width="16" height="31" />
      </g>
      <g class="rh-fill-marker">
        <path d="M312 48L330 30L348 48" />
        <circle cx="330" cy="58" r="6" />
        <text x="352" y="63">fill 123.00</text>
      </g>
      <g class="rh-volume">
        <rect x="54" y="148" width="22" height="18" />
        <rect x="126" y="140" width="22" height="26" />
        <rect x="198" y="152" width="22" height="14" />
        <rect x="270" y="132" width="22" height="34" />
        <rect x="342" y="124" width="22" height="42" />
        <rect x="414" y="136" width="22" height="30" />
      </g>
    </svg>
    <pre><code>var result = Rhodium.Simulation.Rhodium
    .Simulate&lt;MyStrategy&gt;()
    .WithHistory(history)
    .WithMatchingFidelity(MatchingFidelity.QueueAccurate)
    .Run();</code></pre>
    <div class="rh-tape">
      <span>SPY 123.00</span>
      <span>BUY 1</span>
      <span>latency 10ms</span>
      <span>fee maker</span>
    </div>
  </div>
</section>

<section class="rh-signal-strip">
  <a href="/docs/start/first-file-app">
    <svg viewBox="0 0 32 32" aria-hidden="true"><path d="M7 6h18v20H7z" /><path d="M11 11h10M11 16h10M11 21h6" /></svg>
    <span>01</span>
    <strong>Run one `.cs` file</strong>
    <small>NuGet `0.1.0`, no project ceremony.</small>
  </a>
  <a href="/docs/strategy-authoring/">
    <svg viewBox="0 0 32 32" aria-hidden="true"><path d="M7 23c5-14 13-14 18 0" /><path d="M10 17h12M16 9v16" /></svg>
    <span>02</span>
    <strong>Author hooks</strong>
    <small>Setup, generated fields, contexts, orders.</small>
  </a>
  <a href="/docs/simulation/">
    <svg viewBox="0 0 32 32" aria-hidden="true"><path d="M6 17h20" /><path d="M10 12l-4 5 4 5M22 12l4 5-4 5" /><circle cx="16" cy="17" r="3" /></svg>
    <span>03</span>
    <strong>Replay with fidelity</strong>
    <small>Fast vector or queue-accurate matching.</small>
  </a>
  <a href="/docs/analytics/">
    <svg viewBox="0 0 32 32" aria-hidden="true"><path d="M7 24h18" /><rect x="9" y="14" width="3" height="10" /><rect x="15" y="8" width="3" height="16" /><rect x="21" y="11" width="3" height="13" /></svg>
    <span>04</span>
    <strong>Export the proof</strong>
    <small>Tear sheets, round trips, CSV evidence.</small>
  </a>
</section>

<section class="rh-ledger-section">
  <div>
    <p class="rh-eyebrow">What Rhodium is documenting</p>
    <h2>A trading system is only useful when the replay can explain itself.</h2>
    <p>
      The docs follow the same questions a strategy run creates: what data entered
      the world, what hooks fired, how orders became executions, how account state
      changed, and which source or test backs the behavior.
    </p>
  </div>
  <div class="rh-ledger">
    <a href="/docs/market-model/market-events">
      <span>market event</span>
      <strong>BarClosed, TradeOccurred, QuoteUpdated, BookUpdated</strong>
    </a>
    <a href="/docs/strategy-authoring/event-hooks">
      <span>strategy hook</span>
      <strong>OnBar, OnTrade, OnQuote, OnOrderFilled, OnTimer</strong>
    </a>
    <a href="/docs/reference/execution-spec">
      <span>execution spec</span>
      <strong>Intent, acceptance, modification, cancellation, fill</strong>
    </a>
    <a href="/docs/maintainers/tests-as-spec">
      <span>source evidence</span>
      <strong>Tests, generators, analyzers, certification reports</strong>
    </a>
  </div>
</section>

<section class="rh-route-board">
  <div class="rh-route-header">
    <p class="rh-eyebrow">Choose the next desk</p>
    <h2>Every section maps to a simulation surface.</h2>
  </div>
  <div class="rh-route-grid">
    <a href="/docs/data-and-connectivity/">
      <span>Data</span>
      <strong>Plans, replay sources, aggregators, routing policies, providers.</strong>
    </a>
    <a href="/docs/simulation/venues-and-instruments">
      <span>Venues</span>
      <strong>Instruments, accounts, matching settings, settlement calendars.</strong>
    </a>
    <a href="/docs/reference/indicators">
      <span>Indicators</span>
      <strong>Generated readiness flags, streaming windows, bar-derived fields.</strong>
    </a>
    <a href="/docs/engine-internals/">
      <span>Internals</span>
      <strong>Runtime, world state, market kernel, dispatch, tensor store.</strong>
    </a>
  </div>
</section>

<section class="rh-cookbook-callout">
  <div>
    <p class="rh-eyebrow">Executable companion</p>
    <h2>Cookbooks are the first API surface.</h2>
    <p>
      Open the GitHub cookbook folder, start with `cookbook/GettingStarted/01-first-backtest.cs`, then move through
      indicators, order intents, parameter grids, venue config, account seeds, and exports.
    </p>
  </div>
  <a class="rh-path-button" href="https://github.com/HPD-AI/Rhodium/tree/main/cookbook">Open cookbook</a>
</section>
