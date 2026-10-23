# Word collections

## Purpose and boundary

[Word source](../../NuciGenerators.Text/Models/Word.cs) defines `NuciGenerators.Text.Models.Word`, a sealed mutable identifier plus `IEnumerable<string>` values. [Wordlist source](../../NuciGenerators.Text/Models/Wordlist.cs) defines `NuciGenerators.Text.Models.Wordlist`, a sealed dictionary-backed enumerable. Neither type loads files, validates names, copies string sequences, normalises keys, or chooses random values.

## Membership versus object identity

The empty constructor creates an empty dictionary. The enumerable constructor materialises membership immediately with `ToDictionary`: it enumerates input once and retains each original word object. Later insertion or deletion in the source collection cannot change dictionary membership. This is different from `NuciGenerators.Text.NameGenerator.Wordlists`, which retains the caller's list itself.

`NuciGenerators.Text.Models.Wordlist.Values` is a live dictionary-values view, not a snapshot. Enumeration via generic or nongeneric interfaces delegates to that view. Inserting a word affects previously obtained views. `NuciGenerators.Text.Models.Wordlist.Get` returns the same word reference stored at insertion.

The dictionary key is captured from `NuciGenerators.Text.Models.Word.Id` when inserted. Mutating the object's identifier does not re-key the dictionary: lookup by the original key still succeeds and returns an object whose current identifier differs; lookup by the changed identifier fails. Mutating `NuciGenerators.Text.Models.Word.Values` is visible to subsequent retrieval and seed validation.

## Information crossing the validation boundary

Seed novelty checks traverse every reachable wordlist and word, then call `Contains(candidate)` on its values. They compare values, not dictionary keys. Thus changing a key or identifier does not directly determine novelty, whereas replacing a word's values can change acceptance immediately. With `OnlyNewNames=false`, the seed traversal is bypassed completely.

Values need not be a materialised collection: a lazy enumerable may execute on every validation attempt, be expensive, produce changing values, or throw. The library imposes no nonempty-values invariant and does not enforce that string elements are nonnull. A null values sequence fails only when consumed. These are caller obligations, not validations silently performed by the container.

## Failure and tests

`Add` uses dictionary insertion, so duplicate keys throw rather than overwrite. Missing lookup throws; null keys are invalid; null word elements fail during identifier access. Empty and whitespace keys are accepted and remain literal. No method provides removal or key replacement.

[Collection tests](../../NuciGenerators.Text.UnitTests/Models/WordlistTests.cs), especially `NuciGenerators.Text.UnitTests.Models.WordlistTests.GivenAConstructedWordlist_WhenChangingTheSourceCollection_ThenMembershipDoesNotChange`, `GivenAValuesView_WhenAddingAWord_ThenTheViewReflectsTheAddition`, and `GivenAStoredWord_WhenChangingItsIdentifier_ThenItsOriginalDictionaryKeyRemains`, corroborate each ownership boundary. [Shared-input generation tests](../../NuciGenerators.Text.UnitTests/NameGeneratorTests.cs) demonstrate the separate generator-list alias.

## Modification impact

A defensive copy, immutable word, key comparer change, overwrite policy, or list-backed replacement changes observable semantics. Preserve reference identity, literal key equality, lookup failures, and live views unless intentionally migrating consumers. Revisit [Data model](../data-model.md), [State](../state-and-persistence.md), and both generation and collection tests.
