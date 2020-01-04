using System;
using UnityEngine;

namespace WarWolfWorks.Maps
{
    

    [Serializable]
    public struct MapComponent
    {
        [SerializeField]
        private GameObject toLoad;

        [SerializeField]
        private bool destroyOnUnload;

        [SerializeField]
        private bool disablable;

        [SerializeField]
        private bool usesCustomPosition;

        [SerializeField]
        private bool usesCustomRotation;

        [SerializeField]
        private bool fixName;

        [SerializeField]
        private Vector3 customPosition;

        [SerializeField]
        private Vector3 customRotation;

        public GameObject ToLoad => toLoad;

        public bool DestroyOnUnload => destroyOnUnload;

        public bool Disablable => disablable;

        public bool UsesCustomPosition => usesCustomPosition;

        public bool UsesCustomRotation => usesCustomRotation;

        public bool FixName => fixName;

        public Vector3 CustomPosition => customPosition;

        public Vector3 CustomRotation => customRotation;
    }

}
