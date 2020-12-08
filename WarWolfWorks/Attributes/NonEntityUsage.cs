using System;

namespace WarWolfWorks.Attributes
{
    /// <summary>
    /// Use this attribute if a Stat isn't always owned by an Entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class NonEntityUsage : Attribute
    {
        /// <summary>
        /// If true, it means that the stat will never be used by an Entity, not just temporary.
        /// </summary>
        public bool NeverUsed;

        /// <summary>
        /// Determines a Stat that may sometimes not be used by an Entity.
        /// </summary>
        /// <param name="neverUsed">If true, it means that the stat will never be used by an Entity, not just temporary.</param>
        public NonEntityUsage(bool neverUsed)
        {
            NeverUsed = neverUsed;
        }
    }
}
