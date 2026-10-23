# Documentation maintenance

## Ownership and scope

Repository maintainers own the knowledge base; each implementation contributor updates affected behaviour and test references. The [master index](./INDEX.md) contains review date, source baseline, execution evidence, and coverage limits. Root [Architecture](../ARCHITECTURE.md), [Security](../SECURITY.md), [Privacy](../PRIVACY.md), and [Licence](../LICENSE) remain authoritative for their distinct scopes; do not copy their principal content into this directory.

## Evidence refresh procedure

1. Start with the [Change guide](./change-guide.md) and the exact component source links, not a whole-repository pattern assumption.
2. Trace callers, ordered branches, state mutations, custom enumerable/subclass callbacks, and propagated failures. Compare test assertions with source rather than trusting method names alone.
3. Revise component, flow, behaviour, API, and invariant pages together when the same contract crosses those boundaries.
4. Run relevant fixtures and then the full test project; record command, SDK, date, actual counts, and limits. Do not transform a successful test run into a coverage or security claim.
5. Verify all relative links from each document's own directory. Preserve fully qualified symbols, kebab-case filenames, sentence-case headings, and source-versus-inference labels.
6. Update [Open questions](./ambiguities-and-open-questions.md) when contradictions are resolved or new uncertainties discovered. Do not invent rationale, policy, endpoints, or deployment practices.
7. Add each new page to [INDEX](./INDEX.md) with a one-line purpose. Maintain the root README's documentation section and assess any affected root architecture or policy section.

## Coverage discipline

No page should become an isolated duplicate of another. The root architecture owns topology; components own local mechanisms; flows own execution ordering; behaviour pages own observable rules; API pages own signatures; tests own evidence mapping. Operational absence, such as no logger or persistence, should be described explicitly rather than populated with generic technology guidance.

Examples should be checked against the public/protected contract and compiled when practical. Document whether such execution was performed. This generation inspected the example against the contract but did not run a separate consumer executable. Future changes to sample syntax, namespace, algorithm exhaustion, or expected output require renewed verification.
