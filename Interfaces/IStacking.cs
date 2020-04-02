using WarWolfWorks.EntitiesSystem.Statistics;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Interface used for calculating stats inside an <see cref="WarWolfWorks.EntitiesSystem.Entity"/>'s <see cref="Stats"/>.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public interface IStacking
    {
        /// <summary>
        /// Final value that will be returned.
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        float CalculatedValue(IStat stat);

        /// <summary>
        /// Who's stats should this stacking calculate.
        /// </summary>
        Stats Parent { get; set; }
    }
}
