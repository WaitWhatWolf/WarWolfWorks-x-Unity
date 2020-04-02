using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities
{
    /// <summary>
    /// Core class to be used with <see cref="Nyu"/> to simulate a component.
    /// </summary>
    public abstract class NyuComponent : MonoBehaviour, INyuComponent, INyuReferencable
    {
        /// <summary>
        /// The owner of this component.
        /// </summary>
        public Nyu NyuMain { get; internal set; }

        /// <summary>
        /// The position of the parent.
        /// </summary>
        public Vector3 Position => NyuMain.Position;

        /// <summary>
        /// The rotation of the parent.
        /// </summary>
        public Quaternion Rotation => NyuMain.Rotation;

        /// <summary>
        /// The euler rotation of the parent.
        /// </summary>
        public Vector3 Euler => NyuMain.Euler;
    }
}
