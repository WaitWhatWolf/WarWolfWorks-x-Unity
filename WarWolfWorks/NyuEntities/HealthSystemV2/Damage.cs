using System;
using System.Collections.Generic;
using System.Text;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.HealthSystemV2
{
    /// <summary>
    /// Record used by <see cref="NyuHealth"/> to damage health.
    /// </summary>
    /// <remarks>This should not be used in a production-ready environment as use of this
    /// requires implementing a custom C#9 compiler in your project which might be unstable;
    /// If you're working in a production-ready environment, use <see cref="WarWolfWorks.NyuEntities.HealthSystem"/> instead
    /// or go to <see href="https://github.com/WaitWhatWolf/WarWolfWorks-x-Unity/tree/master/WarWolfWorks/NyuEntities">
    /// the WarWolfWorks-x-Unity library</see>
    /// and copy-paste the HealthSystemV2 folder in your own project, then make the Damage record into
    /// a class.</remarks>
    public abstract record Damage : INyuReferencable 
    {
        /// <summary>
        /// The entity which created this damage.
        /// </summary>
        public Nyu NyuMain { get; }

        /// <summary>
        /// Applies this value as "damage" to <see cref="NyuHealth.CurrentHealth"/>.
        /// </summary>
        /// <returns></returns>
        public abstract float Final(NyuHealth to);
    
        /// <summary>
        /// Creates a base damage record.
        /// </summary>
        /// <param name="nyuMain">The entity responsible for "dealing" this damage; Can be null.</param>
        public Damage(Nyu nyuMain)
        {
            NyuMain = nyuMain;
        }
    }
}
