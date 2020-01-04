using System;
using System.Runtime.Serialization;

namespace WarWolfWorks.Internal
{
    internal class WWWException : Exception
    {
        protected string SetMessage { get; set; }

        public override string Message => SetMessage;

        public WWWException(string exceptionMessage)
        {
            SetMessage = $"An exception occured in WWWLibrary: {exceptionMessage} at {StackTrace}";
        }

        public WWWException()
        {
            SetMessage = $"An exception occured in WWWLibrary at {StackTrace}";
        }

        public override string ToString()
        {
            return SetMessage;
        }
    }
}
