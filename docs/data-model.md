# Data model

## Representations and relationships

| Type and source | Fields or representation | Relationships and obligations |
|-----------------|--------------------------|-------------------------------|
| [Word](../NuciGenerators.Text/Models/Word.cs): `NuciGenerators.Text.Models.Word` | Mutable `string Id`, `IEnumerable<string> Values` | Reference type with no custom equality, default initialisation, or validation; values may be lazy |
| [Wordlist](../NuciGenerators.Text/Models/Wordlist.cs): `NuciGenerators.Text.Models.Wordlist` | Dictionary from captured string key to original word object | Constructor materialises membership; values view is live; mutable identifiers do not reindex keys |
| [GenerationSchema](../NuciGenerators.Text/Models/GenerationSchema.cs): `NuciGenerators.Text.Models.GenerationSchema` | `Id`, `Name`, `Category`, `Schema`, `FilterlistPath`, `WordCase` | Mutable metadata; no association to wordlists is enforced |
| [WordCase](../NuciGenerators.Text/Models/WordCase.cs): `NuciGenerators.Text.Models.WordCase` | Integer-backed enum, values 0 through 4 | Metadata only; no casing transformation supplied |
| Generator history | `List<string>` | Exact string duplicate suppression across batches until reset or external mutation |
| Batch result | Separate `List<string>` exposed as enumerable | Accepted order for one successful call; no link to later history clearing |

## Schema fields

| Fully qualified property | Constraint or meaning | Base-generator consumer |
|--------------------------|-----------------------|-------------------------|
| `NuciGenerators.Text.Models.GenerationSchema.Id` | Key annotation; length 3 through 40 when nonnull | None |
| `NuciGenerators.Text.Models.GenerationSchema.Name` | Maximum length 20 when nonnull | None |
| `NuciGenerators.Text.Models.GenerationSchema.Category` | Unconstrained categorisation string | None |
| `NuciGenerators.Text.Models.GenerationSchema.Schema` | Maximum length 60 when nonnull; uninterpreted text | None |
| `NuciGenerators.Text.Models.GenerationSchema.FilterlistPath` | Uninterpreted path string; no normalisation or read | None |
| `NuciGenerators.Text.Models.GenerationSchema.WordCase` | Defaults to enum value `Sentence` | None |

Annotations require explicit validation. There is no required-field attribute, ORM mapping implementation, serialisation configuration, JSON naming contract, migration script, or persistence store in tracked production source.

## Equality and lifecycle

Words use ordinary reference identity unless a consumer provides other comparison logic. Dictionary membership uses string keys; seed rejection uses string values. Schema equality uses four strings, not category/casing, and its object overload adds a runtime-type restriction. Hash-relevant fields must not be mutated while used as hashed keys unless the consumer removes and reinserts the object. See [Schema metadata](./components/schema-metadata.md) and [Word collections](./components/word-collections.md) for overload and aliasing details.

Verified boundaries are indexed in [Testing](./testing.md). A change to any representation can affect external clients despite the lack of a database or wire format.
