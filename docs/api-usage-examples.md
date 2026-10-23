# API usage examples

These examples describe CLR calls, not HTTP exchanges. There are no routes, HTTP bodies/status codes, authentication headers, or rate-limiting policies to illustrate. Use the complete consumer in [Quick start](./quick-start.md) as the construction context.

## Required and excluded patterns

On that example generator, set `NuciGenerators.Text.NameGenerator.StartsWithFilter` to `"Os"`, `IncludedStrings` to `["ar$"]`, and `ExcludedStrings` to `["[0-9]"]`; a request for one result can accept `Oscar`. Affixes are literal; the required pattern is a suffix regex; the exclusion prevents digits. Setting the literal prefix to `"^Os"` does not mean regex start-of-string and will reject these candidates.

## Batch, history, and reset

For a fresh generator from the example:

| Call | Result | History afterwards |
|------|--------|-------------------|
| `NuciGenerators.Text.NameGenerator.Generate(2)` | `Oscar`, `Robert` | `Oscar`, `Robert` |
| `NuciGenerators.Text.NameGenerator.Generate(1)` | `Ilarion` | `Oscar`, `Robert`, `Ilarion` |
| `NuciGenerators.Text.NameGenerator.Reset()` | No result | Empty same history list |
| `NuciGenerators.Text.NameGenerator.Generate(1)` | `Oscar` | `Oscar` |

Earlier returned batch lists remain unchanged by reset. The example's own cycle, not reset, makes its next candidate `Oscar` in this particular sequence.

## Word membership and mutation

Create `NuciGenerators.Text.Models.Word` with identifier `"seed"` and values `["Oscar"]`, then insert it using `NuciGenerators.Text.Models.Wordlist.Add`. Retrieving `"seed"` returns that object. A second insertion with `"seed"` throws `ArgumentException`; retrieving `"missing"` throws `KeyNotFoundException`. Replacing the original object's values with `["Robert"]` changes the sequence seen through the wordlist. Replacing its identifier does not change its original dictionary key.

## Failure after acceptance

The [scripted test adapter](../NuciGenerators.Text.UnitTests/ScriptedNameGenerator.cs) makes this deterministic: enqueue only `Oscar`, then request two. The first iteration accepts `Oscar`; the second dequeue throws `InvalidOperationException`. No batch is returned, but history remains `["Oscar"]`. This is demonstrated by `NuciGenerators.Text.UnitTests.NameGeneratorTests.GivenAnAlgorithmFailureAfterSuccess_WhenGenerating_ThenSuccessfulHistoryIsRetained` in [tests](../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs). The exception type is the adapter's exhaustion mechanism, not a standard library-wide exhaustion error.

## Schema equality and validation

Two `NuciGenerators.Text.Models.GenerationSchema` base instances with equal identifier, name, schema, and filter path compare equal despite different categories or casing. An uninitialised schema validates successfully with the annotation validator because no property is required. Assigning a two-character nonnull identifier makes explicit annotation validation fail; assignment alone does not throw. See [Schema tests](../NuciGenerators.Text.UnitTests/Models/GenerationSchemaTests.cs) and [Schema component](./components/schema-metadata.md).
