using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Utility;

namespace WarWolfWorks.NyuEntities
{
    /// <summary>
    /// Rotation component which takes care of the entity's rotation; Contains various utilities
    /// as well as general rotation functionnality.
    /// </summary>
    public sealed class NyuRotation : Rotation, INyuComponent, INyuUpdate
    {
        /// <summary>
        /// The position of the parent.
        /// </summary>p
        public Vector3 Position => NyuMain.Position;

        /// <summary>
        /// The rotation of the parent.
        /// </summary>
        public Quaternion Rotation => NyuMain.Rotation;

        /// <summary>
        /// The euler rotation of the parent.
        /// </summary>
        public Vector3 Euler => NyuMain.Euler;

        /// <summary>
        /// The <see cref="Nyu"/> parent of this component.
        /// </summary>
        public Nyu NyuMain { get; internal set; }

        internal override void Update()
        {

        }

        void INyuUpdate.NyuUpdate()
        {
            base.Update();
        }
    }
}
