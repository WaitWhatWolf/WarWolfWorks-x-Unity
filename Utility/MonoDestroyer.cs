using System;
using UnityEngine;

namespace WarWolfWorks.Utility
{
    public sealed class MonoDestroyer : MonoBehaviour
    {
        public event Action OnDestroyed;

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }
    }
}
