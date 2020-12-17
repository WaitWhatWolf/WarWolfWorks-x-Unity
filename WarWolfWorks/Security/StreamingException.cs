using System;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Security
{
    /// <summary>
    /// Exception thrown in case of an error in <see cref="Utility.Hooks.Streaming"/>.
    /// </summary>
    [Obsolete(V_SCATALOGSAVE_OBSOLETE_MESSAGE, V_SCATALOGSAVE_OBSOLETE_ISERROR)]
    public sealed class StreamingException : Exception
    {
        private string ActMessage;

        /// <summary>
        /// Message thrown based on <see cref="StreamingResult"/>.
        /// </summary>
        public override string Message => ActMessage;

        /// <summary>
        /// Result of this exception.
        /// </summary>
        public StreamingResult Result { get; private set; }

        /// <summary>
        /// Throws a <see cref="StreamingException"/> based on given <see cref="StreamingResult"/>.
        /// </summary>
        /// <param name="result"></param>
        public StreamingException(StreamingResult result) : base()
        {
            switch(result)
            {
                default: ActMessage = "Exception was incorrectly thrown."; break;
                case StreamingResult.CATEGORY_PASSWORD_MISSMATCH:
                    ActMessage = "The password of a category does not match the password of the variable; Make sure all variables inside a category have the same password as the category.";
                    break;
                case StreamingResult.DEFAULT_PATH_NULL:
                    ActMessage = "A RijndaelStreaming.Catalog was created using the default path, when default path is not set or is incorrect. Make sure to call RijndaelStreaming.SetDefaultPath(string) before using the default path.";
                    break;
                case StreamingResult.INVALID_ARG:
                    ActMessage = "A string passed inside a catalog was null or empty.";
                    break;
                case StreamingResult.INVALID_CATALOG:
                    ActMessage = "A Saver catalog was used inside a load method or vice-versa.";
                    break;
                case StreamingResult.CATALOG_MISSMATCH_CATEGORY:
                    ActMessage = "A SaveAll method was called where one or more catalogs did not have the same Category.";
                    break;
                case StreamingResult.CATALOG_MISSMATCH_FILEPATH:
                    ActMessage = "A SaveAll method was called where one or more catalogs did not have the same Path.";
                    break;
                case StreamingResult.CATALOG_MISSMATCH_PASSWORD:
                    ActMessage = "A SaveAll method was called where one or more catalogs did not have the same Password.";
                    break;
                case StreamingResult.INVALID_CATALOG_COLLECTION_SIZE:
                    ActMessage = "A SaveAll method was called where the catalog collection size given as argument was null or lesser than 2.";
                    break;
                case StreamingResult.INVALID_COLLECTION_SIZE:
                    ActMessage = "A collection of Catalogs was attempted where the array size of values did not match the array size of names.";
                    break;
            }

            Result = result;
        }
    }
}
