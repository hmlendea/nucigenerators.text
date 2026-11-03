using System.Collections.Generic;

namespace NuciGenerators.Text.Models
{
    /// <summary>
    /// A word identified by a key and a collection of string values.
    /// </summary>
    public sealed class Word
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>The values.</value>
        public IEnumerable<string> Values { get; set; } = null!;
    }
}
