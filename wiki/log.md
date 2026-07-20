# Log

Append-only chronological log. Each entry is one dated line using:

`## [YYYY-MM-DD] <type> | <what happened> | <outcome>`

## [2026-07-09] setup | Created LLM-maintained wiki skeleton, schema, hooks, hot-page generator, and Obsidian graph config | Wiki is ready for future sessions
## [2026-07-09] setup | Initialized git repository for the workspace | Wiki files can now be committed and sync hooks can operate once a remote is configured
## [2026-07-09] setup | Configured GitHub remote git@github.com:hqvvny/codex.git | Push is blocked until this machine has a GitHub-authenticated SSH key
## [2026-07-09] setup | Pushed main branch to GitHub remote hqvvny/codex | Repository now tracks origin/main
## [2026-07-09] setup | Moved GitHub SSH key to ~/.ssh/id_ed25519 and verified authentication | Normal git push can now work from hooks
## [2026-07-09] preference | Captured trading-analysis defaults and reasoning rules | Future market work should lead with structure, sessions, liquidity, edge, R:R, invalidation, and provenance
## [2026-07-09] preference | Captured two-stage strategy research pipeline and AI trust gates | Future strategy work separates candidate generation from build/test and requires review before capital exposure
## [2026-07-10] infrastructure | Created strategy queue, candidate workflow, hypothesis template, first filter, and backtest battery | Research pipeline now has an operating layer for Stage 1 and Stage 2 handoff
## [2026-07-10] filter | Ran first filter on MNQ-001 opening range liquidity sweep hypothesis | Verdict is needs-definition before build/test because data source, R:R, and exact rules are not final
## [2026-07-10] artifact | Created MNQ-001 definition worksheet for user input | Next step is filling rule, data, cost, and invalidation fields before build/test
## [2026-07-10] abandonment | Moved MNQ-001 opening range liquidity sweep to Failed Ideas | User rejected it as low-quality/not worth pursuing; do not build this generic setup
## [2026-07-10] constraint | Recorded no paid data budget and zero-budget data ladder | Future research must use free/account-included/exported data first and label data limitations clearly
## [2026-07-10] constraint | Added MotiveWave to the zero-budget data/execution stack | MotiveWave export and Data Export Groups should be tested as high-priority data path
## [2026-07-10] data | Registered local NQU6 1m RTH CSV as DATA-001 | Dataset spans 2017-04-17 to 2026-07-10 with 1,006,460 valid rows; timezone, source platform, and contract-roll handling still need confirmation
## [2026-07-10] infrastructure | Built stdlib OHLCV CSV ingestor and ran it on DATA-001 | Normalized local CSV was generated under ignored data/processed and profile metadata was written to outputs/DATA-001-profile.json
## [2026-07-10] infrastructure | Built session-aware DATA-001 loader and summary script | Unit tests pass and outputs/DATA-001-session-summary.csv records descriptive session stats
## [2026-07-10] data | Confirmed DATA-001 MotiveWave export settings from screenshot | Study export checkbox was off; Big Trades study export requires a small comparison export before assuming availability
## [2026-07-10] reminder | Deferred MotiveWave Big Trades study export comparison | Reminder suggested for 2026-07-11 10:00 Europe/Berlin; continue simple data work for now
## [2026-07-10] analysis | Built DATA-001 time-of-day profile | First RTH/session hour dominates average 1m range; artifacts are descriptive only, not strategy results
## [2026-07-10] analysis | Built DATA-001 range-build profile | First 30/60/120 session minutes average 46.35%/59.10%/71.80% of full RTH range, descriptive only
## [2026-07-10] data | Clarified DATA-001 does not include full ETH/overnight coverage | Separate all-sessions/ETH export is required for overnight, London, Asia, gap, or prior overnight high/low research
## [2026-07-10] reminder | Added second reminder for MotiveWave ETH/all-sessions export | Reminder suggested for 2026-07-11 10:15 Europe/Berlin so DATA-002 can cover overnight context
## [2026-07-10] baseline | Tested MNQ-002 open-long 15:30 to 16:00 thesis on DATA-001 | Gross positive but weak: PF 1.065 exact local-clock, poor year consistency, no R:R because no stop/target
## [2026-07-11] reminder | Added MNQ-002 filter follow-up reminder | Reminder suggested for 2026-07-11 16:00 Europe/Berlin; next work should test structural filters, not optimize raw time entry
## [2026-07-11] data | Registered and normalized DATA-002 NQU6 1m ETH/all-sessions export | Dataset spans 2017-04-17 to 2026-07-10 with 3,106,544 valid rows and no OHLCV sanity errors
## [2026-07-11] filter | Tested MNQ-002 with overnight-negative filter | Overnight-negative improves gross avg/PF versus non-negative but remains weak before costs and not strategy-grade
## [2026-07-11] artifact | Created TradingView Strategy Tester script for MNQ-002 | Pine strategy supports all/overnight-negative/overnight-non-negative filters for visual review, with TradingView fill-model caveat
## [2026-07-11] artifact | Created NinjaTrader 8 Strategy Analyzer script for MNQ-002 | NT8 strategy supports timed RTH-open long testing with all/overnight-negative/overnight-non-negative filters
## [2026-07-11] fix | Clarified MNQ-002 NinjaTrader script must be used as C# NinjaScript | Removed C# region directives and documented that `.js` editors will reject the NT8 strategy code
## [2026-07-11] platform | Identified user's NinjaTrader environment as Web/JavaScript rather than NT8 Strategy Analyzer | Next Ninja-side artifact should be a JavaScript visual marker indicator, while Python remains the backtest source of truth
## [2026-07-11] analysis | Parsed first MNQ-002 NinjaTrader Strategy Analyzer exports | All-style run matches Python baseline, but two exports are identical and the available filtered run is worse, so the three-filter comparison is incomplete
## [2026-07-11] analysis | Added confirmed MNQ-002 NT8 overnight-negative export | Negative overnight filter carries most of the edge with PF 1.12 and +2.65 NQ points/trade, while non-negative remains weak
## [2026-07-11] analysis | Reviewed MNQ-002 overnight-negative hold-time screenshots | 15 bars has the cleanest risk-adjusted profile, 60 bars has the highest net profit, and 45 bars is unattractive due to long recovery
## [2026-07-11] implementation | Added R:R bracket exits to MNQ-002 NinjaTrader strategy | Strategy can now test TimedOnly, BracketOnly, or BracketWithTimeStop using StopLossPoints and RiskReward parameters
## [2026-07-11] fix | Added simplified MNQ-002 RR NinjaTrader strategy fallback | New class avoids custom enums and display/range attributes after NT8 reported many CS0121/CS0229/CS101/CS111-style compile errors
## [2026-07-11] analysis | Built and ran Python MNQ-002 R:R grid | Conservative stop-first bracket tests do not improve the timed-exit baseline; target-first results show high intrabar-fill sensitivity
## [2026-07-20] candidate | Captured MNQ-003 1m 200 EMA retest continuation thesis | Hypothesis is raw and needs mechanical retest, stop, target, cooldown, and session rules before build/test
## [2026-07-20] analysis | Ran MNQ-003 1m 200 EMA retest first Python backtest | Raw full-RTH 1R version fails under conservative stop-first fills with PF 0.957 and high intrabar-fill sensitivity
## [2026-07-20] implementation | Added MNQ-003 NinjaTrader 8 visual backtest strategy | Strategy mirrors the 200 EMA retest rule with candle-extreme plus 2-point stop buffer and 1R target based on entry fill
## [2026-07-20] analysis | Measured MNQ-003 EMA retest depth | Selected retests average about 5.36 points from EMA, but true EMA penetration occurs in only 8.74% of signals and averages 1.87 points when it happens
