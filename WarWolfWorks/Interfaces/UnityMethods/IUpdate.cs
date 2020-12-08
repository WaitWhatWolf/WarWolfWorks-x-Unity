using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes Update for a <see cref="MonoBehaviour"/>-like Update() method.
    /// </summary>
    public interface IUpdate
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.Update().
        /// </summary>
        void Update();
    }
}
