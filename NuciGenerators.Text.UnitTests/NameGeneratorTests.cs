using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using NuciGenerators.Text.Models;

namespace NuciGenerators.Text.UnitTests
{
    [TestFixture]
    public sealed class NameGeneratorTests
    {
        private ScriptedNameGenerator generator = null!;

        [SetUp]
        public void SetUp() => generator = new([]);

        [Test]
        public void GivenANewGenerator_WhenInspectingDefaults_ThenGenerationStartsUnrestrictedExceptForLength()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(generator.MinNameLength, Is.EqualTo(5));
                Assert.That(generator.MaxNameLength, Is.EqualTo(10));
                Assert.That(generator.MaxProcessingTimePerWord, Is.EqualTo(1000));
                Assert.That(generator.OnlyNewNames);
                Assert.That(generator.StartsWithFilter, Is.Empty);
                Assert.That(generator.EndsWithFilter, Is.Empty);
                Assert.That(generator.IncludedStrings, Is.Empty);
                Assert.That(generator.ExcludedStrings, Is.Empty);
                Assert.That(generator.GeneratedWords, Is.Empty);
                Assert.That(generator.Wordlists, Is.Empty);
            }
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-4)]
        [TestCase(-42)]
        [TestCase(int.MinValue)]
        public void GivenANonPositiveCount_WhenGenerating_ThenTheAlgorithmIsNotCalled(int count)
        {
            generator.MaxProcessingTimePerWord = 1;
            generator.GeneratedWords.Add("Oscar");

            Assert.That(generator.Generate(count), Is.Empty);
            Assert.That(generator.AlgorithmCalls, Is.Zero);
            Assert.That(generator.GeneratedWords, Is.EqualTo(new[] { "Oscar" }));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(8)]
        [TestCase(16)]
        [TestCase(32)]
        [TestCase(64)]
        [TestCase(128)]
        [TestCase(256)]
        [TestCase(512)]
        [TestCase(1024)]
        public void GivenEnoughValidCandidates_WhenGenerating_ThenExactlyTheRequestedNamesAreRecorded(int count)
        {
            string[] candidates = BuildCandidates(count);
            generator.Enqueue(candidates);

            IEnumerable<string> names = generator.Generate(count);

            Assert.That(names, Is.EqualTo(candidates));
            Assert.That(generator.GeneratedWords, Is.EqualTo(candidates));
            Assert.That(generator.AlgorithmCalls, Is.EqualTo(count));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("Mary")]
        [TestCase("Testy McTestface")]
        public void GivenInvalidCandidatesBeforeAValidOne_WhenGenerating_ThenOnlyTheValidNameIsRecorded(
            string? invalidName)
        {
            generator.Enqueue(invalidName!, "Oscar");

            Assert.That(generator.Generate(1), Is.EqualTo(new[] { "Oscar" }));
            Assert.That(generator.GeneratedWords, Is.EqualTo(new[] { "Oscar" }));
            Assert.That(generator.AlgorithmCalls, Is.EqualTo(2));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GivenRepeatedCandidates_WhenGenerating_ThenDuplicatesAreSkippedEvenWhenSeedNamesAreAllowed(
            bool onlyNewNames)
        {
            generator.OnlyNewNames = onlyNewNames;
            generator.Enqueue("Oscar", "Oscar", "Robert", "Robert", "Ilarion");

            Assert.That(generator.Generate(3), Is.EqualTo(new[] { "Oscar", "Robert", "Ilarion" }));
            Assert.That(generator.GeneratedWords, Is.EqualTo(new[] { "Oscar", "Robert", "Ilarion" }));
            Assert.That(generator.AlgorithmCalls, Is.EqualTo(5));
        }

        [TestCase(1)]
        [TestCase(4)]
        [TestCase(16)]
        [TestCase(64)]
        public void GivenPreviousGeneration_WhenGeneratingAgain_ThenHistoryIsAppendedAndResultsAreIndependent(int count)
        {
            string[] candidates = BuildCandidates(count * 2);
            generator.Enqueue(candidates);
            IEnumerable<string> firstBatch = generator.Generate(count);
            IEnumerable<string> secondBatch = generator.Generate(count);

            Assert.That(firstBatch, Is.EqualTo(candidates.Take(count)));
            Assert.That(secondBatch, Is.EqualTo(candidates.Skip(count)));
            Assert.That(generator.GeneratedWords, Is.EqualTo(candidates));
        }

        [TestCase(-1)]
        [TestCase(-4)]
        [TestCase(-1000)]
        public void GivenAnAlreadyExpiredBudget_WhenGenerating_ThenNoCandidateIsRequested(int budget)
        {
            generator.MaxProcessingTimePerWord = budget;

            Assert.That(generator.Generate(1), Is.Empty);
            Assert.That(generator.GeneratedWords, Is.Empty);
            Assert.That(generator.AlgorithmCalls, Is.Zero);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(64)]
        public void GivenGenerationHistory_WhenResetting_ThenTheExistingHistoryCollectionIsCleared(int count)
        {
            List<string> history = generator.GeneratedWords;
            history.AddRange(BuildCandidates(count));
            generator.StartsWithFilter = "O";
            generator.IncludedStrings.Add("ar");

            generator.Reset();
            generator.Reset();

            Assert.That(generator.GeneratedWords, Is.SameAs(history).And.Empty);
            Assert.That(generator.StartsWithFilter, Is.EqualTo("O"));
            Assert.That(generator.IncludedStrings, Is.EqualTo(new[] { "ar" }));
        }

        [TestCase("Oscar")]
        [TestCase("Robert")]
        [TestCase("Ilarion")]
        public void GivenAPreviouslyGeneratedName_WhenResetting_ThenItCanBeGeneratedAgain(string name)
        {
            generator.Enqueue(name, name);
            IEnumerable<string> originalNames = generator.Generate(1);

            generator.Reset();

            Assert.That(generator.Generate(1), Is.EqualTo(new[] { name }));
            Assert.That(originalNames, Is.EqualTo(new[] { name }));
            Assert.That(generator.GeneratedWords, Is.EqualTo(new[] { name }));
        }

        [Test]
        public void GivenAFailingAlgorithm_WhenGenerating_ThenItsExceptionIsPropagatedWithoutRecordingAName()
        {
            Assert.That(() => generator.Generate(1), Throws.TypeOf<InvalidOperationException>());
            Assert.That(generator.GeneratedWords, Is.Empty);
        }

        [Test]
        public void GivenAnAlgorithmFailureAfterSuccess_WhenGenerating_ThenSuccessfulHistoryIsRetained()
        {
            generator.Enqueue("Oscar");

            Assert.That(() => generator.Generate(2), Throws.TypeOf<InvalidOperationException>());
            Assert.That(generator.GeneratedWords, Is.EqualTo(new[] { "Oscar" }));
        }

        [Test]
        public void GivenSeparateGenerators_WhenMutatingOne_ThenTheirStateIsIsolated()
        {
            ScriptedNameGenerator other = new([]);
            generator.GeneratedWords.Add("Oscar");
            generator.ExcludedStrings.Add("Oscar");
            generator.IncludedStrings.Add("Robert");
            generator.Wordlists.Add(new());

            Assert.That(other.GeneratedWords, Is.Empty);
            Assert.That(other.ExcludedStrings, Is.Empty);
            Assert.That(other.IncludedStrings, Is.Empty);
            Assert.That(other.Wordlists, Is.Empty);
            Assert.That(other.Validate("Oscar"));
        }

        [Test]
        public void GivenSharedWordlists_WhenAddingASeedAfterConstruction_ThenValidationUsesTheNewSeed()
        {
            List<Wordlist> wordlists = [];
            ScriptedNameGenerator sharedGenerator = new(wordlists);
            wordlists.Add(new([new() { Id = "seed", Values = ["Oscar"] }]));

            Assert.That(sharedGenerator.Wordlists, Is.SameAs(wordlists));
            Assert.That(sharedGenerator.Validate("Oscar"), Is.False);
        }

        private static string[] BuildCandidates(int count)
            => Enumerable.Range(0, count).Select(number => $"N{number:D8}").ToArray();
    }
}