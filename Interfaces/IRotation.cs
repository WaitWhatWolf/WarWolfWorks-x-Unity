using WarWolfWorks.Utility;
using UnityEngine;

namespace WarWolfWorks.Interfaces
{
	public interface IRotation
	{
        Transform ToRotateX { get; }
        Transform ToRotateY { get; }
        Transform ToRotateZ { get; }
        /// <summary>
        /// Rotation towards which this <see cref="IRotation"/> is destinated.
        /// </summary>
        /// <returns></returns>
        Quaternion GetDestination();
        /// <summary>
        /// The default rotation of the <see cref="IRotation"/>.
        /// </summary>
        Vector3 DefaultEulerRotation { get; }
        /// <summary>
        /// Where this <see cref="IRotation"/> currently is.
        /// </summary>
        Quaternion CurrentRotation { get; }
        /// <summary>
        /// Speed at which this <see cref="IRotation"/> is rotating.
        /// </summary>
        float RotationSpeed { get; }
        /// <summary>
        /// Sets the destination of this <see cref="IRotation"/>.
        /// </summary>
        /// <param name="toApply"></param>
        void SetRotation(Vector3 toApply);
        /// <summary>
        /// Sets the destination of this <see cref="IRotation"/> in euler angles.
        /// </summary>
        /// <param name="toApply"></param>
        void SetRotation(Quaternion toApply);
    }
}