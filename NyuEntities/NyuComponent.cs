using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities
{
    /// <summary>
    /// Core class to be used with <see cref="Nyu"/> to simulate a component.
    /// Supported interfaces: <see cref="INyuAwake"/>, <see cref="INyuUpdate"/>, <see cref="INyuFixedUpdate"/>, <see cref="INyuLateUpdate"/>,
    /// <see cref="INyuOnDestroyQueued"/>, <see cref="INyuOnDestroy"/>, 
    /// <see cref="INyuOnEnable"/>, <see cref="INyuOnDisable"/>,
    /// <see cref="INyuOnTriggerEnter"/>, <see cref="INyuOnTriggerEnter2D"/>,
    /// <see cref="INyuOnTriggerExit"/>, <see cref="INyuOnTriggerExit2D"/>,
    /// <see cref="INyuOnCollisionEnter"/>, <see cref="INyuOnCollisionEnter2D"/>,
    /// <see cref="INyuOnCollisionExit"/> and <see cref="INyuOnCollisionExit2D"/>.
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
