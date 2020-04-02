using System;
using System.Collections;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EntitiesSystem
{
    using Interfaces;
    using WarWolfWorks.Interfaces.UnityMethods;

    /// <summary>
    /// Base class for all Components used by an Entity.
    /// </summary>
    [RequireComponent(typeof(Entity))]
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public class EntityComponent : MonoBehaviour, IEntityComponent
    {
        private bool checkedEntity = false;
        private Entity entitymainundr;
        /// <summary>
        /// The Entity which uses this component.
        /// </summary>
        public Entity EntityMain
        {
            get
            {
                if(!checkedEntity)
                {
                    entitymainundr = GetComponent<Entity>();
                    if (entitymainundr == null)
                        entitymainundr = GetComponentInParent<Entity>();
                    checkedEntity = true;
                }

                return entitymainundr;
            }
        }
        /// <summary>
        /// Pointer to EntityMain.Position.
        /// </summary>
        public Vector3 Position
        {
            get => EntityMain.Position;
            set => EntityMain.Position = value;
        }
        /// <summary>
        /// Pointer to EntityMain.Rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => EntityMain.Rotation;
            set => EntityMain.Rotation = value;
        }
        /// <summary>
        /// Pointer to EntityMain.Euler.
        /// </summary>
        public Vector3 Euler
        {
            get => EntityMain.Euler;
            set => EntityMain.Euler = value;
        }

        private void Awake()
        {
            if (!EntityMain.Components.Contains(this)) EntityMain.InternalAddComponent(this);
        }



        public virtual void OnAwake() { }
        public virtual void OnEnabled() { }
        public virtual void OnDisabled() { }
        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixed() { }
        public virtual void OnDestroyed() { }

        void IUpdate.Update() => OnUpdate();
        void IFixedUpdate.FixedUpdate() => OnFixed();
        void IOnEnable.OnEnable() => OnEnabled();
        void IOnDisable.OnDisable() => OnDisabled();
        void IStart.Start() => OnStart();
        void IOnDestroy.OnDestroy() => OnDestroyed();
        void IAwake.Awake() => OnAwake();

        /// <summary>
        /// Pointer to <see cref="Hooks.StartCoroutine(MonoBehaviour, IEnumerator, ref bool)"/>.
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="verifier"></param>
        public void StartCoroutine(IEnumerator routine, ref bool verifier) => Hooks.StartCoroutine(EntityMain, routine, ref verifier);

        /// <summary>
        /// Pointer to <see cref="Hooks.StopCoroutine(MonoBehaviour, IEnumerator, ref bool)"/>.
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="verifier"></param>
        public void StopCoroutine(IEnumerator routine, ref bool verifier) => Hooks.StopCoroutine(EntityMain, routine, ref verifier);
    }
}