# Error handling

## Error taxonomy

| Origin and symbol | Condition | Observable result | Evidence |
|-------------------|-----------|-------------------|----------|
| `NuciGenerators.Text.NameGenerator.IsNameValid` | Ordinary candidate violates a reached rule | `false`; generation requests another candidate if loop permits | [Validation tests](../NuciGenerators.Text.UnitTests/NameGeneratorValidationTests.cs) |
| `NuciGenerators.Text.NameGenerator.Generate` | Count/deadline stops loop | Empty or partial materialised batch; no error status object | [Generation tests](../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) |
| Subclass `GenerationAlogrithm` | Consumer algorithm throws | Exception propagates; prior accepted history retained | [Failure tests](../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) |
| Regex evaluation | Reached pattern has invalid syntax | Argument-exception family propagates | [Malformed-pattern tests](../NuciGenerators.Text.UnitTests/NameGeneratorValidationTests.cs) |
| Seed enumeration | Null collections/elements or enumerable throws | Underlying exception propagates when branch is reached | [Source](../NuciGenerators.Text/NameGenerator.cs); not comprehensively covered |
| `NuciGenerators.Text.Models.Wordlist.Add` | Duplicate key | `ArgumentException`; original member retained | [Collection tests](../NuciGenerators.Text.UnitTests/Models/WordlistTests.cs) |
| `NuciGenerators.Text.Models.Wordlist.Get` | Missing key | `KeyNotFoundException` | [Collection tests](../NuciGenerators.Text.UnitTests/Models/WordlistTests.cs) |
| Wordlist construction/insertion/lookup | Null key or null source | `ArgumentNullException` from underlying operations | [Collection tests](../NuciGenerators.Text.UnitTests/Models/WordlistTests.cs) |
| Wordlist construction/insertion | Null word element | `NullReferenceException` while reading its identifier | [Collection tests](../NuciGenerators.Text.UnitTests/Models/WordlistTests.cs) |
| Explicit schema annotation validation | Length violates attributes | Validator reports invalid result; setter itself succeeds | [Schema tests](../NuciGenerators.Text.UnitTests/Models/GenerationSchemaTests.cs) |

## Partial-state contract

There are no catch blocks, exception wrapping, error codes, retries, or rollback in production source. Once a string is appended to history, a later algorithm/validation exception does not remove it. No batch is returned from the failed call. Reset is an explicit consumer choice and can permit duplicate reuse; automatically resetting in a catch would change the consumer's session contract.

## Reachability and recovery

An invalid regex can be masked by early length, affix, history, or seed rejection, or by short-circuit evaluation of earlier patterns. A negative budget can stop before any malformed settings are consumed. To diagnose, reproduce the input with the exact settings and state that reached the failing branch; do not presume all calls should fail identically.

Consumers should catch failures at the application boundary, choose whether to retain history, and redact sensitive input before recording diagnostics. This is guidance, not an implemented exception policy. [Logging](./logging.md), [Validation flow](./flows/validation.md), and [Troubleshooting](./troubleshooting.md) provide related details.
