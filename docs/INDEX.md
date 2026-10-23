# Repository knowledge base

NuciGenerators.Text is a .NET 10 library of generation contracts, an extensible candidate-validation loop, mutable word collections, and schema metadata. It does not contain a production candidate algorithm, application host, HTTP service, schema interpreter, or persistence implementation. These documents explain the implemented causal relationships and their tests rather than proposing a replacement architecture.

## Review metadata

- Last reviewed: 2026-10-24.
- Source baseline: `f888d03`; documentation changes follow that revision.
- Documentation owner: repository maintainers; contributors revise the affected pages with their changes.
- Coverage: every production type, all four test fixtures, test adapter, project manifests, solution, and CI workflow inspected. The unit-test project executed successfully: 379 passed, zero failed or skipped, using SDK 10.0.112.
- Limits: no external consumer applications or dependency implementation internals were inspected; hosted release configuration and remote CI execution were not verified.

## Start here

1. Read [Repository overview](./repository-overview.md) for scope and entry points.
2. Follow [Quick start](./quick-start.md) for a deterministic consumer example.
3. Consult [Generation flow](./flows/generation.md) when debugging a result.
4. Use [Change guide](./change-guide.md) and [Invariants](./invariants.md) before editing implementation.

## Root document map

| Resource | Distinct purpose |
|----------|------------------|
| [Root architecture](../ARCHITECTURE.md) | System topology, boundaries, and architecture catalogue |
| [Security policy](../SECURITY.md) | Vulnerability reporting and published support policy |
| [Privacy document](../PRIVACY.md) | Published personal-data description; see the qualification in [Open questions](./ambiguities-and-open-questions.md) |
| [Licence](../LICENSE) | GPL-3.0-or-later terms |
| [README](../README.md) | Package introduction and public usage |
| [Contributing](../CONTRIBUTING.md) | Contribution procedure |
| [Code of conduct](../CODE_OF_CONDUCT.md) | Participation standards |

No root roadmap exists at the reviewed baseline. There is deliberately no second architecture document under this directory: use the root architecture for structural orientation and these pages for implementation-level semantics.

## Documentation catalogue

### Orientation and operations

- [Repository overview](./repository-overview.md): Purpose, scope, callers, and ownership.
- [Repository structure](./repository-structure.md): Source and test navigation.
- [Quick start](./quick-start.md): Deterministic example and expected results.
- [Build and deployment](./build-and-deployment.md): Actual build, verification, package, and CI boundaries.
- [Configuration](./configuration.md): All generator settings, defaults, and evaluation timing.
- [Dependencies](./dependencies.md): Production, BCL, and test dependency roles.
- [Integrations](./integrations.md): Runtime versus tooling and downstream boundaries.

### Components and data

- [Generation engine](./components/generation-engine.md): Candidate orchestration, ordering, and extension obligations.
- [Word collections](./components/word-collections.md): Dictionary membership, object aliases, and enumerable values.
- [Schema metadata](./components/schema-metadata.md): Equality, annotations, casing metadata, and inheritance caveats.
- [Data model](./data-model.md): Field meanings and relationships.
- [State and persistence](./state-and-persistence.md): Ownership, history, reset, and absence of durability.
- [Concurrency and scheduling](./concurrency-and-scheduling.md): Timing, re-entrancy, synchronisation, and resource cost.
- [Design decisions](./design-decisions.md): Evidence-backed implementation choices and non-asserted rationale.

### Behaviours and flows

- [Candidate acceptance](./behaviour/candidate-acceptance.md): Ordered rule matrix, comparisons, Unicode, and regex semantics.
- [Batch and history](./behaviour/batch-and-history.md): Cross-call uniqueness, partial success, and reset.
- [Generation flow](./flows/generation.md): Complete call path and early termination.
- [Validation flow](./flows/validation.md): Short-circuit branches and exception visibility.
- [Mutation and reset flow](./flows/mutation-and-reset.md): Reference sharing and key mutation effects.

### Contracts and verification

- [API reference index](./api-reference/INDEX.md): Public library surface; explains why there are no HTTP endpoints.
- [Generator API](./api-reference/generators.md): Interface, base class, public members, and protected extension seam.
- [Model API](./api-reference/models.md): Constructors, properties, methods, enum, and failure contracts.
- [API usage examples](./api-usage-examples.md): Calls and observable outcomes, not fabricated HTTP requests.
- [Invariants](./invariants.md): Contracts, assumptions, and change-sensitive exceptions.
- [Testing](./testing.md): Fixture and test-symbol map, executed results, and gaps.
- [Error handling](./error-handling.md): Failure taxonomy and partial-state consequences.
- [Security mechanics](./security.md): Trust boundary and implementation risks, not a duplicate policy.
- [Logging](./logging.md): Absent built-in diagnostics and consumer responsibilities.

### Maintenance and problem resolution

- [Change guide](./change-guide.md): Modification impact and focused verification.
- [Troubleshooting](./troubleshooting.md): Symptoms mapped to causes and evidence.
- [FAQ](./faq.md): Common contract questions.
- [Ambiguities and open questions](./ambiguities-and-open-questions.md): Contradictions, unsupported guarantees, and test gaps.
- [Documentation maintenance](./documentation-maintenance.md): Ownership and evidence-refresh procedure.

## Deep dive and debugging navigation

| Question | Start with | Then inspect |
|----------|------------|--------------|
| Why was a string rejected? | [Acceptance](./behaviour/candidate-acceptance.md) | [Validation flow](./flows/validation.md) |
| Why is history nonempty after failure? | [Batch and history](./behaviour/batch-and-history.md) | [Error handling](./error-handling.md) |
| Why did a later input mutation change generation? | [Word collections](./components/word-collections.md) | [Mutation flow](./flows/mutation-and-reset.md) |
| Why is a schema equal despite different metadata? | [Schema metadata](./components/schema-metadata.md) | [Model API](./api-reference/models.md) |
| Which tests must change with a rule? | [Change guide](./change-guide.md) | [Testing](./testing.md) |
