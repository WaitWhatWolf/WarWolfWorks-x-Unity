using System;

namespace WarWolfWorks.Enums
{
    /// <summary>
    /// Global enum to handle directions.
    /// </summary>
    [Flags]
    public enum Direction
    {
        /// <summary>
        /// The neutral direction, where X, Y and Z are 0.
        /// </summary>
        Neutral = 0,
        /// <summary>
        /// Y Axis +
        /// </summary>
        Up = 4,
        /// <summary>
        /// Y Axis -
        /// </summary>
        Down = Up << 1,
        /// <summary>
        /// X Axis -
        /// </summary>
        Left = Down << 1,
        /// <summary>
        /// X Axis +
        /// </summary>
        Right = Left << 1,
        /// <summary>
        /// Z Axis +
        /// </summary>
        Forward = Right << 1,
        /// <summary>
        /// Z Axis -
        /// </summary>
        Backward = Forward << 1,
    }
}
