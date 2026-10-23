# Design decisions

## Evidence, not reconstructed intent

No ADR directory or historical design rationale is tracked. The table records observable choices and consequences, not invented author motivations or proposed replacements. The root [Architecture](../ARCHITECTURE.md) remains the topology source; this page explains local trade-offs affecting modifications.

| Observed choice | Evidence | Consequence | Rationale status |
|-----------------|----------|-------------|------------------|
| Shared template method with subclass candidate production | [Generator](../NuciGenerators.Text/NameGenerator.cs) | Validation/history can be reused without prescribing a production algorithm | Separation is implemented; original author rationale not recorded |
| Public interface omits seed-reuse flag | [Interface](../NuciGenerators.Text/INameGenerator.cs) versus base | Interface-only callers cannot set `OnlyNewNames` directly | Intent unknown; do not infer prohibition |
| Mutable aliased inputs | Constructor assignment and [wordlist dictionary](../NuciGenerators.Text/Models/Wordlist.cs) | Live consumer changes influence validation; snapshot boundaries differ | Semantics verified by tests, not documented immutability policy |
| Immediate acceptance commits | Per-iteration batch/history appends | Later exceptions leave accepted history | Transactional rollback is not implemented |
| Wall-clock cooperative deadline | DateTime sampling in generator | Simplicity of loop does not provide pre-emption or monotonic timing | No benchmark or latency guarantee recorded |
| Four-field schema equality | [Schema](../NuciGenerators.Text/Models/GenerationSchema.cs) and [tests](../NuciGenerators.Text.UnitTests/Models/GenerationSchemaTests.cs) | Category/casing changes do not distinguish keys | Intended field selection tested; business rationale not recorded |
| Schema is metadata only | No production generator schema consumer | Higher-level packages must supply interpretation and casing | Capability boundary demonstrated by source |

## When recording a future decision

Distinguish the existing contract, requested change, rationale supplied by maintainers, migration cost, and verification. Link any new ADR from this page rather than presenting a proposal as implemented behaviour. Open ambiguities belong in [Open questions](./ambiguities-and-open-questions.md), not as asserted decisions.
