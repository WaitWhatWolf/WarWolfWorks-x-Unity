using System.Collections;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EntitiesSystem
{
    using Interfaces;
    /// <summary>
    /// Base class for all Components used by an Entity.
    /// </summary>
    [RequireComponent(typeof(Entity))]
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
            if (!EntityMain.Components.Contains(this)) EntityMain.AddEntityComponent(this);
        }

        public virtual void OnAwake() { }
        public virtual void OnEnabled() { }
        public virtual void OnDisabled() { }
        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixed() { }
        public virtual void OnDestroyed() { }

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