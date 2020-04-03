using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Indicates an object which has euler angles.
    /// </summary>
    interface IEulerAngles
    {
        /// <summary>
        /// The euler angles.
        /// </summary>
        Vector3 EulerAngles { get; set; }
    }
}
