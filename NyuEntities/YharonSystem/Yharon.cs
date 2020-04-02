using System;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.YharonSystem
{
    /// <summary>
    /// Core class used to apply Buffs, Debuffs or anything in between to a Nyu entity.
    /// (Supported interfaces: <see cref="INyuAwake"/>, <see cref="INyuUpdate"/>, <see cref="INyuFixedUpdate"/>, <see cref="INyuLateUpdate"/>,
    /// <see cref="INyuOnEnable"/>, <see cref="INyuOnDisable"/> and <see cref="INyuOnDestroy"/>)
    /// </summary>
    public abstract class Yharon : IParentable<NyuYharon>
    {
        /// <summary>
        /// Type of the <see cref="Yharon"/> used to compare with other <see cref="Yharon"/> components inside <see cref="NyuYharon"/>.
        /// </summary>
        public abstract Type YharonType { get; }

        /// <summary>
        /// How this <see cref="Yharon"/> is applied inside a <see cref="NyuYharon"/> when a <see cref="Yharon"/> of an existing type is added
        /// to <see cref="NyuYharon"/>.
        /// </summary>
        public abstract YharonApplication Application { get; }

        /// <summary>
        /// The <see cref="NyuYharon"/> containing this <see cref="Yharon"/>.
        /// </summary>
        public NyuYharon Parent { get; internal set; }

        /// <summary>
        /// Used by <see cref="NyuYharon"/> internally to call OnOverride.
        /// </summary>
        /// <param name="overrider"></param>
        internal void CallOnOverride(Yharon overrider) => OnOverride(overrider);

        /// <summary>
        /// Called when a <see cref="Yharon"/> of the same type as this one is added with it's <see cref="YharonApplication"/> containing the <see cref="YharonApplication.Override"/> flag.
        /// </summary>
        /// <param name="overrider"></param>
        protected abstract void OnOverride(Yharon overrider);
    }
}
