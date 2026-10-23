# Generator API

## Interface surface

[Source](../../NuciGenerators.Text/INameGenerator.cs): `NuciGenerators.Text.INameGenerator`.

| Fully qualified interface member | CLR type and access | Contract |
|--------------------------------|---------------------|----------|
| `NuciGenerators.Text.INameGenerator.MinNameLength` | `int`, get/set | Minimum accepted length; base uses inclusive bound |
| `NuciGenerators.Text.INameGenerator.MaxNameLength` | `int`, get/set | Maximum accepted length; base uses inclusive bound |
| `NuciGenerators.Text.INameGenerator.MaxProcessingTimePerWord` | `int`, get/set | Base uses milliseconds multiplied by requested count |
| `NuciGenerators.Text.INameGenerator.ExcludedStrings` | `List<string>`, get/set | Base rejects if any reached regex matches |
| `NuciGenerators.Text.INameGenerator.IncludedStrings` | `List<string>`, get/set | Base requires every reached regex to match |
| `NuciGenerators.Text.INameGenerator.StartsWithFilter` | `string`, get/set | Base uses literal invariant-culture prefix |
| `NuciGenerators.Text.INameGenerator.EndsWithFilter` | `string`, get/set | Base uses literal invariant-culture suffix |
| `NuciGenerators.Text.INameGenerator.GeneratedWords` | `List<string>`, get | Mutable contents despite getter-only interface |
| `NuciGenerators.Text.INameGenerator.Wordlists` | `List<Wordlist>`, get | Mutable input list despite getter-only interface |
| `NuciGenerators.Text.INameGenerator.Generate(int maximumCount)` | `IEnumerable<string>` | Generation entry; base returns materialised batch |
| `NuciGenerators.Text.INameGenerator.Reset()` | `void` | Base clears current history |

The interface itself contains no default implementations and does not expose `OnlyNewNames`. Defaults below belong to the base class, not every possible implementation.

## Abstract base surface

[Source](../../NuciGenerators.Text/NameGenerator.cs): `NuciGenerators.Text.NameGenerator`.

The public primary constructor accepts `List<NuciGenerators.Text.Models.Wordlist> wordlists`, retains that reference, and performs no argument validation. The class is abstract and must be subclassed. All interface properties/methods are public; `GeneratedWords` and `Wordlists` have protected setters. Other interface settings have public setters. Defaults and configuration timing are fully enumerated in [Configuration](../configuration.md).

| Additional fully qualified member | Visibility and type | Responsibility |
|----------------------------------|---------------------|----------------|
| `NuciGenerators.Text.NameGenerator.OnlyNewNames` | Public `bool`, get/set, default true | Toggle seed-value exclusion, not history duplicate suppression |
| `NuciGenerators.Text.NameGenerator.GenerationAlogrithm()` | Protected abstract `string` | Supply one candidate; spelling is compatibility-sensitive |
| `NuciGenerators.Text.NameGenerator.IsNameValid(string name)` | Protected nonvirtual `bool` | Ordered acceptance without base-owned history mutation |
| `NuciGenerators.Text.NameGenerator.random` | Protected readonly `Random` | Per-instance subclass convenience; base does not draw values |

## Generate call contract

Input is a requested maximum count; there is no request object. Successful result is a separate list exposed as enumerable. Accepted candidates appear in candidate order and also enter instance history immediately. Rejection retries until count/deadline stops iteration. The operation is synchronous; enumeration of the returned list does not rerun it.

Nonpositive counts skip candidate production after deadline setup. No typed error result, timeout flag, per-candidate reason, or partial batch is returned on exception. Subclass failures, malformed reached regex, and invalid reached collections propagate. Earlier acceptance persists in history. See [Generation flow](../flows/generation.md), [Errors](../error-handling.md), and [Concurrency](../concurrency-and-scheduling.md).

## Reset call contract

`NuciGenerators.Text.NameGenerator.Reset()` clears history in place. Repeated calls are harmless for ordinary state. It preserves filter configuration, seed input, earlier batch lists, and subclass state. It does not rewind the protected random object. A previously generated candidate can be accepted again if all current rules permit it.

## Verification and extension impact

[Generation tests](../../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) and [validation tests](../../NuciGenerators.Text.UnitTests/NameGeneratorValidationTests.cs) exercise the base contract through [the scripted subclass](../../NuciGenerators.Text.UnitTests/ScriptedNameGenerator.cs). There is no production alternative implementation to compare. Interface changes affect all consumers; protected signature changes affect subclasses; nonvirtual orchestration cannot be overridden in the ordinary way. See [Change guide](../change-guide.md).
