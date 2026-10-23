# Configuration

## Sources and timing

All production settings belong to `NuciGenerators.Text.NameGenerator` in [source](../NuciGenerators.Text/NameGenerator.cs). There is no JSON settings file, environment-variable reader, command-line parser, secret provider, or precedence chain. The consumer assigns public properties directly. An independent `NuciGenerators.Text.INameGenerator` implementation may choose different defaults and semantics.

| Fully qualified member | Type | Base default | When consumed |
|------------------------|------|--------------|---------------|
| `NuciGenerators.Text.NameGenerator.MinNameLength` | `int` | 5 | Each candidate's UTF-16 length comparison |
| `NuciGenerators.Text.NameGenerator.MaxNameLength` | `int` | 10 | Each candidate's UTF-16 length comparison |
| `NuciGenerators.Text.NameGenerator.MaxProcessingTimePerWord` | `int` | 1000 milliseconds | Once during batch deadline calculation, multiplied by requested count |
| `NuciGenerators.Text.NameGenerator.OnlyNewNames` | `bool` | `true` | Each candidate, controlling seed traversal only |
| `NuciGenerators.Text.NameGenerator.StartsWithFilter` | `string` | Empty | Each candidate after length checks |
| `NuciGenerators.Text.NameGenerator.EndsWithFilter` | `string` | Empty | Each candidate after length checks |
| `NuciGenerators.Text.NameGenerator.ExcludedStrings` | `List<string>` | Fresh empty list | Each candidate reaching regex exclusion |
| `NuciGenerators.Text.NameGenerator.IncludedStrings` | `List<string>` | Fresh empty list | Each candidate reaching regex inclusion |
| `NuciGenerators.Text.NameGenerator.Wordlists` | `List<Wordlist>` | Exact constructor argument | Subclass use and seed novelty checks |
| `NuciGenerators.Text.NameGenerator.GeneratedWords` | `List<string>` | Fresh empty list | Duplicate checks and immediate acceptance recording |

The first eight settings have public setters. The final two properties have protected setters but expose publicly mutable lists. `OnlyNewNames` is absent from [the interface](../NuciGenerators.Text/INameGenerator.cs). No setting requires a restart; changing the budget after deadline calculation does not recompute the current batch's deadline, whereas later validation reads current filter values.

## Invalid and conflicting configuration

There is no configuration validator. A minimum greater than maximum makes all nonblank candidates fail length validation. Negative bounds are compared literally. Null strings or collections can throw only when their branch is reached. Regex syntax errors can remain masked by earlier rejection. Request count and budget multiplication can overflow under the ordinary unchecked build configuration; do not treat extreme values as validated input.

[Default and count tests](../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) and [boundary tests](../NuciGenerators.Text.UnitTests/NameGeneratorValidationTests.cs) establish normal defaults and several invalid-bound cases. [Open questions](./ambiguities-and-open-questions.md) distinguishes untested invalid settings from guaranteed outcomes. For state sharing, see [State and persistence](./state-and-persistence.md).
