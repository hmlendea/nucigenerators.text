# Repository overview

## Purpose and scope

NuciGenerators.Text provides the shared mechanics needed by specialised name and text generators: candidate generation is supplied by a consumer; acceptance, history, and batch collection can be inherited. A caller does not obtain a ready-made language model, random-name dictionary, or schema processor by installing the package.

The complete production boundary comprises six types: `NuciGenerators.Text.INameGenerator`, `NuciGenerators.Text.NameGenerator`, and `NuciGenerators.Text.Models.Word`, `Wordlist`, `GenerationSchema`, and `WordCase`. See the [API index](./api-reference/INDEX.md) for their contracts and the [root architecture](../ARCHITECTURE.md) for topology.

## Entry points and dependants

| Entry | Information entering | Observable effect | Responsibility outside this repository |
|-------|----------------------|-------------------|----------------------------------------|
| Concrete subclass construction | Caller-owned wordlist list | Shared list retained; default settings and fresh history created | Choose algorithm and input provenance |
| `NuciGenerators.Text.NameGenerator.Generate(int)` | Requested count and current mutable settings | Returns materialised strings; appends accepted strings to history | Use output, manage exceptions and retention |
| `NuciGenerators.Text.NameGenerator.Reset()` | Existing generator instance | Clears history in place | Decide when reuse is permitted |
| `NuciGenerators.Text.Models.Wordlist` constructors, `Add`, `Get` | Word identifiers and objects | In-memory keyed membership and lookup | Load source data and maintain key consistency |
| `NuciGenerators.Text.Models.GenerationSchema` | Consumer metadata | Stores values; supports equality and annotation inspection | Interpret schema, load filter files, apply casing |

The only concrete subclass in tracked source is the [test adapter](../NuciGenerators.Text.UnitTests/ScriptedNameGenerator.cs). It intentionally dequeues scripted candidates and is not a production generator. No application composition root, dependency-injection container, controller, executable, scheduler, or release script is tracked.

## System-wide reasoning model

The base class can promise acceptance rules only while its inputs remain stable: it exposes mutable lists and calls arbitrary subclass code. A rejection advances the algorithm without committing history; an acceptance commits immediately; a later exception does not roll back that commitment. This combination explains cross-call uniqueness, retry surprises, and why concurrent or re-entrant consumers need their own coordination. See [Invariants](./invariants.md).

## Evidence boundaries

All implementation claims here concern tracked production source and the current unit tests. Package registries, downstream generators, and remote deployment state are not inferred from badges. [Open questions](./ambiguities-and-open-questions.md) records differences between implementation evidence and existing prose.
