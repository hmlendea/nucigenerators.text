using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace NuciGenerators.Text.UnitTests
{
    [TestFixture]
    public sealed class NameGeneratorValidationTests
    {
        private ScriptedNameGenerator generator = null!;

        [SetUp]
        public void SetUp() => generator = new([]);

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase("\t")]
        [TestCase("\r")]
        [TestCase("\n")]
        [TestCase("\r\n")]
        [TestCase(" \t\r\n ")]
        [TestCase("\u00a0")]
        [TestCase("\u2003")]
        [TestCase("\u3000")]
        public void GivenABlankName_WhenValidating_ThenItIsRejected(string? name)
        {
            generator.MinNameLength = 0;

            Assert.That(generator.Validate(name!), Is.False);
        }

        [TestCase("Bob", false)]
        [TestCase("Mary", false)]
        [TestCase("Oscar", true)]
        [TestCase("Robert", true)]
        [TestCase("Ilarion", true)]
        [TestCase("Angetenar", true)]
        [TestCase("John Rambo", true)]
        [TestCase("Chuck Norris", false)]
        [TestCase("Testy McTestface", false)]
        public void GivenDefaultLengthLimits_WhenValidating_ThenTheLengthIsEnforced(
            string name,
            bool isValid)
            => Assert.That(
                generator.Validate(name),
                Is.EqualTo(isValid));

        [TestCase(0, 0, "a", false)]
        [TestCase(0, 1, "a", true)]
        [TestCase(1, 1, "a", true)]
        [TestCase(1, 1, "ab", false)]
        [TestCase(4, 8, "Bob", false)]
        [TestCase(4, 8, "Mary", true)]
        [TestCase(4, 8, "Oscar", true)]
        [TestCase(4, 8, "John Doe", true)]
        [TestCase(4, 8, "Angetenar", false)]
        [TestCase(4, 8, "John Wick", false)]
        [TestCase(5, 5, "Mary", false)]
        [TestCase(5, 5, "Oscar", true)]
        [TestCase(5, 5, "Robert", false)]
        [TestCase(6, 5, "Oscar", false)]
        [TestCase(-1, 5, "Oscar", true)]
        [TestCase(int.MinValue, int.MaxValue, "Oscar", true)]
        [TestCase(0, -1, "Oscar", false)]
        [TestCase(int.MaxValue, int.MaxValue, "Oscar", false)]
        [TestCase(5, 5, "\u00c9lena", true)]
        [TestCase(2, 2, "\ud83c\udf1e", true)]
        [TestCase(1, 1, "\ud83c\udf1e", false)]
        public void GivenCustomLengthLimits_WhenValidating_ThenBoundariesAreInclusive(
            int minimumLength,
            int maximumLength,
            string name,
            bool isValid)
        {
            generator.MinNameLength = minimumLength;
            generator.MaxNameLength = maximumLength;

            Assert.That(generator.Validate(name), Is.EqualTo(isValid));
        }

        [TestCase("", "", true)]
        [TestCase("Os", "", true)]
        [TestCase("", "ar", true)]
        [TestCase("Os", "ar", true)]
        [TestCase("Oscar", "Oscar", true)]
        [TestCase("os", "", false)]
        [TestCase("", "AR", false)]
        [TestCase("os", "AR", false)]
        [TestCase("ar", "", false)]
        [TestCase("", "Os", false)]
        [TestCase("Oscars", "", false)]
        [TestCase("", "Oscars", false)]
        [TestCase("^Os", "", false)]
        [TestCase("", "ar$", false)]
        [TestCase(" ", "", false)]
        [TestCase("", " ", false)]
        public void GivenPrefixAndSuffixFilters_WhenValidating_ThenLiteralCaseSensitiveMatchingIsUsed(
            string prefix,
            string suffix,
            bool isValid)
        {
            generator.StartsWithFilter = prefix;
            generator.EndsWithFilter = suffix;

            Assert.That(generator.Validate("Oscar"), Is.EqualTo(isValid));
        }

        [TestCase("Oscar", "Oscar", false, false)]
        [TestCase("Oscar", "Oscar", false, true)]
        [TestCase("Oscar", "oscar", true, false)]
        [TestCase("Oscar", "oscar", true, true)]
        [TestCase("Oscar", "OSCAR", true, false)]
        [TestCase("Oscar", "OSCAR", true, true)]
        [TestCase("Oscar", "Oscars", true, false)]
        [TestCase("Oscar", "Oscars", true, true)]
        [TestCase("Oscar", "Osc", true, false)]
        [TestCase("Oscar", "Osc", true, true)]
        [TestCase("Oscar", "", true, false)]
        [TestCase("Oscar", "", true, true)]
        [TestCase("Mary", "Mary", false, false)]
        [TestCase("Mary", "Mary", false, true)]
        [TestCase("Mary", "mary", true, false)]
        [TestCase("Mary", "mary", true, true)]
        public void GivenGenerationHistory_WhenValidating_ThenOnlyExactDuplicatesAreRejected(
            string name,
            string previousName,
            bool isValid,
            bool onlyNewNames)
        {
            generator.MinNameLength = 1;
            generator.OnlyNewNames = onlyNewNames;
            generator.GeneratedWords.Add(previousName);

            Assert.That(generator.Validate(name), Is.EqualTo(isValid));
        }

        [TestCase("Oscar", false, false)]
        [TestCase("Oscar", false, true)]
        [TestCase("Robert", false, false)]
        [TestCase("Robert", false, true)]
        [TestCase("Ilarion", false, false)]
        [TestCase("Ilarion", false, true)]
        [TestCase("Angetenar", false, false)]
        [TestCase("Angetenar", false, true)]
        [TestCase("oscar", true, false)]
        [TestCase("oscar", true, true)]
        [TestCase("OSCAR", true, false)]
        [TestCase("OSCAR", true, true)]
        [TestCase("Oscars", true, false)]
        [TestCase("Oscars", true, true)]
        [TestCase("Mary", true, false)]
        [TestCase("Mary", true, true)]
        public void GivenMultipleSeedLists_WhenValidating_ThenEverySeedValueIsConsidered(
            string name,
            bool isNovel,
            bool onlyNewNames)
        {
            generator.MinNameLength = 1;
            generator.OnlyNewNames = onlyNewNames;
            generator.Wordlists.Add(new());
            generator.Wordlists.Add(new([new() { Id = "first", Values = ["Oscar", "Robert"] }]));
            generator.Wordlists.Add(new([new() { Id = "second", Values = ["Ilarion", "Angetenar"] }]));

            Assert.That(generator.Validate(name), Is.EqualTo(!onlyNewNames || isNovel));
        }

        [TestCase("Oscar", true, false)]
        [TestCase("Oscar", true, true)]
        [TestCase("^Oscar$", true, false)]
        [TestCase("^Oscar$", true, true)]
        [TestCase("^Os", true, false)]
        [TestCase("^Os", true, true)]
        [TestCase("ar$", true, false)]
        [TestCase("ar$", true, true)]
        [TestCase("[A-Z][a-z]+", true, false)]
        [TestCase("[A-Z][a-z]+", true, true)]
        [TestCase("^.{5}$", true, false)]
        [TestCase("^.{5}$", true, true)]
        [TestCase("sc", true, false)]
        [TestCase("sc", true, true)]
        [TestCase(".", true, false)]
        [TestCase(".", true, true)]
        [TestCase("", true, false)]
        [TestCase("", true, true)]
        [TestCase("(?i)oscar", true, false)]
        [TestCase("(?i)oscar", true, true)]
        [TestCase("^(Oscar|Robert)$", true, false)]
        [TestCase("^(Oscar|Robert)$", true, true)]
        [TestCase("\\bOscar\\b", true, false)]
        [TestCase("\\bOscar\\b", true, true)]
        [TestCase("[0-9]", false, false)]
        [TestCase("[0-9]", false, true)]
        [TestCase("^oscar$", false, false)]
        [TestCase("^oscar$", false, true)]
        [TestCase("^Robert$", false, false)]
        [TestCase("^Robert$", false, true)]
        [TestCase("^.{4}$", false, false)]
        [TestCase("^.{4}$", false, true)]
        [TestCase("^.{6}$", false, false)]
        [TestCase("^.{6}$", false, true)]
        [TestCase("^ar", false, false)]
        [TestCase("^ar", false, true)]
        [TestCase("Os$", false, false)]
        [TestCase("Os$", false, true)]
        [TestCase("\\s", false, false)]
        [TestCase("\\s", false, true)]
        [TestCase("\\.", false, false)]
        [TestCase("\\.", false, true)]
        public void GivenARegexPattern_WhenValidating_ThenInclusionAndExclusionHaveOppositeEffects(
            string pattern,
            bool doesMatch,
            bool isExclusion)
        {
            List<string> patterns = generator.IncludedStrings;

            if (isExclusion)
            {
                patterns = generator.ExcludedStrings;
            }

            patterns.Add(pattern);

            Assert.That(generator.Validate("Oscar"), Is.EqualTo(doesMatch != isExclusion));
        }

        [TestCase("^O", "ar$", true)]
        [TestCase("^O", "z$", false)]
        [TestCase("^Z", "ar$", false)]
        [TestCase("^Z", "z$", false)]
        [TestCase("", "", true)]
        [TestCase("Oscar", "Oscar", true)]
        public void GivenMultipleRequiredPatterns_WhenValidating_ThenAllMustMatch(
            string firstPattern,
            string secondPattern,
            bool isValid)
        {
            generator.IncludedStrings.AddRange([firstPattern, secondPattern]);

            Assert.That(generator.Validate("Oscar"), Is.EqualTo(isValid));
        }

        [TestCase("^O", "ar$", false)]
        [TestCase("^O", "z$", false)]
        [TestCase("^Z", "ar$", false)]
        [TestCase("^Z", "z$", true)]
        [TestCase("", "", false)]
        [TestCase("Robert", "Mary", true)]
        public void GivenMultipleExcludedPatterns_WhenValidating_ThenAnyMatchRejectsTheName(
            string firstPattern,
            string secondPattern,
            bool isValid)
        {
            generator.ExcludedStrings.AddRange([firstPattern, secondPattern]);

            Assert.That(generator.Validate("Oscar"), Is.EqualTo(isValid));
        }

        [TestCase("^O", "z$", true)]
        [TestCase("^O", "ar$", false)]
        [TestCase("^Z", "z$", false)]
        [TestCase("^Z", "ar$", false)]
        public void GivenBothPatternLists_WhenValidating_ThenBothConstraintsMustBeSatisfied(
            string includedPattern,
            string excludedPattern,
            bool isValid)
        {
            generator.IncludedStrings.Add(includedPattern);
            generator.ExcludedStrings.Add(excludedPattern);

            Assert.That(generator.Validate("Oscar"), Is.EqualTo(isValid));
        }

        [TestCase("[", false)]
        [TestCase("[", true)]
        [TestCase("(", false)]
        [TestCase("(", true)]
        [TestCase("*", false)]
        [TestCase("*", true)]
        [TestCase("\\", false)]
        [TestCase("\\", true)]
        public void GivenAnInvalidPattern_WhenValidating_ThenTheRegexErrorIsPropagated(
            string pattern,
            bool isExclusion)
        {
            if (isExclusion)
            {
                generator.ExcludedStrings.Add(pattern);
            }
            else
            {
                generator.IncludedStrings.Add(pattern);
            }

            Assert.That(() => generator.Validate("Oscar"), Throws.InstanceOf<ArgumentException>());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("Mary")]
        [TestCase("Testy McTestface")]
        public void GivenAnInvalidNameAndMalformedPatterns_WhenValidating_ThenLengthGuardsRunFirst(
            string? name)
        {
            generator.IncludedStrings.Add("[");
            generator.ExcludedStrings.Add("[");

            Assert.That(generator.Validate(name!), Is.False);
        }

        [TestCase("Oscar", true)]
        [TestCase("Mary", false)]
        public void GivenACandidate_WhenValidatingRepeatedly_ThenHistoryIsNotModified(
            string name,
            bool isValid)
        {
            Assert.That(generator.Validate(name), Is.EqualTo(isValid));
            Assert.That(generator.Validate(name), Is.EqualTo(isValid));
            Assert.That(generator.GeneratedWords, Is.Empty);
        }
    }
}