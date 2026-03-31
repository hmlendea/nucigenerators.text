[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html) [![Build Status](https://github.com/hmlendea/nucigenerators.text/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucigenerators.text/actions/workflows/dotnet.yml) [![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucigenerators.text)](https://github.com/hmlendea/nucigenerators.text/releases/latest)

# NuciGenerators.Text

Core text generation primitives for the NuciGenerators ecosystem.

This package provides the shared abstractions and models used to build text and name generators. It is not a ready-made generator by itself. Instead, it gives you:

- `INameGenerator`, the public generator contract
- `NameGenerator`, an abstract base class with validation and generation flow
- model types such as `Word`, `Wordlist`, `GenerationSchema`, and `WordCase`

If you want to build a custom fantasy name generator, procedural word generator, or a higher-level package on top of the NuciGenerators stack, this is the base package to start from.

## Features

- Simple interface for generating a batch of names
- Reusable abstract base class for implementing custom generation algorithms
- Built-in validation for:
  - minimum and maximum length
  - prefix and suffix filters
  - required regex patterns
  - forbidden regex patterns
  - duplicate prevention
  - excluding names already present in the seed wordlists
- Tracking of previously generated values
- Basic word and schema models for higher-level generator implementations

## Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciGenerators.Text)

### .NET CLI

```bash
dotnet add package NuciGenerators.Text
```

### Package Manager

```powershell
Install-Package NuciGenerators.Text
```

## Target Framework

The project currently targets `.NET 10.0`.

## What This Package Contains

### `INameGenerator`

The public contract for generators. It exposes:

- `MinNameLength`
- `MaxNameLength`
- `MaxProcessingTimePerWord`
- `ExcludedStrings`
- `IncludedStrings`
- `StartsWithFilter`
- `EndsWithFilter`
- `GeneratedWords`
- `Wordlists`
- `Generate(int maximumCount)`
- `Reset()`

### `NameGenerator`

An abstract base class that implements most of the behavior needed by a generator:

- loops until it collects the requested number of valid results or the processing time budget is exhausted
- calls your custom generation algorithm through `GenerationAlogrithm()`
- validates each candidate name before accepting it
- stores accepted values in `GeneratedWords`

Default settings:

- `MinNameLength = 5`
- `MaxNameLength = 10`
- `MaxProcessingTimePerWord = 1000`
- `OnlyNewNames = true`
- `StartsWithFilter = string.Empty`
- `EndsWithFilter = string.Empty`
- `ExcludedStrings = []`
- `IncludedStrings = []`

### Models

- `Word`: a word entry identified by `Id`, with one or more string `Values`
- `Wordlist`: a keyed collection of `Word` objects with enumeration support
- `GenerationSchema`: metadata container for describing generation schemes
- `WordCase`: casing mode enum with `Original`, `Lower`, `Upper`, `Title`, and `Sentence`

## Quick Start

Because `NameGenerator` is abstract, you need to derive from it and implement the generation algorithm.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

using NuciGenerators.Text;
using NuciGenerators.Text.Models;

public sealed class SimpleNameGenerator : NameGenerator
{
	public SimpleNameGenerator(List<Wordlist> wordlists)
		: base(wordlists)
	{
	}

	protected override string GenerationAlogrithm()
	{
		Wordlist syllables = Wordlists[0];

		string first = syllables.Values.ElementAt(random.Next(syllables.Values.Count())).Values.First();
		string second = syllables.Values.ElementAt(random.Next(syllables.Values.Count())).Values.First();

		return first + second;
	}
}

List<Wordlist> wordlists =
[
	new Wordlist(
	[
		new Word { Id = "a", Values = ["al", "ar", "an"] },
		new Word { Id = "b", Values = ["dor", "mir", "len"] },
		new Word { Id = "c", Values = ["ia", "or", "us"] }
	])
];

SimpleNameGenerator generator = new(wordlists)
{
	MinNameLength = 4,
	MaxNameLength = 8,
	StartsWithFilter = "a",
	ExcludedStrings = [".*rr.*"],
	IncludedStrings = ["[aeiou]"],
};

IEnumerable<string> names = generator.Generate(10);

foreach (string name in names)
{
	Console.WriteLine(name);
}
```

## Validation Rules

Every candidate returned by `GenerationAlogrithm()` is checked before it is accepted.

A generated name is rejected when:

- it is `null`, empty, or whitespace
- its length is outside the configured minimum and maximum bounds
- it does not start with `StartsWithFilter`
- it does not end with `EndsWithFilter`
- it has already been generated in the current session
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

## Building from Source

```bash
dotnet build NuciGenerators.Text.sln
```

To produce a release build:

```bash
dotnet build NuciGenerators.Text.sln -c Release
```

## Project Structure

```text
NuciGenerators.Text/
  INameGenerator.cs
  NameGenerator.cs
  Models/
	GenerationSchema.cs
	Word.cs
	WordCase.cs
	Wordlist.cs
```

## Contributing

Contributions are welcome.

When contributing:

- keep the library cross-platform
- preserve the existing public API unless a breaking change is intentional
- keep changes focused and consistent with the current coding style
- update documentation when behavior changes

## License

This project is licensed under the `GNU General Public License v3.0` or later. See [LICENSE](./LICENSE) for details.
