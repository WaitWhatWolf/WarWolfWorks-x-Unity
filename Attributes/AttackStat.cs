using System;
using WarWolfWorks.EntitiesSystem.Attacking;

namespace WarWolfWorks.Attributes
{
    /// <summary>
    /// Attribute which is used with <see cref="Attack"/> to flag fields which are used as stats such as range, recoil, etc...
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class AttackStat : Attribute
    {
#pragma warning disable 0649
#pragma warning disable IDE0044
        /// <summary>
        /// The name to be displayed for the flagged stat.
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Creates an <see cref="AttackStat"/>.
        /// </summary>
        /// <param name="displayName"></param>
        public AttackStat(string displayName)
        {
            DisplayName = displayName;
        }

    }
}
