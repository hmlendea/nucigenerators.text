# Batch and history

## Observable sequence

[Implementation](../../NuciGenerators.Text/NameGenerator.cs): `NuciGenerators.Text.NameGenerator.Generate` returns an already-created list behind an `IEnumerable<string>` signature. Enumerating it does not invoke generation again. Each invocation has its own list; instance history spans calls.

With stable inputs, candidate order determines output order. For candidates `Oscar`, `Oscar`, `Robert` and request two, the first string is committed, its duplicate is rejected, and the second distinct string is committed. The returned batch and history contain `Oscar`, `Robert`. Setting `OnlyNewNames=false` does not change that duplicate rejection.

## Count and time

The target is a maximum accepted count, not an attempt count or a guarantee of completion. Rejections consume time and algorithm calls but not result slots. Deadline exhaustion returns fewer strings without a special status. Nonpositive counts skip the loop after deadline setup. Negative budgets with positive counts normally produce an expired deadline and no algorithm calls, as tested. Zero budget depends on the sampled wall clock and is not a deterministic instruction to execute once.

The timeout is cooperative: a started iteration can finish and append after the nominal deadline. No cancellation is delivered to the subclass or regex engine. See [Concurrency and scheduling](../concurrency-and-scheduling.md).

## Failure and reuse

If the first candidate succeeds and the next algorithm call throws, the caller receives an exception instead of a partial batch. History still contains the successful string. A retry will reject it unless the consumer clears or changes history. `NuciGenerators.Text.NameGenerator.Reset` clears the same history object, not old returned batches, settings, or seed collections. It does not reset subclass counters, queues, random state, or external resources.

Consumers can mutate `GeneratedWords` directly. Uniqueness is consequently a check against current history, not an immutable audit guarantee. The returned list is distinct from history; changing it through a cast does not modify history. Subclasses and custom enumerable code can also introduce side effects outside the base method's control.

## Evidence and dependants

[Batch tests](../../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) assert exact sequences and algorithm call counts, cross-call accumulation, independent results, reset identity, and retained history after a later failure. Changes to these semantics affect consumers implementing retries, caching results, or maintaining a session across batches. [Mutation flow](../flows/mutation-and-reset.md) shows the reference relationships.
