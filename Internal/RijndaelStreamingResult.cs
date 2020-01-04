namespace WarWolfWorks.Internal
{
    /// <summary>
    /// Code of a <see cref="RijndaelStreamingException"/>.
    /// </summary>
    public enum RijndaelStreamingResult
    {
        /// <summary>
        /// No exception.
        /// </summary>
        OK,
        /// <summary>
        /// The category's password was not the same as it's variable.
        /// </summary>
        CATEGORY_PASSWORD_MISSMATCH,
        /// <summary>
        /// The category's default path was used without being set.
        /// </summary>
        DEFAULT_PATH_NULL,
        /// <summary>
        /// One or more string values given inside a <see cref="WarWolfWorks.Utility.Hooks.RijndaelStreaming.Catalog"/> were invalid or null.
        /// </summary>
        INVALID_ARG,
        /// <summary>
        /// A saving catalog was used to load a variable or vice-versa.
        /// </summary>
        INVALID_CATALOG,
    }
}
