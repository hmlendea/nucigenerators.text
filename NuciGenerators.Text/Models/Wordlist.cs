using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NuciGenerators.Text.Models
{
    /// <summary>
    /// A keyed collection of words.
    /// </summary>
    public sealed class Wordlist : IEnumerable<Word>
    {
        private readonly Dictionary<string, Word> words;

        /// <summary>
        /// Gets the words in the list.
        /// </summary>
        /// <value>The words in the list.</value>
        public IEnumerable<Word> Values => words.Values;

        /// <summary>
        /// Initializes a new instance of the <see cref="Wordlist"/> class.
        /// </summary>
        public Wordlist() => words = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="Wordlist"/> class.
        /// </summary>
        /// <param name="words">The words.</param>
        public Wordlist(IEnumerable<Word> words)
            => this.words = words.ToDictionary(word => word.Id, word => word);
        /// <summary>
        /// Adds a word.
        /// </summary>
        /// <param name="word">The word.</param>
        public void Add(Word word)
            => words.Add(word.Id, word);

        /// <summary>
        /// Gets the word with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public Word Get(string id)
            => words[id];

        /// <summary>
        /// Returns an enumerator that iterates through the words.
        /// </summary>
        public IEnumerator<Word> GetEnumerator() => Values.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the words.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
