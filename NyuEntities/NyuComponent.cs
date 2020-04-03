using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities
{
    /// <summary>
    /// Core class to be used with <see cref="Nyu"/> to simulate a component.
    /// </summary>
    public abstract class NyuComponent : MonoBehaviour, INyuComponent, INyuReferencable, IPosition, IRotation, IEulerAngles
    {
        /// <summary>
        /// The owner of this component.
        /// </summary>
        public Nyu NyuMain { get; internal set; }

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
    }
}
