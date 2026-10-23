# Testing

## Execution evidence

On 2026-10-24, `dotnet test NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj --verbosity minimal --nologo` completed successfully with SDK 10.0.112: 379 discovered, 379 passed, zero failed or skipped. This is a local baseline result, not a coverage percentage, performance measurement, or remote CI result.

[Test project](../NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj): NUnit 4.6.1, adapter 6.2.0, Test SDK 18.3.0, coverlet collector 6.0.4, nullable reference checking enabled, and a direct production project reference. No integration-test or architecture-test project exists.

## Test seam

[Adapter](../NuciGenerators.Text.UnitTests/ScriptedNameGenerator.cs): `NuciGenerators.Text.UnitTests.ScriptedNameGenerator` queues supplied strings, counts calls, and exposes the protected validation method. Dequeue exhaustion throws. This makes candidate ordering deterministic without replacing production validation or using reflection. It does not model production randomness, I/O, timing delays, or algorithm exhaustion contracts beyond its own queue.

## Behaviour-to-test index

All method names below are members of the fully qualified fixture shown in the first column; links locate their implementations.

| Fixture and source | Test methods to inspect | Contract demonstrated |
|--------------------|-------------------------|-----------------------|
| `NuciGenerators.Text.UnitTests.NameGeneratorTests` ([source](../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs)) | `GivenANewGenerator_WhenInspectingDefaults_ThenGenerationStartsUnrestrictedExceptForLength`; `GivenANonPositiveCount_WhenGenerating_ThenTheAlgorithmIsNotCalled` | Defaults and skipped candidate production |
| Same fixture | `GivenEnoughValidCandidates_WhenGenerating_ThenExactlyTheRequestedNamesAreRecorded`; `GivenInvalidCandidatesBeforeAValidOne_WhenGenerating_ThenOnlyTheValidNameIsRecorded` | Accepted order, history, call count, rejection retries |
| Same fixture | `GivenRepeatedCandidates_WhenGenerating_ThenDuplicatesAreSkippedEvenWhenSeedNamesAreAllowed`; `GivenPreviousGeneration_WhenGeneratingAgain_ThenHistoryIsAppendedAndResultsAreIndependent` | History distinct from seed-reuse flag and per-batch lists |
| Same fixture | `GivenAnAlreadyExpiredBudget_WhenGenerating_ThenNoCandidateIsRequested` | Negative initial budget, not hard cancellation |
| Same fixture | `GivenGenerationHistory_WhenResetting_ThenTheExistingHistoryCollectionIsCleared`; `GivenAPreviouslyGeneratedName_WhenResetting_ThenItCanBeGeneratedAgain` | In-place clearing, preserved configuration, candidate reuse |
| Same fixture | `GivenAFailingAlgorithm_WhenGenerating_ThenItsExceptionIsPropagatedWithoutRecordingAName`; `GivenAnAlgorithmFailureAfterSuccess_WhenGenerating_ThenSuccessfulHistoryIsRetained` | Exceptions and nontransactional history |
| Same fixture | `GivenSeparateGenerators_WhenMutatingOne_ThenTheirStateIsIsolated`; `GivenSharedWordlists_WhenAddingASeedAfterConstruction_ThenValidationUsesTheNewSeed` | Instance defaults versus deliberately shared input |
| `NuciGenerators.Text.UnitTests.NameGeneratorValidationTests` ([source](../NuciGenerators.Text.UnitTests/NameGeneratorValidationTests.cs)) | `GivenABlankName_WhenValidating_ThenItIsRejected`; `GivenDefaultLengthLimits_WhenValidating_ThenTheLengthIsEnforced`; `GivenCustomLengthLimits_WhenValidating_ThenBoundariesAreInclusive` | Whitespace, inclusive bounds, inconsistent bounds, UTF-16 length |
| Same fixture | `GivenPrefixAndSuffixFilters_WhenValidating_ThenLiteralCaseSensitiveMatchingIsUsed`; `GivenGenerationHistory_WhenValidating_ThenOnlyExactDuplicatesAreRejected`; `GivenMultipleSeedLists_WhenValidating_ThenEverySeedValueIsConsidered` | Literal affixes and exact-value history/seed scans |
| Same fixture | `GivenARegexPattern_WhenValidating_ThenInclusionAndExclusionHaveOppositeEffects`; `GivenMultipleRequiredPatterns_WhenValidating_ThenAllMustMatch`; `GivenMultipleExcludedPatterns_WhenValidating_ThenAnyMatchRejectsTheName`; `GivenBothPatternLists_WhenValidating_ThenBothConstraintsMustBeSatisfied` | Regex combinations and short-circuit predicate results |
| Same fixture | `GivenAnInvalidPattern_WhenValidating_ThenTheRegexErrorIsPropagated`; `GivenAnInvalidNameAndMalformedPatterns_WhenValidating_ThenLengthGuardsRunFirst`; `GivenACandidate_WhenValidatingRepeatedly_ThenHistoryIsNotModified` | Invalid patterns, guard ordering, validation purity |
| `NuciGenerators.Text.UnitTests.Models.WordlistTests` ([source](../NuciGenerators.Text.UnitTests/Models/WordlistTests.cs)) | Constructor/enumeration, insertion/retrieval, literal identifiers, duplicate/missing/null cases | Dictionary contract and original object identity |
| Same fixture | `GivenAConstructedWordlist_WhenChangingTheSourceCollection_ThenMembershipDoesNotChange`; `GivenAValuesView_WhenAddingAWord_ThenTheViewReflectsTheAddition`; `GivenAStoredWord_WhenChangingItsValues_ThenRetrievalReflectsTheChange`; `GivenAStoredWord_WhenChangingItsIdentifier_ThenItsOriginalDictionaryKeyRemains` | Membership snapshot, live view, value alias, captured key |
| `NuciGenerators.Text.UnitTests.Models.GenerationSchemaTests` ([source](../NuciGenerators.Text.UnitTests/Models/GenerationSchemaTests.cs)) | Matching/nullable-field combinations, each differing equality field, category/casing differences, self/null/unrelated-object comparison | Base-instance equality and hash consistency |
| Same fixture | `GivenAnIdentifierLength_WhenValidatingAnnotations_ThenDeclaredBoundsAreEnforced`; `GivenANameLength_WhenValidatingAnnotations_ThenTheMaximumIsEnforced`; `GivenASchemaLength_WhenValidatingAnnotations_ThenTheMaximumIsEnforced`; `GivenANewSchema_WhenValidating_ThenNullableFieldsAreAcceptedAndSentenceCaseIsTheDefault` | Explicit annotation validation and constructor default |

## Focused execution

```bash
dotnet test NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj --filter FullyQualifiedName~NameGeneratorValidationTests
dotnet test NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj --filter FullyQualifiedName~WordlistTests
dotnet test NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj --filter FullyQualifiedName~GenerationSchemaTests
```

Optional collection through the declared coverage tooling:

```bash
dotnet test NuciGenerators.Text.UnitTests/NuciGenerators.Text.UnitTests.csproj --collect:"XPlat Code Coverage"
```

Coverage collection was not executed for this task. The collector normally writes results under the test run's results directory; no committed report or threshold is configured in the workflow.

## Gaps and limitations

Tests do not establish hard timeout behaviour, wall-clock-change handling, budget-overflow semantics, concurrency/re-entrancy safety, derived-schema equality, arbitrary throwing/lazy enumerable behaviour, cryptographic randomness, remote integration behaviour, or explicit enum numeric compatibility. They also do not compile the README example or test consumer-specific algorithms. See [Open questions](./ambiguities-and-open-questions.md). Large parameterised suites are not evidence of every branch or contract being covered.
