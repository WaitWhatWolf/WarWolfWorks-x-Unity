using UnityEngine;

namespace WarWolfWorks.Attributes
{
    /// <summary>
    /// Displays an enum value inside the inspector as Flags.
    /// </summary>
    public class EnumFlagsAttribute : PropertyAttribute
    {
        /// <summary>
        /// Custom name of the enum value.
        /// </summary>
        public string enumName;

        /// <summary>
        /// Displays an enum value inside the inspector as Flags.
        /// </summary>
        public EnumFlagsAttribute() { }

        /// <summary>
        /// Displays an enum value inside the inspector as Flags with a custom name.
        /// </summary>
        /// <param name="name"></param>
        public EnumFlagsAttribute(string name)
        {
            enumName = name;
        }
    }
}
