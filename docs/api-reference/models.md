# Model API

## Word

[Source](../../NuciGenerators.Text/Models/Word.cs): public sealed `NuciGenerators.Text.Models.Word` has an implicit public parameterless constructor and two public get/set properties:
- `NuciGenerators.Text.Models.Word.Id`: `string`; no default or validation.
- `NuciGenerators.Text.Models.Word.Values`: `IEnumerable<string>`; no default, copying, or materialisation.

Both properties are null until assigned at runtime. No custom equality or hash override is defined. The library does not require a nonempty values sequence at insertion; consuming a null sequence later can fail.

## Wordlist

[Source](../../NuciGenerators.Text/Models/Wordlist.cs): public sealed `NuciGenerators.Text.Models.Wordlist` implements generic and nongeneric enumeration.

| Fully qualified member | Access and signature | Result and failures |
|------------------------|----------------------|---------------------|
| `NuciGenerators.Text.Models.Wordlist.Wordlist()` | Public constructor | Empty keyed collection |
| `NuciGenerators.Text.Models.Wordlist.Wordlist(IEnumerable<Word> words)` | Public constructor | Materialises membership, retains word objects; duplicate/null keys and invalid source elements throw |
| `NuciGenerators.Text.Models.Wordlist.Values` | Public getter, `IEnumerable<Word>` | Live values view, not snapshot |
| `NuciGenerators.Text.Models.Wordlist.Add(Word word)` | Public `void` | Inserts at current identifier; duplicate key throws without replacing original |
| `NuciGenerators.Text.Models.Wordlist.Get(string id)` | Public `Word` | Original stored reference; missing key throws `KeyNotFoundException`, null key throws `ArgumentNullException` |
| `NuciGenerators.Text.Models.Wordlist.GetEnumerator()` | Public `IEnumerator<Word>` | Enumerates values view |
| `NuciGenerators.Text.Models.Wordlist` explicit `IEnumerable.GetEnumerator()` | Explicit nongeneric implementation | Delegates to generic enumerator |

There is no public count, remove, overwrite, clear, or key-reindex method. Empty/whitespace identifiers are permitted. Identifier equality is case-sensitive with no normalisation. Current enumeration order is exercised by tests, but there is no independent ordering abstraction or comparer parameter. See [Word collections](../components/word-collections.md).

## Generation schema

[Source](../../NuciGenerators.Text/Models/GenerationSchema.cs): public nonsealed `NuciGenerators.Text.Models.GenerationSchema` implements `IEquatable<GenerationSchema>`.

All six properties are public get/set: `NuciGenerators.Text.Models.GenerationSchema.Id`, `Name`, `Category`, `Schema`, `FilterlistPath` are strings; `NuciGenerators.Text.Models.GenerationSchema.WordCase` is the casing enum. Their full constraints and data roles are in [Data model](../data-model.md).

| Fully qualified member | Signature | Semantics |
|------------------------|-----------|-----------|
| `NuciGenerators.Text.Models.GenerationSchema.GenerationSchema()` | Public constructor | Sets casing to `Sentence`; leaves strings null |
| `NuciGenerators.Text.Models.GenerationSchema.Equals(GenerationSchema other)` | Public `bool` | Null false; reference identity true; otherwise compare four strings |
| `NuciGenerators.Text.Models.GenerationSchema.Equals(object obj)` | Public override `bool` | Adds equal-runtime-type requirement before typed comparison |
| `NuciGenerators.Text.Models.GenerationSchema.GetHashCode()` | Public override `int` | Hashes same four strings, not category/casing |

No equality operators, schema parser, file loader, persistence adapter, or automatic annotation validator are defined. Overload selection matters for derived classes; see [Schema metadata](../components/schema-metadata.md).

## Word case

[Source](../../NuciGenerators.Text/Models/WordCase.cs): `NuciGenerators.Text.Models.WordCase.Original=0`, `.Lower=1`, `.Upper=2`, `.Title=3`, `.Sentence=4`. Each member is public enum surface. Values are implicit by declaration order; undefined casts are not rejected by a schema setter. No runtime casing function implements these labels, so the enum does not establish transformed output.

## Verification

[Wordlist tests](../../NuciGenerators.Text.UnitTests/Models/WordlistTests.cs) cover collection contracts and [schema tests](../../NuciGenerators.Text.UnitTests/Models/GenerationSchemaTests.cs) cover metadata equality/validation. Dedicated word-construction and enum-number compatibility fixtures are absent. [Testing](../testing.md) records the remaining scope.
