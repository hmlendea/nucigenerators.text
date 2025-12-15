using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text.RegularExpressions;

using NuciExtensions;
using NuciGenerators.Text.Models;

namespace NuciGenerators.Text
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NameGenerator"/> class.
    /// </summary>
    /// <param name="wordlists">Word lists.</param>
    public abstract class NameGenerator(List<Wordlist> wordlists) : INameGenerator
    {
        /// <summary>
        /// Gets or sets the minimum length of the name.
        /// </summary>
        /// <value>The minimum length of the name.</value>
        public int MinNameLength { get; set; } = 5;

        /// <summary>
        /// Gets or sets the maximum length of the name.
        /// </summary>
        /// <value>The maximum length of the name.</value>
        public int MaxNameLength { get; set; } = 10;

        /// <summary>
        /// Gets or sets the maximum processing time per word.
        /// </summary>
        /// <value>The maximum processing time per word, in milliseconds.</value>
        public int MaxProcessingTimePerWord { get; set; } = 1000;

        public bool OnlyNewNames { get; set; } = true;

        /// <summary>
        /// Gets or sets the excluded strings.
        /// </summary>
        /// <value>The excluded strings.</value>
        public List<string> ExcludedStrings { get; set; } = [];

        /// <summary>
        /// Gets or sets the included strings.
        /// </summary>
        /// <value>The included strings.</value>
        public List<string> IncludedStrings { get; set; } = [];

        /// <summary>
        /// Gets or sets the string that all generated names must start with.
        /// </summary>
        /// <value>The string start filter.</value>
        public string StartsWithFilter { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the string that all generated names must end with.
        /// </summary>
        /// <value>The string end filter.</value>
        public string EndsWithFilter { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the used words.
        /// </summary>
        /// <value>The used words.</value>
        public List<string> GeneratedWords { get; protected set; } = [];

        public List<Wordlist> Wordlists { get; protected set; } = wordlists;

        protected readonly Random random = new();

        /// <summary>
        /// Generates names based on a schema.
        /// </summary>
        /// <param name="schema">The generation schema.</param>
        /// <param name="maximumCount">The maximum number of names to generate.</param>
        /// <returns>A collection of generated names.</returns>
        public IEnumerable<string> Generate(string schema, int maximumCount)
            => Generate(schema, [], maximumCount);

        /// <summary>
        /// Generates names based on a schema.
        /// </summary>
        /// <param name="schema">The generation schema.</param>
        /// <param name="maximumCount">The maximum number of names to generate.</param>
        /// <param name="casing">The casing of the generated names.</param>
        /// <returns>A collection of generated names.</returns>
        public IEnumerable<string> Generate(string schema, int maximumCount, WordCase casing)
            => Generate(schema, [], maximumCount, casing);

        /// <summary>
        /// Generates names based on a schema.
        /// </summary>
        /// <param name="schema">The generation schema.</param>
        /// <param name="filters">The blacklist filters.</param>
        /// <param name="maximumCount">The maximum number of names to generate.</param>
        /// <returns>A collection of generated names.</returns>
        public IEnumerable<string> Generate(string schema, List<string> filters, int maximumCount)
            => Generate(schema, filters, maximumCount, WordCase.Original);

        /// <summary>
        /// Generates names based on a schema.
        /// </summary>
        /// <param name="schema">The generation schema.</param>
        /// <param name="filters">The blacklist filters.</param>
        /// <param name="maximumCount">The maximum number of names to generate.</param>
        /// <param name="casing">The casing of the generated names.</param>
        /// <returns>A collection of generated names.</returns>
        public IEnumerable<string> Generate(string schema, List<string> filters, int maximumCount, WordCase casing)
        {
            List<List<string>> z = [];
            List<string> names = [];

            int generatorsCount = schema.Count(x => x.Equals('{'));

            while (z.Count < generatorsCount)
            {
                string name = schema;
                string currentGeneration = schema;

                while (currentGeneration.Contains('{') || currentGeneration.Contains('}'))
                {
                    int pos = currentGeneration.IndexOf('{') + 1;
                    string com = currentGeneration[pos..currentGeneration.IndexOf('}')];
                    IEnumerable<string> values = [];

                    string[] split = com.Split(',');

                    values = GenerateBySchema(schema, maximumCount, split, filters);

                    currentGeneration = currentGeneration.Replace("{" + com + "}", string.Empty);
                    z.Add([.. values]);
                }
            }

            for (int i = 0; i < z.Min(x => x.Count); i++)
            {
                string name = string.Empty;

                z.ForEach(x => name += x[i]);

                name = GetNameWithCasing(name, casing);
                names.Add(name);
            }

            return names;
        }

        /// <summary>
        /// Generates names.
        /// </summary>
        /// <returns>The names.</returns>
        /// <param name="maximumCount">Maximum count.</param>
        public IEnumerable<string> Generate(int maximumCount)
        {
            List<string> names = [];

            DateTime startTime = DateTime.Now;
            DateTime currentTime = DateTime.Now;
            DateTime endTime = startTime.AddMilliseconds(MaxProcessingTimePerWord * maximumCount);

            while (names.Count < maximumCount && currentTime <= endTime)
            {
                string name = GenerationAlogrithm();

                if (IsNameValid(name))
                {
                    names.Add(name);
                    GeneratedWords.Add(name);
                }

                currentTime = DateTime.Now;
            }

            return names;
        }

        /// <summary>
        /// Reset the list of used names.
        /// </summary>
        public void Reset() => GeneratedWords.Clear();

        protected abstract string GenerationAlogrithm();

        /// <summary>
        /// Generates names based on a schema.
        /// </summary>
        /// <param name="schema">The generation schema.</param>
        /// <param name="maximumCount">The maximum number of names to generate.</param>
        /// <param name="split">The schema split parts.</param>
        /// <param name="filters">The blacklist filters.</param>
        /// <returns>A collection of generated names.</returns>
        protected abstract IEnumerable<string> GenerateBySchema(string schema, int maximumCount, string[] split, List<string> filters);

        /// <summary>
        /// Checks wether the the name is valid.
        /// </summary>
        /// <returns><c>true</c>, if name is valid, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        protected bool IsNameValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (name.Length < MinNameLength || name.Length > MaxNameLength)
            {
                return false;
            }

            if (!name.StartsWith(StartsWithFilter, StringComparison.InvariantCulture) ||
                !name.EndsWith(EndsWithFilter, StringComparison.InvariantCulture))
            {
                return false;
            }

            // The same name was previously generated
            if (GeneratedWords.Contains(name))
            {
                return false;
            }

            // The same name was part of the seed wordlists
            if (OnlyNewNames && Wordlists.Any(wl => wl.Any(w => w.Values.Contains(name))))
            {
                return false;
            }

            // The name contains a blacklisted pattern
            if (ExcludedStrings.Any(p => Regex.IsMatch(name, p)))
            {
                return false;
            }

            // The name does not contain a mandatory pattern
            if (!IncludedStrings.All(p => Regex.IsMatch(name, p)))
            {
                return false;
            }

            return true;
        }

        private static string GetNameWithCasing(string name, WordCase casing)
        {
            if (casing.Equals(WordCase.Lower))
            {
                return name.ToLower();
            }

            if (casing.Equals(WordCase.Upper))
            {
                return name.ToUpper();
            }

            if (casing.Equals(WordCase.Title))
            {
                return name.ToTitleCase();
            }

            if (casing.Equals(WordCase.Sentence))
            {
                return name.ToSentenceCase();
            }

            return name;
        }
    }
}
