using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Utility;

namespace WarWolfWorks.NyuEntities
{
    /// <summary>
    /// Rotation component which takes care of the entity's rotation; Contains various utilities
    /// as well as general rotation functionnality.
    /// </summary>
    public sealed class NyuRotation : MonoRotation, INyuComponent, INyuUpdate, IPosition, IRotation, IEulerAngles
    {
        /// <summary>
        /// The position of the parent.
        /// </summary>
        public Vector3 Position { get => NyuMain.Position; set => NyuMain.Position = value; }

        /// <summary>
        /// The rotation of the parent.
        /// </summary>
        public Quaternion Rotation { get => NyuMain.Rotation; set => NyuMain.Rotation = value; }

        /// <summary>
        /// The euler rotation of the parent.
        /// </summary>
        public Vector3 EulerAngles { get => NyuMain.EulerAngles; set => NyuMain.EulerAngles = value; }

        /// <summary>
        /// The <see cref="Nyu"/> parent of this component.
        /// </summary>
        public Nyu NyuMain { get; internal set; }

#pragma warning disable 1591
        protected override void Update()
        {

        }
#pragma warning restore 1591

        void INyuUpdate.NyuUpdate()
        {
            base.Update();
        }
    }
}
