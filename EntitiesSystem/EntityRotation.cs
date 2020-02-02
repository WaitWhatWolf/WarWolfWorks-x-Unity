using System.Collections;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EntitiesSystem
{
    /// <summary>
    /// Basic script for controling an entity's rotation.
    /// </summary>
    public sealed class EntityRotation : Rotation, IEntityComponent
    {
        /// <summary>
        /// Entity using this <see cref="EntityRotation"/> component.
        /// </summary>
        public Entity EntityMain
        {
            get; private set;
        }

        void IAwake.OnAwake() { EntityMain = GetComponentInParent<Entity>(); }
        void IDestroy.OnDestroyed() { }

        void IEnableDisable.OnDisabled() { }

        void IEnableDisable.OnEnabled() { }

        void IFixedUpdate.OnFixed() { }

        void IStart.OnStart() { }

        void IUpdate.OnUpdate()
        {
            base.Update();
        }

        /// <summary>
        /// Overrided <see cref="Rotation.Update"/> method to do nothing; 
        /// This is to adapt this <see cref="EntityRotation"/> to an <see cref="Entity"/>'s Execution order.
        /// </summary>
        internal sealed override void Update()
        {
            
        }

        void ICoroutinable.StartCoroutine(IEnumerator routine, ref bool verifier) { this.StartCoroutine(routine, ref verifier); }

        void ICoroutinable.StopCoroutine(IEnumerator routine, ref bool verifier) { this.StopCoroutine(routine, ref verifier); }
    }
}