using System;
using UnityEngine;

namespace WarWolfWorks.NyuEntities.YharonSystem
{
    /// <summary>
    /// Class used with <see cref="NyuYharon"/> to indicate that it is resistant to a specific <see cref="Yharon"/>.
    /// </summary>
    internal struct TaCx1
    {
        /// <summary>
        /// How much it is resistant (0-1).
        /// </summary>
        public float Percent;
        /// <summary>
        /// What type of <see cref="Yharon"/> is it resistant to.
        /// </summary>
        public Type Resistance;

        /// <summary>
        /// Creates a new <see cref="TaCx1"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="percent01"></param>
        public TaCx1(Type type, float percent01)
        {
            Resistance = type;
            Percent = Mathf.Clamp(percent01, 0f, 1f);
        }
    }
}
