using System;

namespace WarWolfWorks.Internal
{
    /// <summary>
    /// Exception thrown in case of an error in <see cref="WarWolfWorks.Utility.Hooks.RijndaelStreaming"/>.
    /// </summary>
    public sealed class RijndaelStreamingException : Exception
    {
        private string ActMessage;

        /// <summary>
        /// Message thrown based on <see cref="RijndaelStreamingResult"/>.
        /// </summary>
        public override string Message => ActMessage;

        /// <summary>
        /// Result of this exception.
        /// </summary>
        public RijndaelStreamingResult Result { get; private set; }

        /// <summary>
        /// Throws a <see cref="RijndaelStreamingException"/> based on given <see cref="RijndaelStreamingResult"/>.
        /// </summary>
        /// <param name="result"></param>
        public RijndaelStreamingException(RijndaelStreamingResult result) : base()
        {
            switch(result)
            {
                default: ActMessage = "Exception was incorrectly thrown."; break;
                case RijndaelStreamingResult.CATEGORY_PASSWORD_MISSMATCH:
                    ActMessage = "The password of a category does not match the password of the variable; Make sure all variables inside a category have the same password as the category.";
                    break;
                case RijndaelStreamingResult.DEFAULT_PATH_NULL:
                    ActMessage = "A RijndaelStreaming.Catalog was created using the default path, when default path is not set or is incorrect. Make sure to call RijndaelStreaming.SetDefaultPath(string) before using the default path.";
                    break;
                case RijndaelStreamingResult.INVALID_ARG:
                    ActMessage = "A string passed inside a catalog was null or empty.";
                    break;
                case RijndaelStreamingResult.INVALID_CATALOG:
                    ActMessage = "A Saver catalog was used inside a load method or vice-versa.";
                    break;
            }

            Result = result;
        }
    }
}
