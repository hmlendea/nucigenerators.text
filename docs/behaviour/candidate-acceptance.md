# Candidate acceptance

## Ordered rule matrix

[Implementation](../../NuciGenerators.Text/NameGenerator.cs): `NuciGenerators.Text.NameGenerator.IsNameValid`. An accepted candidate must survive every reached rule in this order:

| Order | Decision | Observable consequence |
|-------|----------|------------------------|
| 1 | `string.IsNullOrWhiteSpace(candidate)` | Reject null, empty, or all-whitespace strings regardless of minimum length |
| 2 | Length below minimum or above maximum | Reject outside inclusive bounds; length counts UTF-16 code units |
| 3 | Prefix or suffix does not match | Reject; both filters use `StringComparison.InvariantCulture` and are case-sensitive, not regex |
| 4 | Candidate in history | Reject exact string duplicate even when seed reuse is enabled |
| 5 | `OnlyNewNames` and candidate in any seed values | Reject exact seed string; identifiers are not checked |
| 6 | Any excluded regex matches | Reject after the first matching excluded pattern |
| 7 | Not all required regex patterns match | Reject after the first nonmatching required pattern |
| 8 | No rejection | Return true without recording the string |

Prefix/suffix matching is linguistic invariant-culture comparison, not ordinal comparison. History uses ordinary string equality through list membership; seed membership uses the supplied enumerable's membership semantics. Do not collapse those into one universal comparer.

## Boundaries and transformation

No trimming, case folding, Unicode normalisation, casing conversion, or substring extraction occurs. A surrogate-pair character can occupy length two; [validation tests](../../NuciGenerators.Text.UnitTests/NameGeneratorValidationTests.cs) include that case. Non-whitespace strings containing spaces are not rejected solely for containing spaces.

An empty prefix/suffix matches. Empty required-pattern lists pass because `All` over an empty list is true; empty excluded-pattern lists do not reject because `Any` is false. An empty regex pattern matches any candidate reaching regex evaluation: as an inclusion it adds no restriction; as an exclusion it rejects everything reaching that rule. Regex patterns are not implicitly anchored, so `sc` matches within `Oscar`; use anchors for whole-string rules.

## State and alternative paths

Setting `NuciGenerators.Text.NameGenerator.OnlyNewNames=false` bypasses only seed checks; it does not bypass history, filters, or length. A malformed regex may remain unevaluated because an earlier guard rejects or an earlier pattern short-circuits its list. Rule reordering therefore changes which exceptions are visible and what work occurs, even if many ordinary true/false results remain unchanged.

## Verification

[Validation tests](../../NuciGenerators.Text.UnitTests/NameGeneratorValidationTests.cs) cover Unicode whitespace, inclusive and inconsistent bounds, surrogate pairs, literal case-sensitive affixes, history and seed cases, regex inclusion/exclusion combinations, invalid syntax, and early length rejection before malformed patterns. Repeated validation tests demonstrate that the base method itself does not mutate history. See [Validation flow](../flows/validation.md) for downstream failure effects and [Testing](../testing.md) for the test-symbol index.
