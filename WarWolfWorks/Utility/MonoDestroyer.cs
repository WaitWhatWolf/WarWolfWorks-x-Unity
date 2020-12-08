using System;
using UnityEngine;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// A monobehaviour class which has a public action called when it is destroyed.
    /// </summary>
    public sealed class MonoDestroyer : MonoBehaviour
    {
        /// <summary>
        /// Called in OnDestroy.
        /// </summary>
        public event Action OnDestroyed;

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }
    }
}
