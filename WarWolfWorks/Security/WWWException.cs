using System;

namespace WarWolfWorks.Security
{
    /// <summary>
    /// Simple exception which details the line, source and message.
    /// </summary>
    public class WWWException : Exception
    {
#pragma warning disable 1591
        protected int Line;
        protected object MSource;
        protected string ActMessage;
#pragma warning restore 1591

        /// <summary>
        /// Formats the message based on <see cref="Line"/>, <see cref="MSource"/> and <see cref="ActMessage"/>.
        /// </summary>
        public override string Message => string.Format("{1} generated an exception at line {0}: {2}", Line, MSource, ActMessage);

        /// <summary>
        /// Base constructor of <see cref="WWWException"/>.
        /// </summary>
        /// <param name="line">The line at which the exception was generated.</param>
        /// <param name="mSource">Source of the exception.</param>
        /// <param name="message">Additional message.</param>
        public WWWException(int line, object mSource, string message)
        {
            Line = line;
            MSource = mSource;
            ActMessage = message;
        }
    }
}
