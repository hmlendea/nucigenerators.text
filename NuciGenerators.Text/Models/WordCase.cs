namespace NuciGenerators.Text.Models
{
    /// <summary>
    /// The casing applied to generated words.
    /// </summary>
    public enum WordCase
    {
        /// <summary>
        /// The original casing is preserved.
        /// </summary>
        Original,

        /// <summary>
        /// Every letter is lowercase.
        /// </summary>
        Lower,

        /// <summary>
        /// Every letter is uppercase.
        /// </summary>
        Upper,

        /// <summary>
        /// The first word starts with an uppercase letter, and the rest are lowercase.
        /// </summary>
        Title,

        /// <summary>
        /// Each word starts with an uppercase letter, and the rest are lowercase.
        /// </summary>
        Sentence
    }
}
