[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucigenerators.text)](https://github.com/hmlendea/nucigenerators.text/releases/latest)
[![Build Status](https://github.com/hmlendea/nucigenerators.text/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucigenerators.text/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/NuciGenerators.Text)](https://nuget.org/packages/NuciGenerators.Text)
[![License](https://img.shields.io/github/license/hmlendea/nucigenerators.text)](https://github.com/hmlendea/nucigenerators.text/blob/master/LICENSE)

# NuciGenerators.Text

Core text generation primitives for the NuciGenerators ecosystem.

NuciGenerators.Text is a .NET library for building custom text and name generators from reusable generation contracts, validation rules, and word models.
## 📑 Table of Contents

- [Capabilities](#capabilities)
- [Use Cases](#use-cases)
- [Usage](#usage)
  - [Examples](#examples)
- [Compatibility](#compatibility)
- [Installation](#installation)
  - [Package Manager Installation](#package-manager-installation)
- [Configuration](#configuration)
  - [Settings](#settings)
- [Extensibility](#extensibility)
- [Privacy and Data](#privacy-and-data)
- [Development](#development)
  - [Requirements](#requirements)
  - [Setup](#setup)
  - [Build](#build)
  - [Test](#test)
  - [Continuous Integration](#continuous-integration)
  - [Release](#release)
  - [Dependencies](#dependencies)
- [GitHub Actions](#github-actions)
- [Project Structure](#project-structure)
  - [Projects and Packages](#projects-and-packages)
  - [Directories](#directories)
- [Architecture](#architecture)
- [Documentation](#documentation)
- [API Reference](#api-reference)
- [Troubleshooting](#troubleshooting)
- [FAQ](#faq)
- [Contributing](#contributing)
- [Code of Conduct](#code-of-conduct)
- [Security](#security)
- [Project Engagement](#project-engagement)
- [License](#license)

## ✨ Capabilities

- Define custom candidate-generation algorithms through `NameGenerator`.
- Generate batches of unique text values with a cooperative processing budget.
- Apply length, prefix, suffix, inclusion, exclusion, and seed-word filters.
- Track generated values and reset generation history.
- Describe reusable words, wordlists, schemas, and casing modes.

## 🎯 Use Cases

- **Custom generators:** Build fantasy name, procedural word, and domain-specific text generators.
- **Reusable word data:** Store and group word fragments in keyed wordlists.
- **Validated output:** Apply shared acceptance rules before returning generated values.

## 🚀 Usage

Derive from `NameGenerator`, implement `GenerationAlgorithm()`, and configure the inherited filters before requesting results.

```csharp
using System.Collections.Generic;
using NuciGenerators.Text;

public sealed class ExampleNameGenerator : NameGenerator
{
    private static readonly string[] candidates = ["Oscar", "Robert", "Ilarion"];
    private int nextCandidate;

    public ExampleNameGenerator() : base([])
    {
    }

    protected override string GenerationAlgorithm()
    {
        string candidate = candidates[nextCandidate];
        nextCandidate = (nextCandidate + 1) % candidates.Length;
        return candidate;
    }
}

ExampleNameGenerator generator = new();
IEnumerable<string> names = generator.Generate(2);
```

### Examples

The [quick-start guide](./docs/quick-start.md) provides a complete deterministic consumer example and links to the relevant behaviour documentation.

## 🧩 Compatibility

| Component | Supported Versions | Notes |
|-----------|--------------------|-------|
| .NET | 10.0 | The library targets `net10.0`. |

## 📦 Installation

[![Obtain it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciGenerators.Text)

### Package Manager Installation

```bash
dotnet add package NuciGenerators.Text
```

Or, via the `Package Manager Console`:
```powershell
Install-Package NuciGenerators.Text
```

## ⚙️ Configuration

The consumer configures public `NameGenerator` properties directly in code. There are no configuration files, environment variables, command-line options, secret providers, or precedence chains.

### Settings

The subsequent settings are recognised:
| Section | Key | Type | Default | Required | Description |
|---------|-----|------|---------|----------|-------------|
| `NameGenerator` | `MinNameLength` | `int` | `5` | No | Minimum candidate length. |
| `NameGenerator` | `MaxNameLength` | `int` | `10` | No | Maximum candidate length. |
| `NameGenerator` | `MaxProcessingTimePerWord` | `int` | `1000` | No | Milliseconds allocated per requested result. |
| `NameGenerator` | `OnlyNewNames` | `bool` | `true` | No | Controls rejection of values already present in seed wordlists. |
| `NameGenerator` | `StartsWithFilter` | `string` | Empty | No | Required prefix. |
| `NameGenerator` | `EndsWithFilter` | `string` | Empty | No | Required suffix. |
| `NameGenerator` | `ExcludedStrings` | `List<string>` | `[]` | No | Regular expressions that reject candidates. |
| `NameGenerator` | `IncludedStrings` | `List<string>` | `[]` | No | Regular expressions every candidate must match. |

## 🧱 Extensibility

Implement `GenerationAlgorithm()` in a `NameGenerator` subclass, or implement `INameGenerator` when a different generation flow is required.

| Extension Point | Contract | Purpose |
|-----------------|----------|---------|
| `GenerationAlgorithm()` | Protected abstract method | Produce one candidate for validation. |
| `INameGenerator` | Public interface | Supply an independent generator implementation. |

## 🛡️ Privacy and Data

NuciGenerators.Text does not collect, process, transmit, or persist personal data. The library operates entirely in memory on data provided by the consuming application.

For the detailed description of how the application handles privacy and personal data, see [PRIVACY.md](./PRIVACY.md).

## 🛠️ Development

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Setup

```bash
git clone https://github.com/hmlendea/nucigenerators.text.git
cd nucigenerators.text
dotnet restore
```

### Build
- `OnlyNewNames` is enabled and the same value already exists in any seed `Wordlist`
- it matches any pattern in `ExcludedStrings`
- it does not match every pattern in `IncludedStrings`

`ExcludedStrings` and `IncludedStrings` are evaluated as regular expressions.

## Generation Behavior

`Generate(int maximumCount)` attempts to return up to the requested number of names.

Important behavior notes:

- the method may return fewer items than requested if the constraints are too restrictive
- the total processing window is limited by `MaxProcessingTimePerWord * maximumCount` milliseconds
- accepted results are appended to `GeneratedWords`
- `Reset()` clears the generated history so values can be produced again

## Public API Summary

### `INameGenerator`

```csharp
public interface INameGenerator
{
	int MinNameLength { get; set; }
	int MaxNameLength { get; set; }
	int MaxProcessingTimePerWord { get; set; }
	List<string> ExcludedStrings { get; set; }
	List<string> IncludedStrings { get; set; }
	string StartsWithFilter { get; set; }
	string EndsWithFilter { get; set; }
	List<string> GeneratedWords { get; }
	List<Wordlist> Wordlists { get; }

	IEnumerable<string> Generate(int maximumCount);
	void Reset();
}
```

### `Word`

```csharp
public sealed class Word
{
	public string Id { get; set; }
	public IEnumerable<string> Values { get; set; }
}
```

### `Wordlist`

```csharp
public sealed class Wordlist : IEnumerable<Word>
{
	public IEnumerable<Word> Values { get; }

	public Wordlist();
	public Wordlist(IEnumerable<Word> words);

	public void Add(Word word);
	public Word Get(string id);
}
```

### `GenerationSchema`

`GenerationSchema` is a lightweight metadata model with the following fields:

- `Id`
- `Name`
- `Category`
- `Schema`
- `FilterlistPath`
- `WordCase`

It also implements equality members for comparing schema definitions.

## Typical Use Cases

- creating custom procedural name generators
- building domain-specific generator packages on top of a shared base
- storing and grouping word fragments in reusable wordlists
- applying validation and filtering rules to generated text

### Build

```bash
dotnet build NuciGenerators.Text.slnx
```

### Test

```bash
dotnet test NuciGenerators.Text.slnx --no-build --verbosity normal
```

### Continuous Integration

The `dotnet.yml` workflow restores dependencies, builds the solution, and runs tests on pushes and pull requests targeting `master`.

### Release

Published GitHub Releases trigger the `github-release.yml` workflow to pack the NuGet package, upload it to the release, and append its SHA256 checksum. The `nuget-release.yml` workflow publishes the package to NuGet.org using OIDC authentication.

### Dependencies

| Package | Version | Scope | Purpose |
|---------|---------|-------|---------|
| `NuciExtensions` | `6.0.2` | Runtime | Provides extensions used by the production library. |
| `NUnit` | `4.6.1` | Development | Provides unit-test assertions and execution support. |
| `NUnit3TestAdapter` | `6.2.0` | Development | Integrates NUnit tests with the .NET test platform. |
| `Microsoft.NET.Test.Sdk` | `18.3.0` | Development | Provides .NET test discovery and execution. |
| `coverlet.collector` | `6.0.4` | Development | Provides optional coverage collection for tests. |

## ⚙️ GitHub Actions

| Workflow | Purpose | What it does |
|----------|---------|--------------|
| [ .NET ](https://github.com/hmlendea/nucigenerators.text/blob/master/.github/workflows/dotnet.yml) | Continuous integration | Restores, builds, and tests the solution. |
| [ GitHub Release ](https://github.com/hmlendea/nucigenerators.text/blob/master/.github/workflows/github-release.yml) | GitHub package release | Packs the NuGet package, uploads it to a release, and records its checksum. |
| [ NuGet.org Release ](https://github.com/hmlendea/nucigenerators.text/blob/master/.github/workflows/nuget-release.yml) | NuGet publication | Packs and publishes the package after a published GitHub release. |

## 🗂️ Project Structure

The solution contains the production library and separate unit-test and architecture-test projects.

### Projects and Packages

| Project | Type | Purpose |
|---------|------|---------|
| `NuciGenerators.Text` | .NET library | Public generation contracts and models. |
| `NuciGenerators.Text.UnitTests` | Test project | Unit tests for generation and model behaviour. |
| `NuciGenerators.Text.ArchitectureTests` | Test project | Architecture and source-file convention tests. |

### Directories

| Directory | Purpose |
|-----------|---------|
| `NuciGenerators.Text/Models` | Production word, schema, and casing models. |
| `NuciGenerators.Text.UnitTests/Models` | Model unit tests. |
| `docs` | Repository knowledge base and API documentation. |
| `.github/workflows` | Continuous integration and release workflows. |

## 🏗️ Architecture

NuciGenerators.Text is an in-process .NET library with an interface-based generation contract, an extensible base generator, and mutable in-memory word models.

See the [architecture documentation](./ARCHITECTURE.md) for the system context, principal components, runtime flows, ownership boundaries, dependencies, constraints, and extension points.

## 📚 Documentation

The repository maintains a knowledge base covering implementation contracts, execution flows, state ownership, verification evidence, and safe modification paths.

| Resource | Description |
|----------|-------------|
| [Knowledge Base Index](./docs/INDEX.md) | Complete documentation catalogue and entry points. |
| [Quick Start](./docs/quick-start.md) | Deterministic consumer example and next steps. |
| [Configuration](./docs/configuration.md) | Public settings, defaults, and invalid configurations. |
| [Testing](./docs/testing.md) | Test coverage and verification guidance. |
| [Troubleshooting](./docs/troubleshooting.md) | Evidence-based diagnostic guidance. |
| [FAQ](./docs/faq.md) | Frequently asked questions. |

## 🌐 API Reference

See the [API reference index](./docs/api-reference/INDEX.md) for the public CLR contracts, protected extension points, and model documentation.

## 🩺 Troubleshooting

See the [Troubleshooting Guide](./docs/troubleshooting.md) for diagnostic procedures and resolutions to recurring problems.

## ❓ FAQ

See the [Frequently Asked Questions](./docs/faq.md) for responses to frequently raised inquiries.

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Include tests for additional or modified functionality
- Raise a new [issue](https://github.com/hmlendea/nucigenerators.text/issues) for problems or suggestions

See the [contributing guidelines](./CONTRIBUTING.md) for details on how to report issues, suggest enhancements, and submit changes.

## 📜 Code of Conduct

See the [Code of Conduct](./CODE_OF_CONDUCT.md) for the standards of conduct expected in all project spaces.

## 🔒 Security

NuciGenerators.Text is a library with no network operations, file I/O, or persistent storage.

For information on reporting security vulnerabilities, see [SECURITY.md](./SECURITY.md).

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/nucigenerators.text/issues)!

If you find this project useful, consider funding it or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)

## 📄 License

This project is being distributed under the `GNU General Public License v3.0` or later.
See [LICENSE](./LICENSE) for additional information.
