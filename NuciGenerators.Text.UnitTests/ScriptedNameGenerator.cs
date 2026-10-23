using System.Collections.Generic;

using NuciGenerators.Text.Models;

namespace NuciGenerators.Text.UnitTests
{
    internal sealed class ScriptedNameGenerator(List<Wordlist> wordlists) : NameGenerator(wordlists)
    {
        private readonly Queue<string> candidates = new();

        public int AlgorithmCalls { get; private set; }

        public bool Validate(string name) => IsNameValid(name);

        public void Enqueue(params string[] names)
        {
            foreach (string name in names)
            {
                candidates.Enqueue(name);
            }
        }

        protected override string GenerationAlogrithm()
        {
            AlgorithmCalls += 1;

            return candidates.Dequeue();
        }
    }
}