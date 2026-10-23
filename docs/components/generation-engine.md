# Generation engine

## Ownership and source

[Base implementation](../../NuciGenerators.Text/NameGenerator.cs): `NuciGenerators.Text.NameGenerator` implements [the interface](../../NuciGenerators.Text/INameGenerator.cs). The caller supplies a subclass because the base cannot manufacture a candidate. The interface permits independent implementations; it does not enforce the base class's behavioural rules.

## Construction and extension contract

The primary constructor retains the supplied list as `NuciGenerators.Text.NameGenerator.Wordlists`. Each instance initialises independent history and filter lists, default properties, and a protected readonly `System.Random` object. The base itself never draws random values. Subclasses decide whether to use that field, their own randomness, deterministic data, files, or services; none of those choices is implied by inheritance.

The protected abstract `NuciGenerators.Text.NameGenerator.GenerationAlogrithm()` spelling is part of the subclass contract. It returns one candidate per invocation. A candidate can be rejected without being returned to the caller. There is no exhaustion signal, completion flag, asynchronous callback, or cancellation argument. Throwing aborts the batch; returning null or whitespace merely fails acceptance and can cause another attempt.

## Orchestration and causality

`NuciGenerators.Text.NameGenerator.Generate(int)` allocates a fresh batch list, reads the wall clock twice, then computes a deadline before evaluating whether the requested count is positive. The multiplication of budget and count occurs as integer arithmetic before `AddMilliseconds` receives it. There is no argument normalisation or overflow guard in this method.

The loop continues only while both the accepted count is below the request and the last sampled time is at or before the deadline. It invokes the subclass, calls `NuciGenerators.Text.NameGenerator.IsNameValid`, and appends accepted strings to the batch followed by history. The current time is refreshed after validation. Consequently, expensive algorithm work or validation can cross the deadline and still commit its candidate: the deadline does not pre-empt an iteration.

An ordinary rejection retries by requesting the next candidate, not by rerunning the same input explicitly. An exception escapes, so there is no completed batch result even if earlier strings were accepted. History remains committed. See [Generation flow](../flows/generation.md) and [Error handling](../error-handling.md).

## Validation ownership

`NuciGenerators.Text.NameGenerator.IsNameValid` is protected, nonvirtual, and reads current properties each time. It does not append to history or modify a batch. Its ordered guards preserve early rejection before expensive regex evaluation, but they also mean invalid settings can remain latent until a candidate reaches the affected branch. For enumerable seed values, user-provided enumeration itself may have side effects or throw; validation is not an isolation boundary around arbitrary input providers.

`NuciGenerators.Text.NameGenerator.Generate` and `Reset` are nonvirtual. Independent interface implementation is the substitution point if a different orchestration contract is required. Hiding members in a subclass is not equivalent to changing calls made through the base contract.

## Tests and modification impact

[Batch tests](../../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) demonstrate invocation counts, independent batch lists, accumulated history, expired budgets, reset identity, exception propagation, and shared wordlists. [Validation tests](../../NuciGenerators.Text.UnitTests/NameGeneratorValidationTests.cs) compare individual acceptance classes. [The adapter](../../NuciGenerators.Text.UnitTests/ScriptedNameGenerator.cs) connects both: its public validation method calls the protected base method and its candidate method dequeues a queue.

Changing call ordering affects error visibility, accumulated history, test-adapter invocation counts, and downstream subclasses. Changing timing affects termination, not merely performance. Changing collection types affects public signatures and caller aliasing. Use the [Change guide](../change-guide.md) before any such modification.
