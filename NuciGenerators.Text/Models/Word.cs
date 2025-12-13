using System.Collections.Generic;

namespace NuciGenerators.Text.Models
{
    public sealed class Word
    {
        public string Id { get; set; }

        public IEnumerable<string> Values { get; set; }
    }
}
