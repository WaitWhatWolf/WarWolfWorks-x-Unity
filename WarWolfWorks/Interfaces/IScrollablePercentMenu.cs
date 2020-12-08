using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Used to make a scroll menu based on min-max position based on a percentage.
    /// </summary>
    public interface IScrollablePercentMenu
    {
        /// <summary>
        /// Goes towards this position the closer <see cref="Percent"/> is to 0.
        /// </summary>
        Vector3 MinPosition { get; set; }
        /// <summary>
        /// Goes towards this position the closer <see cref="Percent"/> is to 1.
        /// </summary>
        Vector3 MaxPosition { get; set; }
        /// <summary>
        /// <see cref="RectTransform"/> which will be moved based on <see cref="Percent"/>.
        /// </summary>
        RectTransform ScrollHolder { get; }
        /// <summary>
        /// Percentage at which this menu is scrolled. (percentage in 0-1)
        /// </summary>
        float Percent { get; set; }
    }
}
