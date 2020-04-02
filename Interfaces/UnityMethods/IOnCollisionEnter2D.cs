using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes OnCollisionEnter2D for a <see cref="MonoBehaviour"/>-like OnCollisionEnter2D(Collision2D) method.
    /// </summary>
    public interface IOnCollisionEnter2D
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnCollisionEnter2D(Collision2D).
        /// </summary>
        /// <param name="collision"></param>
        void OnCollisionEnter2D(Collision2D collision);
    }
}
