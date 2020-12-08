using WarWolfWorks.NyuEntities;
using WarWolfWorks.NyuEntities.Statistics;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Interface used for calculating stats inside an <see cref="Nyu"/>'s <see cref="Stats"/>.
    /// </summary>
    public interface INyuStacking
    {
        /// <summary>
        /// Final value that will be returned.
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        float CalculatedValue(INyuStat stat);

        /// <summary>
        /// Who's stats should this stacking calculate.
        /// </summary>
        Stats Parent { get; }

        /// <summary>
        /// Sets the <see cref="Parent"/>.
        /// </summary>
        /// <param name="to"></param>
        void SetParent(Stats to);
    }
}
