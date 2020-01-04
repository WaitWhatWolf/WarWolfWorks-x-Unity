using System;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Depicts a 3D axis for directions. (Flags-friendly)
    /// </summary>
    [Flags]
    public enum Axis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }
}
