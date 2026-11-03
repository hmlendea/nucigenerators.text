# Dependencies

## Production boundary

[Production manifest](../NuciGenerators.Text/NuciGenerators.Text.csproj) targets `net10.0` and has no direct third-party production dependencies. The repository uses only .NET BCL APIs in production code.

Equality of ordinary base instances and rejection of unrelated object types are corroborated by [schema tests](../NuciGenerators.Text.UnitTests/Models/GenerationSchemaTests.cs). Derived-schema behaviour remains a separate gap.

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

[Test manifest](../NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj) references NUnit 5.0.0, NUnit3TestAdapter 6.3.1, Microsoft.NET.Test.Sdk 18.11.0, and coverlet.collector 10.1.0. They supply assertion/discovery/execution and optional coverage collection, not production functionality. Tests reference the library project; production does not reference tests.

## Tooling and delivery

[CI](../.github/workflows/dotnet.yml) uses GitHub-hosted Ubuntu with checkout v2 and setup-dotnet v1, selecting .NET 10.0.x. Those pinned action majors are current repository configuration, not an endorsement of maintenance/security status. No hosted integration is required to execute generation in an installed application. See [Build and deployment](./build-and-deployment.md) and [Integrations](./integrations.md).
