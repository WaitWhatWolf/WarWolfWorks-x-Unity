using System;

namespace WarWolfWorks.Attributes
{
    /// <summary>
    /// Attribute which removes all "s_" or "S_" for every field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CompleteNoS : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="CompleteNoS"/>.
        /// </summary>
        public CompleteNoS()
        {

        }
    }
}
