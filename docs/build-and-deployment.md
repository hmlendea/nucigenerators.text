# Build and deployment

## Repository build contract

[The solution](../NuciGenerators.Text.slnx) references the library and unit-test projects. Both target `net10.0`. Install a compatible .NET 10 SDK; no database, service container, frontend toolchain, or application configuration is required for repository tests. Commands execute from the repository root.

```bash
dotnet restore NuciGenerators.Text.slnx
dotnet build NuciGenerators.Text.slnx --no-restore
dotnet test NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj --no-build --verbosity minimal
```

For a Release build and tests using matching output:

```bash
dotnet build NuciGenerators.Text.slnx --configuration Release
dotnet test NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj --configuration Release --no-build --verbosity minimal
```

Using `--no-build` presumes the chosen configuration has already been built. The explicit test-project command avoids ambiguity about which project supplies the tests. On 2026-10-24, the project test command restored/built successfully and reported 379 passed, zero failed or skipped, using SDK 10.0.112.

## Configured CI

[The .NET workflow](../.github/workflows/dotnet.yml) runs on pushes and pull requests targeting `master`. Its single Ubuntu job checks out source with checkout v2, selects .NET 10.0.x with setup-dotnet v1, restores, builds without restoring again, and tests without rebuilding. It does not collect coverage explicitly, upload test artefacts, publish documentation, run a security audit, or release a package. Local test success is not evidence of remote workflow success.

## Package and deployment boundary

[The production manifest](../NuciGenerators.Text/NuciGenerators.Text.csproj) declares version 1.0.1, repository metadata, GPL-3.0-or-later, and `GeneratePackageOnBuild=false`. A build does not automatically generate a NuGet package. Explicit local packing can use:

```bash
dotnet pack NuciGenerators.Text/NuciGenerators.Text.csproj --configuration Release
```

This command describes an SDK operation, not a verified publication pipeline. No tracked release script, package-push workflow, deployment environment, container, installer, or service entry point is present. Published distribution support is described by the root [Security policy](../SECURITY.md); actual release publication automation remains unverified.

Consumers deploy the library inside their own process and own configuration, logging, storage, scaling, and lifecycle. Build artefacts are ignored by [repository ignore rules](../.gitignore). See [Testing](./testing.md) and [Troubleshooting](./troubleshooting.md).
