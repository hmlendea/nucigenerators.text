# Troubleshooting

| Symptom | Causal investigation | Resolution boundary |
|---------|----------------------|---------------------|
| Build cannot locate solution | Old README commands reference a `.sln`; current [solution](../NuciGenerators.Text.slnx) is `.slnx` | Use [Build commands](./build-and-deployment.md) |
| Tests fail to execute with `--no-build` | Matching test binaries may not have been built for configuration/framework | Build first or omit `--no-build`; use explicit test project |
| Empty batch | Count nonpositive, initial deadline expired, or all reached candidates rejected | Inspect count/budget, exact candidate sequence, current history and filters; see [Generation flow](./flows/generation.md) |
| Fewer names than requested | Count is maximum; repeated candidates or constraints consume cooperative budget | Improve consumer algorithm/filter compatibility; no built-in exhaustion reason exists |
| Seed name still rejected with reuse enabled | `OnlyNewNames=false` skips seeds only, not history | Inspect/explicitly reset history if reuse is intended |
| A regex-looking prefix rejects valid names | Prefix/suffix are literal invariant-culture comparisons | Put regex in inclusion/exclusion lists; see [Acceptance](./behaviour/candidate-acceptance.md) |
| Malformed pattern sometimes throws and sometimes does not | Earlier guards or earlier patterns short-circuit | Reproduce exact input/state; see [Validation flow](./flows/validation.md) |
| Batch exceeds nominal timeout | Algorithm/regex/iteration is not pre-empted | Bound consumer work; budget is not a hard cancellation mechanism |
| Retry skips a name after a failed call | Previous iteration already committed to history | Choose application-level retry/reset policy; no rollback is supplied |
| Changing a word identifier does not change lookup | Dictionary captured the original key | Do not mutate key identity after insertion; no public reindex API exists |
| Schema with different category/casing compares equal | Those fields are excluded from equality and hash | Preserve existing semantics or plan explicit compatibility change |
| Invalid schema assignment succeeds | Attributes are metadata, not setter validation | Invoke annotation validation explicitly in the consumer |
| No rejection logs appear | No built-in logger, reason object, or event | Add safe consumer diagnostics; see [Logging](./logging.md) |

These are source-derived diagnostic paths, not reports of production incidents. No service restart, migration, cache flush, or network health probe is required by the library itself. Consult [Open questions](./ambiguities-and-open-questions.md) for cases whose intended contract remains unsettled.
