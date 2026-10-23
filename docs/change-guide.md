# Change guide

## Modification impact matrix

| Intended change | Owning implementation | Coupled contracts and verification | Documentation to revise |
|-----------------|-----------------------|-----------------------------------|-------------------------|
| New candidate algorithm | Consumer subclass; repository test seam is [ScriptedNameGenerator](../NuciGenerators.Text.UnitTests/ScriptedNameGenerator.cs) | Protected spelling, synchronous invocation, accepted/rejected retries, exhaustion choice | Consumer docs, [Quick start](./quick-start.md), [Generation component](./components/generation-engine.md) if shared contract changes |
| Acceptance rule or comparer | [NameGenerator](../NuciGenerators.Text/NameGenerator.cs) | Validation order, syntax-error visibility, Unicode and exact duplicate semantics; validation and batch fixtures | [Acceptance](./behaviour/candidate-acceptance.md), [Validation flow](./flows/validation.md), [Invariants](./invariants.md) |
| Budget or clock implementation | Same source, generation method | Setup before count guard, between-iteration checks, late acceptance, overflow, zero/negative budgets | [Concurrency](./concurrency-and-scheduling.md), [Batch behaviour](./behaviour/batch-and-history.md) |
| History collection or reset | Same source, properties and reset | Public list type, identity, independent results, partial-failure retention | [State](./state-and-persistence.md), [Mutation flow](./flows/mutation-and-reset.md), [Generator API](./api-reference/generators.md) |
| Wordlist storage or key policy | [Wordlist](../NuciGenerators.Text/Models/Wordlist.cs) | Duplicate/null failure types, captured keys, original object identity, live values view | [Word collections](./components/word-collections.md), [Data model](./data-model.md), [Model API](./api-reference/models.md) |
| Schema equality or annotations | [GenerationSchema](../NuciGenerators.Text/Models/GenerationSchema.cs) | Equality/hash field agreement, nullable validation, inherited overload behaviour | [Schema component](./components/schema-metadata.md), [Model API](./api-reference/models.md) |
| Casing interpretation | [WordCase](../NuciGenerators.Text/Models/WordCase.cs), plus any new consumer | Numeric values, default, existing ambiguous comments; no current transformation to preserve | [Open questions](./ambiguities-and-open-questions.md), [Schema component](./components/schema-metadata.md) |
| Public property or method | [Interface](../NuciGenerators.Text/INameGenerator.cs) and [base](../NuciGenerators.Text/NameGenerator.cs) | Interface/base alignment, all downstream implementers/subclasses, compile compatibility | [API reference](./api-reference/INDEX.md), [Configuration](./configuration.md), root [README](../README.md) |
| Dependency, SDK, or CI | [Library manifest](../NuciGenerators.Text/NuciGenerators.Text.csproj), [test manifest](../NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj), [workflow](../.github/workflows/dotnet.yml) | Restore/build/discovery, package compatibility and production/test dependency direction | [Dependencies](./dependencies.md), [Build](./build-and-deployment.md), root [Architecture](../ARCHITECTURE.md) |

## Safe procedure

1. Identify the behaviour and its observable state, including externally retained references.
2. Read the implementation, corresponding test methods in [Testing](./testing.md), and [Invariants](./invariants.md). Treat source-versus-test gaps explicitly.
3. Add a regression test demonstrating the intended changed or preserved contract; do not weaken unrelated assertions to force a pass.
4. Run the focused fixture, then the full test project and a library build with matching configuration.
5. Review public/protected API, failure ordering, mutation visibility, and consumer-owned boundaries.
6. Revise the affected pages and [index review metadata](./INDEX.md); assess root documentation only where its claims change.

Do not silently correct `GenerationAlogrithm` spelling, replace list properties with readonly abstractions, include category/casing in equality, or make invalid inputs harmless. Each may look like cleanup but changes externally visible contracts. No architecture fitness tests currently enforce these rules automatically.
