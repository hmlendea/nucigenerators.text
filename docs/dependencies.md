# Dependencies

## Production boundary

[Production manifest](../NuciGenerators.Text/NuciGenerators.Text.csproj) targets `net10.0` and directly references NuciExtensions 5.3.1. The repository call site is `NuciGenerators.Text.Models.GenerationSchema.Equals(object)` in [schema source](../NuciGenerators.Text/Models/GenerationSchema.cs), using `NotEquals` on runtime types before casting. No other production third-party call sites are present.

The dependency's implementation was not decompiled or substituted with its latest branch. This knowledge base records the call site's role and tests, not unverified package internals. Equality of ordinary base instances and rejection of unrelated object types are corroborated by [schema tests](../NuciGenerators.Text.UnitTests/Models/GenerationSchemaTests.cs). Derived-schema behaviour remains a separate gap.

## Platform dependencies

| BCL facility | Required effect | Change-sensitive consequence |
|--------------|-----------------|------------------------------|
| `Dictionary<string, Word>` and LINQ `ToDictionary` | Materialise keyed membership | Duplicate/null key failures and object retention |
| LINQ `Any`, `All`, and enumerable `Contains` | Ordered early termination of validation scans | Failure masking and execution of custom enumerable code |
| `Regex.IsMatch` static overload | Evaluate supplied patterns | Syntax failures, regex options/defaults, and no library-selected per-call timeout |
| `DateTime.Now` and `AddMilliseconds` | Calculate and sample cooperative deadline | Wall-clock dependence and setup before count check |
| `Random` | Protected subclass convenience | No reproducible seed injection in the base constructor |
| Data annotations | Optional consumer-invoked metadata validation | Attribute presence does not enforce setter validation |

## Verification dependencies

[Test manifest](../NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj) references NUnit 4.6.1, NUnit3TestAdapter 6.2.0, Microsoft.NET.Test.Sdk 18.3.0, and coverlet.collector 6.0.4. They supply assertion/discovery/execution and optional coverage collection, not production functionality. Tests reference the library project; production does not reference tests.

## Tooling and delivery

[CI](../.github/workflows/dotnet.yml) uses GitHub-hosted Ubuntu with checkout v2 and setup-dotnet v1, selecting .NET 10.0.x. Those pinned action majors are current repository configuration, not an endorsement of maintenance/security status. No hosted integration is required to execute generation in an installed application. See [Build and deployment](./build-and-deployment.md) and [Integrations](./integrations.md).
