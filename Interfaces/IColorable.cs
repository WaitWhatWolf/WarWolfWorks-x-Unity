#if WWWCLM_2
using WarWolfWorks.Utility.Coloring;
using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Interface used by <see cref="ColorManager"/> to manage colors.
    /// </summary>
    public interface IColorable
    {
        /// <summary>
        /// Color which will be applied by a <see cref="ColorManager"/>.
        /// </summary>
        Color ColorApplier { set; }
    }
}
#endif