using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Indicates an object which has a rotation.
    /// </summary>
    public interface IRotation
    {
        /// <summary>
        /// The rotation.
        /// </summary>
        Quaternion Rotation { get; set; }
    }
}
