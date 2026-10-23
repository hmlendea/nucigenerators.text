# Concurrency and scheduling

## Execution model

[Generator source](../NuciGenerators.Text/NameGenerator.cs) contains a synchronous while loop. There is no task scheduler, timer callback, queue worker, async method, lock, cancellation token, or disposal lifecycle. The host thread executes subclass code, value enumeration, regex evaluation, history mutation, and result construction in sequence.

An individual instance is not protected for concurrent generation/reset/configuration. Two generators can also share the same outer seed list and mutable word objects. Separate instances isolate their default filter/history lists, but not explicitly shared inputs. A consumer needing parallelism must isolate mutable state or synchronise all relevant access, including external aliases. This is a recommendation, not a lock implemented in the library.

## Re-entrancy

The base calls arbitrary subclass code before validation. No guard prevents that code from recursively invoking generation, resetting history, or changing settings. Likewise, custom seed enumerables can execute consumer logic during validation. The ordinary uniqueness and ordering explanation assumes such callbacks do not mutate the operation re-entrantly. No tests establish re-entrant semantics.

## Deadline semantics

`NuciGenerators.Text.NameGenerator.Generate` samples `DateTime.Now`, computes `startTime.AddMilliseconds(MaxProcessingTimePerWord * maximumCount)`, and compares the latest sample at loop entry. Time is sampled again only after the whole candidate iteration. Wall-clock adjustment can alter duration; the clock is not monotonic. Budget multiplication is integer arithmetic, with no explicit checked block or range validation.

The budget is for the requested batch size, not separately reset for every candidate or accepted word. A slow candidate may be accepted after the nominal deadline. Zero budget is timing-dependent. A subclass that blocks forever or a pathological regex is not interrupted. [Expired-budget tests](../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) verify negative initial budgets, not hard timeout enforcement.

## Cost model

For each attempt, history membership scans up to current history size; seed novelty can enumerate all seed values; regex work depends on candidate and pattern complexity. A successful acceptance increases future history scan cost. History has no built-in bound, and contradictory filters can consume the entire cooperative budget without results. Static regex APIs may use runtime caching, but this library does not own a compiled-pattern cache or set a per-call timeout.

See [Troubleshooting](./troubleshooting.md) for symptoms and [Open questions](./ambiguities-and-open-questions.md) for coverage gaps. No performance benchmark or safe capacity limit is established.
