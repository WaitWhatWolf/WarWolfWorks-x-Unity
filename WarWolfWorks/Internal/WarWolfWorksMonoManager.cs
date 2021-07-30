using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WarWolfWorks.Internal
{
    internal class WarWolfWorksMonoManager : MonoBehaviour
    {
        private void Start() => WWWResources.MonoManager_OnStart?.Invoke();

        private void Update() => WWWResources.MonoManager_OnUpdate?.Invoke();
        private void FixedUpdate() => WWWResources.MonoManager_OnFixedUpdate?.Invoke();
        private void LateUpdate() => WWWResources.MonoManager_OnLateUpdate?.Invoke();

        private void OnDestroy() => WWWResources.MonoManager_OnDestroyed?.Invoke();
    }
}
