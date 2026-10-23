# Schema metadata

## Purpose and non-consumers

[Schema source](../../NuciGenerators.Text/Models/GenerationSchema.cs) defines `NuciGenerators.Text.Models.GenerationSchema`. [Casing source](../../NuciGenerators.Text/Models/WordCase.cs) defines `NuciGenerators.Text.Models.WordCase`. The schema stores consumer metadata; neither `NuciGenerators.Text.NameGenerator` nor `INameGenerator` takes a schema argument or reads these fields. `FilterlistPath` does not cause a file read, `Schema` does not execute a grammar, and `WordCase` does not transform output.

## Annotation lifecycle

`Id` has `[Key]` and a length constraint of 3 through 40; `Name` has maximum length 20; `Schema` has maximum length 60. Category, filter path, and casing have no validation attributes. There is no `[Required]`. Uninitialised reference fields are null at runtime and the annotations permit null values. Setters do not run validation.

The caller must invoke an annotation validator explicitly. [Schema tests](../../NuciGenerators.Text.UnitTests/Models/GenerationSchemaTests.cs) use `Validator.TryValidateObject` with all-property validation; that invocation, rather than construction or property assignment, causes invalid lengths to be reported. `[Key]` alone supplies no persistence adapter or automatic identifier generation.

## Equality and hashing

`NuciGenerators.Text.Models.GenerationSchema.Equals(GenerationSchema)` checks null, reference identity, then exact string equality for `Id`, `Name`, `Schema`, and `FilterlistPath`. It ignores `Category` and `WordCase`. `GetHashCode()` combines the same four strings, treating null as zero, using unchecked arithmetic with multiplier 397 and XOR. Equal ordinary instances have equal hashes; a hash is not a stable external identifier and collisions remain possible.

`NuciGenerators.Text.Models.GenerationSchema.Equals(object)` also checks runtime type via the NuciExtensions `NotEquals` call before invoking typed equality. The class is not sealed. Typed equality lacks that runtime-type gate, so inherited types can produce different results depending on overload selection. Existing tests exercise base instances and unrelated object types, not derived-schema equality. Do not claim inheritance-wide equality guarantees from those tests.

All participating properties remain mutable. Mutating them while the schema is a hash-set element or dictionary key can invalidate lookup assumptions. Category/casing mutation leaves this implementation's hash unchanged. This is a consumer lifecycle constraint, not enforced immutability.

## Casing contract and ambiguity

The implicit enum values are `Original=0`, `Lower=1`, `Upper=2`, `Title=3`, and `Sentence=4`; the schema constructor chooses `Sentence`. There is no `Enum.IsDefined` validation, and no production casing implementation establishes the meaning of the last two modes. Their source comments describe first-word versus each-word capitalisation in a way that may conflict with conventional terminology. Preserve numeric values and confirm intended semantics before implementing casing downstream.

## Evidence and change impact

[Schema tests](../../NuciGenerators.Text.UnitTests/Models/GenerationSchemaTests.cs) cover each equality field, ignored fields, nullable combinations, null/unrelated operands, hashes through hash-set deduplication, annotation boundaries, and the constructor default. No serialisation schema or byte format is defined. Changes to equality or enum order can affect consumers even though the base generator never accesses schema metadata. See [Open questions](../ambiguities-and-open-questions.md).
