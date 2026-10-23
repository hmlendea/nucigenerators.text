# Invariants and contracts

## Base-class contracts under stable inputs

These statements concern `NuciGenerators.Text.NameGenerator`, not arbitrary `NuciGenerators.Text.INameGenerator` implementations. They assume ordinary single-threaded, non-re-entrant use and well-formed input objects.

| Invariant | Why it holds | Verification and change sensitivity |
|-----------|--------------|-------------------------------------|
| Every returned result passed acceptance when committed | Only the true branch appends to the batch | [Validation](../NuciGenerators.Text.UnitTests/NameGeneratorValidationTests.cs) and [batch](../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) tests; rule reordering affects exceptions |
| Current history duplicates are rejected regardless of seed reuse | History guard precedes and is independent of `OnlyNewNames` | Repeated-candidate and history tests |
| No ordinary validation call appends history | `IsNameValid` reads state and returns a decision | Repeated-validation test; custom enumerables remain arbitrary code |
| Successful batches are distinct from history and each other | A new list is allocated per call | Batch-independence and reset tests |
| Acceptance commits immediately, not at batch completion | Both append operations occur inside each iteration | Failure-after-success test |
| Reset clears the existing history object only | It calls `Clear()` on the current list | Reset-identity test |
| Wordlist membership uses insertion-time key, not current object identifier | Dictionary insertion captures key separately from object reference | Identifier-mutation test in [collection tests](../NuciGenerators.Text.UnitTests/Models/WordlistTests.cs) |
| Schema hash and equality use the same four fields | Both methods use identifier, name, schema, filter path | [Schema tests](../NuciGenerators.Text.UnitTests/Models/GenerationSchemaTests.cs) for ordinary base instances |

## Not invariants

- A full requested count is not guaranteed; deadline exhaustion is a normal return.
- Elapsed time is not strictly bounded by the configured budget.
- Nonpositive count is not an early-return validation path: deadline setup happens first.
- Inputs and history are not immutable, private, durable, or globally unique.
- All strings are not necessarily non-personal, normalised, or trimmed.
- Data annotations do not enforce setters and do not create persistence.
- A schema does not direct the base algorithm, and casing metadata does not transform results.
- No concurrency or re-entrancy guarantee is established.

## Compatibility-sensitive details

Preserve the misspelled protected method signature, public mutable collection types, enum numeric order, annotation bounds, literal comparison modes, and reset collection identity unless explicitly changing the contract. Hash codes must not be used as permanent serialised identifiers. See [API reference](./api-reference/INDEX.md), [Change guide](./change-guide.md), and [Open questions](./ambiguities-and-open-questions.md).
