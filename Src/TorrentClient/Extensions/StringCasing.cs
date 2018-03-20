namespace TorrentClient.Extensions
{
    /// <summary>
    /// Determines the casing of the string
    /// </summary>
    public enum StringCasing
    {
        /// <summary>
        /// Ignore casing string casing.
        /// </summary>
        None,

        /// <summary>
        /// Upper-case string (invariant) string casing.
        /// </summary>
        Upper,

        /// <summary>
        /// Lower-case string (invariant) string casing.
        /// </summary>
        Lower,

        /// <summary>
        /// Sentence case string string casing.
        /// </summary>
        Sentence,

        /// <summary>
        /// Word casing string casing.
        /// </summary>
        Title,

        /// <summary>
        /// Capital word case string casing.
        /// </summary>
        CapitalWord,

        /// <summary>
        /// Snake case string casing.
        /// </summary>
        SnakeCase,

        /// <summary>
        /// Camel case string casing.
        /// </summary>
        CamelCase
    }
}
