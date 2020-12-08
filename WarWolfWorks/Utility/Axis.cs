using System;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Depicts a 3D axis for directions. (Flags-friendly)
    /// </summary>
    [Flags]
    public enum Axis
    {
        /// <summary>
        /// No Axis.
        /// </summary>
        None = 0,
        /// <summary>
        /// Horizontal Axis.
        /// </summary>
        X = 2,
        /// <summary>
        /// Vertical Axis.
        /// </summary>
        Y = 4,
        /// <summary>
        /// Depth Axis.
        /// </summary>
        Z = 8
    }
}
