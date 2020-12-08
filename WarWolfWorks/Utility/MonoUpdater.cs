using System;
using UnityEngine;

namespace WarWolfWorks.Utility
{
    internal sealed class MonoUpdater : MonoBehaviour
    {
        public event Action OnUpdate;
        private void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}
