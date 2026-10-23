using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using NUnit.Framework;

using NuciGenerators.Text.Models;

namespace NuciGenerators.Text.UnitTests.Models
{
    [TestFixture]
    public sealed class GenerationSchemaTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("Oscar")]
        [TestCase("oscar")]
        [TestCase("OSCAR")]
        [TestCase("John Doe")]
        [TestCase("\u00c9lodia")]
        public void GivenIdenticalFields_WhenComparing_ThenEqualityIsSymmetricAndHashesMatch(string? value)
        {
            GenerationSchema first = BuildSchema(value, value, value, value);
            GenerationSchema second = BuildSchema(value, value, value, value);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(first.Equals(second));
                Assert.That(second.Equals(first));
                Assert.That(first.Equals((object)second));
                Assert.That(second.Equals((object)first));
                Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
                Assert.That(new HashSet<GenerationSchema> { first, second }, Has.Count.EqualTo(1));
            }
        }

        [Test]
        [Combinatorial]
        public void GivenMatchingNullableFieldCombinations_WhenComparing_ThenEqualityAndHashingRemainConsistent(
            [Values(null, "seed")] string? identifier,
            [Values(null, "Oscar")] string? name,
            [Values(null, "{first}{last}")] string? schema,
            [Values(null, "filters.txt")] string? filterlistPath)
        {
            GenerationSchema first = BuildSchema(identifier, name, schema, filterlistPath);
            GenerationSchema second = BuildSchema(identifier, name, schema, filterlistPath);
            GenerationSchema third = BuildSchema(identifier, name, schema, filterlistPath);

            Assert.That(first.Equals(second));
            Assert.That(second.Equals(third));
            Assert.That(first.Equals(third));
            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
        }

        [TestCase("Oscar", "oscar")]
        [TestCase("Oscar", "OSCAR")]
        [TestCase("Oscar", "Oscar ")]
        [TestCase("Oscar", " Oscar")]
        [TestCase("Oscar", "Robert")]
        [TestCase("Oscar", "")]
        [TestCase("Oscar", null)]
        [TestCase(null, "")]
        [TestCase("", " ")]
        [TestCase("\u00c9lodia", "Elodia")]
        public void GivenDifferentIdentifiers_WhenComparing_ThenSchemasAreNotEqual(string? first, string? second)
            => AssertUnequalSchemas(
                BuildSchema(first, "Oscar", "{first}", "filters.txt"),
                BuildSchema(second, "Oscar", "{first}", "filters.txt"));

        [TestCase("Oscar", "oscar")]
        [TestCase("Oscar", "OSCAR")]
        [TestCase("Oscar", "Oscar ")]
        [TestCase("Oscar", " Oscar")]
        [TestCase("Oscar", "Robert")]
        [TestCase("Oscar", "")]
        [TestCase("Oscar", null)]
        [TestCase(null, "")]
        [TestCase("", " ")]
        [TestCase("\u00c9lodia", "Elodia")]
        public void GivenDifferentNames_WhenComparing_ThenSchemasAreNotEqual(string? first, string? second)
            => AssertUnequalSchemas(
                BuildSchema("seed", first, "{first}", "filters.txt"),
                BuildSchema("seed", second, "{first}", "filters.txt"));

        [TestCase("{first}", "{last}")]
        [TestCase("{first}", "{FIRST}")]
        [TestCase("{first}", "{first} ")]
        [TestCase("{first}", " {first}")]
        [TestCase("{first}", "")]
        [TestCase("{first}", null)]
        [TestCase(null, "")]
        [TestCase("", " ")]
        public void GivenDifferentSchemas_WhenComparing_ThenSchemasAreNotEqual(string? first, string? second)
            => AssertUnequalSchemas(
                BuildSchema("seed", "Oscar", first, "filters.txt"),
                BuildSchema("seed", "Oscar", second, "filters.txt"));

        [TestCase("filters.txt", "other.txt")]
        [TestCase("filters.txt", "FILTERS.TXT")]
        [TestCase("filters.txt", "filters.txt ")]
        [TestCase("filters.txt", " filters.txt")]
        [TestCase("filters.txt", "")]
        [TestCase("filters.txt", null)]
        [TestCase(null, "")]
        [TestCase("", " ")]
        [TestCase("folder/filters.txt", "folder\\filters.txt")]
        public void GivenDifferentFilterlistPaths_WhenComparing_ThenSchemasAreNotEqual(string? first, string? second)
            => AssertUnequalSchemas(
                BuildSchema("seed", "Oscar", "{first}", first),
                BuildSchema("seed", "Oscar", "{first}", second));

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("Astora")]
        [TestCase("Romania")]
        public void GivenDifferentCategories_WhenComparing_ThenTheCategoryDoesNotParticipateInEquality(
            string? category)
        {
            GenerationSchema first = BuildSchema("seed", "Oscar", "{first}", "filters.txt");
            GenerationSchema second = BuildSchema("seed", "Oscar", "{first}", "filters.txt");
            first.Category = "Nucilandia";
            second.Category = category!;

            Assert.That(first.Equals(second));
            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
        }

        [Test]
        [Combinatorial]
        public void GivenDifferentWordCases_WhenComparing_ThenWordCaseDoesNotParticipateInEquality(
            [Values] WordCase firstCase,
            [Values] WordCase secondCase)
        {
            GenerationSchema first = BuildSchema("seed", "Oscar", "{first}", "filters.txt");
            GenerationSchema second = BuildSchema("seed", "Oscar", "{first}", "filters.txt");
            first.WordCase = firstCase;
            second.WordCase = secondCase;

            Assert.That(first.Equals(second));
            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("Oscar")]
        public void GivenASchema_WhenComparingToItself_ThenBothEqualityOverloadsAreReflexive(string? value)
        {
            GenerationSchema schema = BuildSchema(value, value, value, value);

            Assert.That(schema.Equals(schema));
            Assert.That(schema.Equals((object)schema));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("Oscar")]
        public void GivenASchema_WhenComparingToNull_ThenBothEqualityOverloadsReturnFalse(string? value)
        {
            GenerationSchema schema = BuildSchema(value, value, value, value);

            Assert.That(schema.Equals((GenerationSchema)null!), Is.False);
            Assert.That(schema!.Equals((object)null!), Is.False);
        }

        [TestCase("Oscar")]
        [TestCase(42)]
        [TestCase(true)]
        [TestCase(6.13)]
        public void GivenADifferentObjectType_WhenComparing_ThenObjectEqualityReturnsFalse(object other)
            => Assert.That(
                BuildSchema("seed", "Oscar", "{first}", "filters.txt").Equals(other),
                Is.False);

        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, false)]
        [TestCase(3, true)]
        [TestCase(4, true)]
        [TestCase(39, true)]
        [TestCase(40, true)]
        [TestCase(41, false)]
        [TestCase(64, false)]
        public void GivenAnIdentifierLength_WhenValidatingAnnotations_ThenDeclaredBoundsAreEnforced(
            int length,
            bool isValid)
        {
            GenerationSchema schema = BuildSchema(new string('a', length), "Oscar", "{first}", "filters.txt");

            Assert.That(ValidateSchema(schema), Is.EqualTo(isValid));
        }

        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(19, true)]
        [TestCase(20, true)]
        [TestCase(21, false)]
        [TestCase(64, false)]
        public void GivenANameLength_WhenValidatingAnnotations_ThenTheMaximumIsEnforced(int length, bool isValid)
        {
            GenerationSchema schema = BuildSchema("seed", new string('a', length), "{first}", "filters.txt");

            Assert.That(ValidateSchema(schema), Is.EqualTo(isValid));
        }

        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(59, true)]
        [TestCase(60, true)]
        [TestCase(61, false)]
        [TestCase(128, false)]
        public void GivenASchemaLength_WhenValidatingAnnotations_ThenTheMaximumIsEnforced(int length, bool isValid)
        {
            GenerationSchema schema = BuildSchema("seed", "Oscar", new string('a', length), "filters.txt");

            Assert.That(ValidateSchema(schema), Is.EqualTo(isValid));
        }

        [Test]
        public void GivenANewSchema_WhenValidating_ThenNullableFieldsAreAcceptedAndSentenceCaseIsTheDefault()
        {
            GenerationSchema schema = new();

            Assert.That(ValidateSchema(schema));
            Assert.That(schema.WordCase, Is.EqualTo(WordCase.Sentence));
        }

        private static GenerationSchema BuildSchema(
            string? identifier,
            string? name,
            string? schema,
            string? filterlistPath) => new()
        {
            Id = identifier!,
            Name = name!,
            Schema = schema!,
            FilterlistPath = filterlistPath!
        };

        private static void AssertUnequalSchemas(GenerationSchema first, GenerationSchema second)
        {
            Assert.That(first.Equals(second), Is.False);
            Assert.That(second.Equals(first), Is.False);
            Assert.That(first.Equals((object)second), Is.False);
            Assert.That(second.Equals((object)first), Is.False);
            Assert.That(new HashSet<GenerationSchema> { first, second }, Has.Count.EqualTo(2));
        }

        private static bool ValidateSchema(GenerationSchema schema)
            => Validator.TryValidateObject(schema, new ValidationContext(schema), [], true);
    }
}