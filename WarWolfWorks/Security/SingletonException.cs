using System;
using WarWolfWorks.Utility;

namespace WarWolfWorks.Security
{
    /// <summary>
    /// Throws a singleton exception; Used by <see cref="Singleton{T}"/>.
    /// </summary>
    internal sealed class SingletonException : Exception
    {
        public override string Message => "Cannot create a singleton when an instance of same T type already exists.";
    }
}
