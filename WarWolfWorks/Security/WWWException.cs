using System;

namespace WarWolfWorks.Security
{
    internal abstract class WWWException : Exception
    {
        protected string ActMessage;

        public override string Message => ActMessage;
    }
}
