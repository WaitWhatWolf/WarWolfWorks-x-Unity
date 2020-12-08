using System;
using static WarWolfWorks.Utility.Hooks.Streaming;
using static WarWolfWorks.Constants;

namespace WarWolfWorks.Security
{
    /// <summary>
    /// Code of a <see cref="StreamingException"/>.
    /// </summary>
    [Obsolete(V_SCATALOGSAVE_OBSOLETE_MESSAGE, V_SCATALOGSAVE_OBSOLETE_ISERROR)]
    public enum StreamingResult
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
        /// One or more string values given inside a <see cref="WarWolfWorks.Utility.Hooks.Streaming.Catalog"/> were invalid or null.
        /// </summary>
        INVALID_ARG,
        /// <summary>
        /// A saving catalog was used to load a variable or vice-versa.
        /// </summary>
        INVALID_CATALOG,
        /// <summary>
        /// A SaveAll method was called where all catalogs did not have the same Category value.
        /// </summary>
        CATALOG_MISSMATCH_CATEGORY,
        /// <summary>
        /// A SaveAll method was called where all catalogs did not have the same Password value.
        /// </summary>
        CATALOG_MISSMATCH_PASSWORD,
        /// <summary>
        /// A SaveAll method was called where all catalogs did not have the same Path value.
        /// </summary>
        CATALOG_MISSMATCH_FILEPATH,
        /// <summary>
        /// A SaveAll method was called where the catalog collection size was null or lesser than 2.
        /// </summary>
        INVALID_CATALOG_COLLECTION_SIZE,
        /// <summary>
        /// Attempted to create multiple catalogs using <see cref="Catalog.Savers(string, string, string[], string[])"/> where 
        /// the length of names did not match the length of values.
        /// </summary>
        INVALID_COLLECTION_SIZE
    }
}
