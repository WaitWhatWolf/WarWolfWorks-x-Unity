using static WarWolfWorks.WWWResources;
using WarWolfWorks.NyuEntities.Statistics;

namespace WarWolfWorks.Enums
{
    /// <summary>
    /// Stacking applied by <see cref="DefaultStacking"/> represented in an enum. For a detailed description of all
    /// stacking interactions, see <see cref="DefaultStacking"/>'s constant values located in <see cref="WWWResources"/>.
    /// </summary>
    public enum EDefaultNyuStacking
    {
#pragma warning disable 1591
        Base = NYU_STATS_STACKING_BASE,
        Overrider = NYU_STATS_STACKING_OVERRIDER,
        Additive = NYU_STATS_STACKING_ADDITIVE,
        BaseMult = NYU_STATS_STACKING_BASEMULT,
        TotalMult = NYU_STATS_STACKING_TOTALMULT,
        Pwner = NYU_STATS_STACKING_PWNER,
#pragma warning restore 1591
    }
}
