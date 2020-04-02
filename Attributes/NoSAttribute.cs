using System;
using UnityEngine;

namespace WarWolfWorks.Attributes
{
    /// <summary>
    /// Removes "s_" from a serialized field inside the inspector; Useful for naming variables.
    /// </summary>
    public sealed class NoSAttribute : PropertyAttribute
    {
        /// <summary>
        /// In case some field get cropped out.
        /// </summary>
        public float Padding;

        /// <summary>
        /// Creates a new <see cref="NoSAttribute"/>.
        /// </summary>
        public NoSAttribute()
        {
            Padding = 0;
        }

        /// <summary>
        /// Creates a <see cref="NoSAttribute"/> with extra space at the end; This is here in case a field gets cropped off.
        /// </summary>
        /// <param name="padding"></param>
        public NoSAttribute(float padding)
        {
            Padding = padding;
        }
    }
}
