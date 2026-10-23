# Repository structure

## Production and verification map

| Location | Ownership and entry symbols |
|----------|-----------------------------|
| [Solution](../NuciGenerators.Text.slnx) | Main library and unit-test project; no executable project |
| [Public contract](../NuciGenerators.Text/INameGenerator.cs) | `NuciGenerators.Text.INameGenerator` |
| [Generation base](../NuciGenerators.Text/NameGenerator.cs) | `NuciGenerators.Text.NameGenerator.Generate`, `Reset`, `IsNameValid`, `GenerationAlogrithm` |
| [Word](../NuciGenerators.Text/Models/Word.cs) | `NuciGenerators.Text.Models.Word` |
| [Wordlist](../NuciGenerators.Text/Models/Wordlist.cs) | `NuciGenerators.Text.Models.Wordlist` |
| [Schema](../NuciGenerators.Text/Models/GenerationSchema.cs) | `NuciGenerators.Text.Models.GenerationSchema` |
| [Casing enum](../NuciGenerators.Text/Models/WordCase.cs) | `NuciGenerators.Text.Models.WordCase` |
| [Library manifest](../NuciGenerators.Text/NuciGenerators.Text.csproj) | Target, package identity, licence, direct production dependency |
| [Test manifest](../NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj) | NUnit tooling and production project reference |
| [Test adapter](../NuciGenerators.Text.UnitTests/ScriptedNameGenerator.cs) | Queue-backed subclass and protected validation exposure |
| [Batch tests](../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) | Construction, generation, history, failure, and reset |
| [Validation tests](../NuciGenerators.Text.UnitTests/NameGeneratorValidationTests.cs) | Ordered acceptance constraints and regex failure |
| [Collection tests](../NuciGenerators.Text.UnitTests/Models/WordlistTests.cs) | Lookup, membership snapshots, aliasing, and exceptions |
| [Schema tests](../NuciGenerators.Text.UnitTests/Models/GenerationSchemaTests.cs) | Equality, hashing, annotations, and casing default |
| [CI workflow](../.github/workflows/dotnet.yml) | Restore, build, test on master pushes and pull requests |

## Generated and non-runtime files

Build output under `bin/` and `obj/` is not authoritative source. [Ignore rules](../.gitignore) exclude generated output, test results, and local artefacts. [Funding configuration](../.github/FUNDING.yml) concerns project funding, not runtime data processing. Root policy files and the [architecture document](../ARCHITECTURE.md) have distinct scopes indexed in [Documentation](./INDEX.md).

No tracked `docs/` content preceded this knowledge base. Directory examples in generic documentation requirements do not imply browser, calendar, presentation, authentication, or HTTP capabilities in this repository.
