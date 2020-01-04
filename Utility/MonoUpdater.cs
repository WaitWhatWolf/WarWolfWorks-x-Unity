using System;
using UnityEngine;

namespace WarWolfWorks.Utility
{
    public sealed class MonoUpdater : MonoBehaviour
    {
        public event Action OnUpdate;
        private void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}
