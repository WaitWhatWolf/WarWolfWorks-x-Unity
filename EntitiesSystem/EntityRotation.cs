using System.Collections;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.UnityMethods;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EntitiesSystem
{
    /// <summary>
    /// Basic script for controling an entity's rotation.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public sealed class EntityRotation : Rotation, IEntityComponent
    {
        /// <summary>
        /// Entity using this <see cref="EntityRotation"/> component.
        /// </summary>
        public Entity EntityMain
        {
            get; private set;
        }

        void IAwake.Awake() { EntityMain = GetComponentInParent<Entity>(); }
        void IOnDestroy.OnDestroy() { }

        void IOnDisable.OnDisable() { }

        void IOnEnable.OnEnable() { }

        void IFixedUpdate.FixedUpdate() { }

        void IStart.Start() { }

        void IUpdate.Update()
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