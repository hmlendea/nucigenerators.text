# State and persistence

## Ownership ledger

| State | Creation and owner | Mutation and retention | Evidence |
|-------|--------------------|------------------------|----------|
| Outer wordlist list | Consumer passes exact reference to generator | Caller or subclass may modify; retained as long as references exist | [Generation tests](../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) shared-input test |
| Wordlist dictionary | Wordlist creates or materialises it | Insertion via `Add`; no public removal API | [Wordlist source](../NuciGenerators.Text/Models/Wordlist.cs) |
| Word objects and values | Consumer supplies original references | Property replacement visible through all aliases; lazy values can run repeatedly | [Collection tests](../NuciGenerators.Text.UnitTests/Models/WordlistTests.cs) |
| History | Generator initialises fresh list | Appended after each acceptance; cleared in place by reset; publicly mutable | [Generator source](../NuciGenerators.Text/NameGenerator.cs) |
| Batch | One allocation per generation invocation | Independent list of accepted strings; retained by caller | [Generation tests](../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) batch-independence tests |
| Random state | Generator initialises protected random object | Base never consumes it; subclass controls draws; reset does not replace it | [Generator source](../NuciGenerators.Text/NameGenerator.cs) |
| Schema | Consumer creates mutable metadata object | No registration, cache, or automatic validation | [Schema source](../NuciGenerators.Text/Models/GenerationSchema.cs) |

## Commit semantics

Acceptance is an immediate in-memory state transition, not a transaction over the whole batch. A later failure leaves earlier history intact. An unsuccessful call can therefore alter future results. Reset clears history only: it does not undo downstream side effects or release externally retained strings. See [Batch behaviour](./behaviour/batch-and-history.md) and [Mutation flow](./flows/mutation-and-reset.md).

## Persistence boundary

No tracked implementation writes data to disk, database, object storage, or a remote system. No migrations, cache expiry, backup policy, retention period, or cross-process consistency protocol exists. The consuming application owns any such facilities. An in-memory history list is not durable uniqueness enforcement, and garbage collection is not a promised secure-erasure procedure. [Privacy](../PRIVACY.md) is the published policy; [Security mechanics](./security.md) qualifies implementation-level responsibilities.
