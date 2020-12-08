using UnityEngine;
using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Core interface for implementing a Nyu component outside of <see cref="NyuComponent"/>.
    /// </summary>
    public interface INyuComponent : INyuReferencable
    {
        /// <summary>
        /// The position of the parent Nyu.
        /// </summary>
        Vector3 Position { get; }
        /// <summary>
        /// The rotation of the parent Nyu.
        /// </summary>
        Quaternion Rotation { get; }
        /// <summary>
        /// The Euler rotation of the parent Nyu.
        /// </summary>
        Vector3 EulerAngles { get; }
    }
}
