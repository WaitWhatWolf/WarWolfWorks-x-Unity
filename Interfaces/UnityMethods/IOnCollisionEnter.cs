using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes IOnCollisionEnter for a <see cref="MonoBehaviour"/>-like OnCollisionEnter(Collision) method.
    /// </summary>
    public interface IOnCollisionEnter
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnCollisionEnter(Collision).
        /// </summary>
        /// <param name="collision"></param>
        void OnCollisionEnter(Collision collision);
    }
}
