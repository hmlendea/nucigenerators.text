# Security mechanics

This page explains code-level trust boundaries and limitations; vulnerability reporting and supported-distribution policy remain in the root [Security policy](../SECURITY.md). It is not a second security notice or a security certification.

## Inputs and trust

The library runs with the host process's privileges. Caller-provided algorithms and arbitrary enumerable seed values execute in that process; there is no sandbox, authentication, authorisation, or permission boundary. The base does not enforce that input is non-sensitive. Generated strings may originate from personal data supplied by the host.

## Threats and implemented limits

| Concern | Evidence and present limit | Host responsibility |
|---------|----------------------------|---------------------|
| Expensive regex or hostile patterns | [Validation](../NuciGenerators.Text/NameGenerator.cs) uses static `Regex.IsMatch` without a per-call timeout | Restrict pattern authors and assess runtime regex timeout policy; the batch deadline is not protection during a match |
| Nonterminating algorithm or enumerable | Arbitrary synchronous callback/enumeration; no cancellation or pre-emption | Bound or isolate consumer work |
| Memory/CPU exhaustion | Unbounded history and repeated linear scans | Limit input, batch size, session lifetime, and parallelism |
| Shared-state corruption | Public mutable lists and no locking | Coordinate aliases or isolate state |
| Path handling | `FilterlistPath` is stored but never read | A future file-loading consumer must validate paths and privileges |
| Randomness | Protected `System.Random` is a convenience, unused by the base | Do not use generated names as secrets or assume cryptographic randomness |
| Error disclosure | Exceptions propagate; no logging/redaction layer | Sanitise host diagnostics and user-visible failures |

## Data flow qualification

Repository-owned production source contains no explicit network/file persistence operation. This says nothing about consumer subclasses or the content of supplied strings. [Privacy](../PRIVACY.md) and the existing README use stronger personal-data wording than source alone supports; [Open questions](./ambiguities-and-open-questions.md) records that distinction instead of silently treating it as a guarantee.

## Verification scope

[Validation tests](../NuciGenerators.Text.UnitTests/NameGeneratorValidationTests.cs) test malformed patterns and guards, not adversarial execution time. No security scan results, penetration tests, sandbox proof, secure-erasure test, or dependency vulnerability assessment were produced for this documentation task.
