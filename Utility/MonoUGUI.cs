using System;
using UnityEngine;

namespace WarWolfWorks.Utility
{
    internal sealed class MonoUGUI : MonoBehaviour
    {
        public event Action OnGUIevent;
        private void OnGUI()
        {
            OnGUIevent?.Invoke();
        }
    }
}
