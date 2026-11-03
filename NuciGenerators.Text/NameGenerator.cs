using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using NuciGenerators.Text.Models;

namespace NuciGenerators.Text
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NameGenerator"/> class.
    /// </summary>
    /// <param name="wordlists">Word lists.</param>
    public abstract class NameGenerator(List<Wordlist> wordlists) : INameGenerator
    {
        private const int DefaultMinimumNameLength = 5;
        private const int DefaultMaximumNameLength = 10;
        private const int DefaultMaximumProcessingTimePerWord = 1000;

        /// <summary>
        /// Gets or sets the minimum length of the name.
        /// </summary>
        /// <value>The minimum length of the name.</value>
        public int MinNameLength { get; set; } = DefaultMinimumNameLength;

        /// <summary>
        /// Gets or sets the maximum length of the name.
        /// </summary>
        /// <value>The maximum length of the name.</value>
        public int MaxNameLength { get; set; } = DefaultMaximumNameLength;

        /// <summary>
        /// Gets or sets the maximum processing time per word.
        /// </summary>
        /// <value>The maximum processing time per word, in milliseconds.</value>
        public int MaxProcessingTimePerWord { get; set; } = DefaultMaximumProcessingTimePerWord;

        /// <summary>
        /// Gets or sets a value indicating whether seed values are excluded from generated names.
        /// </summary>
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
        /// Gets the generated words.
        /// </summary>
        public List<string> GeneratedWords { get; protected set; } = [];

        /// <summary>
        /// Gets the word lists used to reject existing seed values.
        /// </summary>
        public List<Wordlist> Wordlists { get; protected set; } = wordlists;

        /// <summary>
        /// Gets the random source available to candidate-generation implementations.
        /// </summary>
        protected readonly Random random = new();

        /// <summary>
        /// Generates names.
        /// </summary>
        /// <returns>The names.</returns>
        /// <param name="maximumCount">Maximum count.</param>
        public IEnumerable<string> Generate(int maximumCount)
        {
            List<string> names = [];
            DateTime endTime = CalculateEndTime(maximumCount);
            DateTime currentTime = DateTime.Now;

            while (ShouldContinue(names.Count, maximumCount, currentTime, endTime))
            {
                AddCandidateIfValid(names, GenerationAlogrithm());
                currentTime = DateTime.Now;
            }

            return names;
        }

        private DateTime CalculateEndTime(int maximumCount)
            => DateTime.Now.AddMilliseconds(MaxProcessingTimePerWord * maximumCount);

        private static bool ShouldContinue(int currentCount, int maximumCount, DateTime currentTime, DateTime endTime)
            => currentCount < maximumCount && currentTime <= endTime;

        private void AddCandidateIfValid(List<string> names, string candidate)
        {
            if (!IsNameValid(candidate))
            {
                return;
            }

            names.Add(candidate);
            GeneratedWords.Add(candidate);
        }

        /// <summary>
        /// Reset the list of used names.
        /// </summary>
        public void Reset() => GeneratedWords.Clear();

        /// <summary>
        /// Generates one candidate name using the legacy extension-point spelling.
        /// </summary>
        /// <returns>A candidate name.</returns>
        protected virtual string GenerationAlogrithm() => GenerationAlgorithm();

        /// <summary>
        /// Generates one candidate name.
        /// </summary>
        /// <returns>A candidate name.</returns>
        protected virtual string GenerationAlgorithm()
            => throw new NotSupportedException("A candidate-generation algorithm must be overridden.");

        /// <summary>
        /// Checks whether the name is valid.
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
    }
}
