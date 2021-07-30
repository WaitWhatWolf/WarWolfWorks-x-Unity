using WarWolfWorks.NyuEntities;
using WarWolfWorks.NyuEntities.Statistics;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Interface used for calculating stats inside an <see cref="Nyu"/>'s <see cref="Stats"/>.
    /// </summary>
    public interface INyuStacking : IParentable<Stats>
    {
        /// <summary>
        /// Final value that will be returned.
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        float CalculatedValue(INyuStat stat);
    }
}
