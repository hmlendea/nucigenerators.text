# Frequently asked questions

## Can this package generate names without additional code?

No. `NuciGenerators.Text.NameGenerator` is abstract and its candidate method has no production implementation here. See [Quick start](./quick-start.md).

## Does a generation schema control the base generator?

No. `NuciGenerators.Text.Models.GenerationSchema` is metadata. Neither schema text, filterlist path, nor casing is interpreted by the base. See [Schema metadata](./components/schema-metadata.md).

## Is the enumerable result lazy?

No for the base implementation. `NuciGenerators.Text.NameGenerator.Generate` constructs a list before returning. An independent interface implementation is not bound by that implementation detail. See [Generator API](./api-reference/generators.md).

## Does reset remove seeds or rewind the algorithm?

No. It only clears current history in place. Settings, seed lists, subclass queues/counters, and random state remain. See [Mutation flow](./flows/mutation-and-reset.md).

## Is the budget a guaranteed maximum execution time?

No. It is a wall-clock deadline checked between attempts. It cannot interrupt algorithm work or regex matching. See [Concurrency](./concurrency-and-scheduling.md).

## Are generated strings guaranteed not to be personal data?

No code-level classification enforces that. Input content and downstream use belong to the consumer; there is no built-in persistence or explicit network transfer. See [Security mechanics](./security.md) and [Policy wording gap](./ambiguities-and-open-questions.md).

## Where are the HTTP endpoints and authentication settings?

They do not exist in this class-library repository. [API reference](./api-reference/INDEX.md) documents CLR contracts instead.

## Why retain the misspelled algorithm method?

`NuciGenerators.Text.NameGenerator.GenerationAlogrithm` is the existing protected abstract signature. Correcting it without compatibility planning breaks subclasses. See [Invariants](./invariants.md).

## Are all documented edge cases demonstrated by tests?

No. [Testing](./testing.md) distinguishes test-backed contracts, source-derived branches, and missing coverage. [Open questions](./ambiguities-and-open-questions.md) records remaining uncertainty rather than treating it as a guarantee.
