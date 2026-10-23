using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using NuciGenerators.Text.Models;

namespace NuciGenerators.Text.UnitTests.Models
{
    [TestFixture]
    public sealed class WordlistTests
    {
        private Wordlist wordlist = null!;

        [SetUp]
        public void SetUp() => wordlist = new();

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(16)]
        [TestCase(64)]
        [TestCase(256)]
        [TestCase(1024)]
        public void GivenWords_WhenConstructing_ThenAllEnumerationPathsExposeTheOriginalInstances(int count)
        {
            Word[] words = BuildWords(count);
            Wordlist constructed = new(words);

            Assert.That(constructed.Values, Is.EqualTo(words));
            Assert.That(constructed.ToArray(), Is.EqualTo(words));
            Assert.That(((IEnumerable)constructed).Cast<Word>().ToArray(), Is.EqualTo(words));

            foreach (Word word in words)
            {
                Assert.That(constructed.Get(word.Id), Is.SameAs(word));
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(16)]
        [TestCase(64)]
        [TestCase(256)]
        public void GivenWords_WhenAdding_ThenEveryWordIsRetrievableAndEnumerated(int count)
        {
            Word[] words = BuildWords(count);

            foreach (Word word in words)
            {
                wordlist.Add(word);
            }

            Assert.That(wordlist.Values, Is.EqualTo(words));
            Assert.That(wordlist.ToArray(), Is.EqualTo(words));

            foreach (Word word in words)
            {
                Assert.That(wordlist.Get(word.Id), Is.SameAs(word));
            }
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t")]
        [TestCase("Oscar")]
        [TestCase("oscar")]
        [TestCase("OSCAR")]
        [TestCase("John Doe")]
        [TestCase("seed.with.dots")]
        [TestCase("seed/with/slashes")]
        [TestCase("\u00c9lodia")]
        public void GivenAStringIdentifier_WhenAddingAndGetting_ThenItIsPreservedLiterally(string identifier)
        {
            Word word = new() { Id = identifier, Values = ["Oscar", "Robert"] };
            wordlist.Add(word);

            Assert.That(wordlist.Get(identifier), Is.SameAs(word));
            Assert.That(wordlist.Get(identifier).Values, Is.EqualTo(new[] { "Oscar", "Robert" }));
        }

        [TestCase("Oscar")]
        [TestCase("oscar")]
        [TestCase("")]
        [TestCase(" ")]
        public void GivenADuplicateIdentifier_WhenAdding_ThenTheOriginalWordIsRetained(string identifier)
        {
            Word original = new() { Id = identifier, Values = ["Oscar"] };
            wordlist.Add(original);

            Assert.That(
                () => wordlist.Add(new() { Id = identifier, Values = ["Robert"] }),
                Throws.TypeOf<ArgumentException>());
            Assert.That(wordlist.Get(identifier), Is.SameAs(original));
            Assert.That(wordlist, Has.Exactly(1).Items);
        }

        [TestCase("Oscar")]
        [TestCase("")]
        [TestCase(" ")]
        public void GivenDuplicateIdentifiers_WhenConstructing_ThenAnArgumentExceptionIsThrown(string identifier)
            => Assert.That(
                () => new Wordlist([
                    new() { Id = identifier, Values = [] },
                    new() { Id = identifier, Values = [] }]),
                Throws.TypeOf<ArgumentException>());

        [TestCase("Oscar", "oscar")]
        [TestCase("Oscar", "OSCAR")]
        [TestCase("Oscar", "Oscar ")]
        [TestCase("Oscar", " Oscar")]
        [TestCase("", " ")]
        public void GivenDistinctLiteralIdentifiers_WhenAdding_ThenTheyAreNotNormalised(
            string firstIdentifier,
            string secondIdentifier)
        {
            Word first = new() { Id = firstIdentifier, Values = [] };
            Word second = new() { Id = secondIdentifier, Values = [] };
            wordlist.Add(first);
            wordlist.Add(second);

            Assert.That(wordlist.Get(firstIdentifier), Is.SameAs(first));
            Assert.That(wordlist.Get(secondIdentifier), Is.SameAs(second));
            Assert.That(wordlist, Has.Exactly(2).Items);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("missing")]
        [TestCase("oscar")]
        [TestCase("OSCAR")]
        public void GivenAMissingIdentifier_WhenGetting_ThenAKeyNotFoundExceptionIsThrown(string identifier)
        {
            wordlist.Add(new() { Id = "Oscar", Values = [] });

            Assert.That(() => wordlist.Get(identifier), Throws.TypeOf<KeyNotFoundException>());
            Assert.That(wordlist, Has.Exactly(1).Items);
        }

        [Test]
        public void GivenANullIdentifier_WhenGetting_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => wordlist.Get(null!),
                Throws.TypeOf<ArgumentNullException>());

        [Test]
        public void GivenANullWord_WhenAdding_ThenNoEntryIsInserted()
        {
            Assert.That(() => wordlist.Add(null!), Throws.TypeOf<NullReferenceException>());
            Assert.That(wordlist, Is.Empty);
        }

        [Test]
        public void GivenAWordWithANullIdentifier_WhenAdding_ThenNoEntryIsInserted()
        {
            Assert.That(
                () => wordlist.Add(new() { Id = null!, Values = [] }),
                Throws.TypeOf<ArgumentNullException>());
            Assert.That(wordlist, Is.Empty);
        }

        [Test]
        public void GivenANullSource_WhenConstructing_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => new Wordlist(null!),
                Throws.TypeOf<ArgumentNullException>());

        [Test]
        public void GivenANullSourceElement_WhenConstructing_ThenANullReferenceExceptionIsThrown()
            => Assert.That(
                () => new Wordlist(new Word[] { null! }),
                Throws.TypeOf<NullReferenceException>());

        [Test]
        public void GivenANullSourceIdentifier_WhenConstructing_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => new Wordlist([new() { Id = null!, Values = [] }]),
                Throws.TypeOf<ArgumentNullException>());

        [Test]
        public void GivenAConstructedWordlist_WhenChangingTheSourceCollection_ThenMembershipDoesNotChange()
        {
            Word original = new() { Id = "original", Values = ["Oscar"] };
            List<Word> source = [original];
            Wordlist constructed = new(source);
            source.Clear();
            source.Add(new() { Id = "replacement", Values = ["Robert"] });

            Assert.That(constructed, Is.EqualTo(new[] { original }));
            Assert.That(() => constructed.Get("replacement"), Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        public void GivenAValuesView_WhenAddingAWord_ThenTheViewReflectsTheAddition()
        {
            IEnumerable<Word> values = wordlist.Values;
            Word word = new() { Id = "seed", Values = ["Oscar"] };
            wordlist.Add(word);

            Assert.That(values, Is.EqualTo(new[] { word }));
        }

        [Test]
        public void GivenAStoredWord_WhenChangingItsValues_ThenRetrievalReflectsTheChange()
        {
            Word word = new() { Id = "seed", Values = ["Oscar"] };
            wordlist.Add(word);
            word.Values = ["Robert", "Ilarion"];

            Assert.That(wordlist.Get("seed").Values, Is.EqualTo(new[] { "Robert", "Ilarion" }));
        }

        [Test]
        public void GivenAStoredWord_WhenChangingItsIdentifier_ThenItsOriginalDictionaryKeyRemains()
        {
            Word word = new() { Id = "original", Values = [] };
            wordlist.Add(word);
            word.Id = "changed";

            Assert.That(wordlist.Get("original"), Is.SameAs(word));
            Assert.That(() => wordlist.Get("changed"), Throws.TypeOf<KeyNotFoundException>());
        }

        private static Word[] BuildWords(int count)
            => Enumerable.Range(0, count)
                .Select(number => new Word { Id = $"seed-{number}", Values = [$"value-{number}"] })
                .ToArray();
    }
}