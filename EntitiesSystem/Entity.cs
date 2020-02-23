using System;
using UnityEngine;

namespace WarWolfWorks.EntitiesSystem
{
    using Statistics;
    using System.Collections;
    using System.Collections.Generic;
    using WarWolfWorks.Debugging;
    using WarWolfWorks.Interfaces;
    using WarWolfWorks.Security;
    using WarWolfWorks.Utility;

    /// <summary>
    /// The core class of everything inside <see cref="WarWolfWorks.EntitiesSystem"/>
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        /// <summary>
        /// Override this to set the entity's base type. (E.G Enemy, Boss, Player, etc...)
        /// </summary>
        public abstract Type EntityType { get; }
#pragma warning disable 0649
        private bool ComponentsDrawn = false;
       private List<IEntityComponent> コンポーネント = new List<IEntityComponent>();
        internal List<IEntityComponent> Components
        {
            get
            {
                if (!ComponentsDrawn)
                {
                    if(コンポーネント.Count > 0)
                    {
                        ComponentsDrawn = true;
                        return コンポーネント;
                    }
                    RefreshComponents();
                    ComponentsDrawn = true;
                }return コンポーネント;
            }
        }
#pragma warning restore 0649

        [SerializeField][Rename("Entity Name")]
        private string 名前;
        /// <summary>
        /// The name of the entity. (To get the component name, use lowercase name instead).
        /// </summary>
        public virtual string Name
        {
            get => 名前.Equals(string.Empty) ? gameObject.name : 名前;
            set => 名前 = value;
        }

        /// <summary>
        /// Returns true if this entity is of the same type as the value given; Compares <see cref="EntityType"/>, not the class' actual type.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public bool IsEntity(Type of)
            => of == EntityType;

        /// <summary>
        /// Returns true if this entity's underlying type is the same as the value given; Compares <see cref="EntityType"/>, not the class' actual type.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public bool IsUnderlyingEntity(Type of)
            => EntityType.Equals(of);

        /// <summary>
        /// Pointer to <see cref="Entity"/>.transform.position.
        /// </summary>
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        /// <summary>
        /// Pointer to <see cref="Entity"/>.transform.rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        /// <summary>
        /// Pointer to <see cref="Entity"/>.transform.eulerAngles.
        /// </summary>
        public Vector3 Euler
        {
            get => transform.eulerAngles;
            set => transform.eulerAngles = value;
        }

        /// <summary>
        /// Call type used for Unity Monobehaviour called by Entity-related scripts. (see <see cref="EntityComponent"/>)
        /// </summary>
        public enum CallType
        {
            /// <summary>
            /// Used for <see cref="MonoBehaviour"/>.Awake().
            /// </summary>
            awake,
            /// <summary>
            /// Used for <see cref="MonoBehaviour"/>.Start().
            /// </summary>
            start,
            /// <summary>
            /// Used for <see cref="MonoBehaviour"/>.OnEnable().
            /// </summary>
            enable,
            /// <summary>
            /// Used for <see cref="MonoBehaviour"/>.OnDisable().
            /// </summary>
            disable,
            /// <summary>
            /// Used for <see cref="MonoBehaviour"/>.Update().
            /// </summary>
            update,
            /// <summary>
            /// Used for <see cref="MonoBehaviour"/>.FixedUpdate().
            /// </summary>
            fixedU,
            /// <summary>
            /// Used for <see cref="MonoBehaviour"/>.OnDestroy().
            /// </summary>
            destroy
        }

        private bool StatsCreated;
        private Stats stats;
        /// <summary>
        /// All stats contained by the entity.
        /// </summary>
        public Stats Stats
        {
            get
            {
                if (!StatsCreated)
                {
                    stats = new Stats();
                    StatsCreated = true;
                }
                return stats;
            }
        }

        /// <summary>
        /// Triggers when a Unity method is called on the Entity. 
        /// Only triggers on: <see cref="CallType.destroy"/>, <see cref="CallType.enable"/>, <see cref="CallType.disable"/>, <see cref="CallType.awake"/>.
        /// </summary>
        public event Action<Entity, CallType> OnCallEventTrigger;

        /// <summary>
        /// Calls a Monobehaviour method from all <see cref="EntityComponent"/>s attached to this entity.
        /// </summary>
        /// <param name="callType"></param>
        internal void CallComponentMethods(CallType callType)
        {
            for(int i = 0; i < Components.Count; i++)
            {
                switch (callType)
                {
                    case CallType.awake: Components[i].OnAwake(); break;
                    case CallType.start: Components[i].OnStart(); break;
                    case CallType.enable: Components[i].OnEnabled(); break;
                    case CallType.update: Components[i].OnUpdate(); break;
                    case CallType.fixedU: Components[i].OnFixed(); break;
                    case CallType.disable: Components[i].OnDisabled(); break;
                    case CallType.destroy: Components[i].OnDestroyed(); break;
                }
            }
        }

        #region Components
        internal void RefreshComponents()
        {
            コンポーネント = new List<IEntityComponent>(GetComponents<IEntityComponent>());
        }
        /// <summary>
        /// A more performant way of getting an <see cref="IEntityComponent"/> as opposed to <see cref="Component.GetComponent{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetEntityComponent<T>()
        {
            return (T)Components.Find(ec => ec is T);
        }
        /// <summary>
        /// Equivalent to <see cref="GetEntityComponent{T}"/> with a shorter name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GEC<T>()
        {
            return (T)Components.Find(ec => ec is T);
        }

        /// <summary>
        /// Returns all entity components which are of given generic type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetEntityComponents<T>()
        {
            return (IEnumerable<T>)Components.FindAll(ec => ec is T);
        }

        /// <summary>
        /// Equivalent to <see cref="GetEntityComponent{T}"/> in an try-out fashion.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGetEntityComponent<T>(out T component)
        {
            component = (T)Components.Find(ec => ec is T);
            if (component != null)
                return true;
            else return false;
        }

        /// <summary>
        /// Same method as <see cref="TryGetEntityComponent{T}(out T)"/> with a shorter name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TGEC<T>(out T component)
        {
            component = (T)Components.Find(ec => ec is T);
            if (component != null)
                return true;
            else return false;
        }

        /// <summary>
        /// Returns all <see cref="IEntityComponent"/> components attached to this entity.
        /// </summary>
        /// <returns></returns>
        public IEntityComponent[] GetAllEntityComponents() => Components.ToArray();
        #endregion

        private AnimationManager animManag;
        private bool animManChecked = false;
        /// <summary>
        /// Entity's Animation Manager. (Always attached by default)
        /// </summary>
        public AnimationManager AnimationManager
        {
            get
            {
                if (!animManChecked)
                {
                    animManag = GetComponent<AnimationManager>();
                    animManChecked = true;
                }

                return animManag;
            }
        }

        internal void InternalAddComponent(IEntityComponent component)
        {
            Components.Add(component);
        }

        /// <summary>
        /// Adds an <see cref="EntityComponent"/> to this <see cref="Entity"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddEntityComponent<T>() where T : Component, IEntityComponent
        {
            T used = gameObject.AddComponent<T>();
            InternalAddComponent(used);

            return used;
        }

        /// <summary>
        /// Adds an <see cref="EntityComponent"/> to this <see cref="Entity"/>.
        /// </summary>
        /// <returns></returns>
        public IEntityComponent AddEntityComponent(Type type)
        {
            if (!Hooks.Implements(type, typeof(IEntityComponent)))
                throw new EntityException(EntityExceptionType.ENTITY_COMPONENT_TYPE_INVALID);

            IEntityComponent used = (IEntityComponent)gameObject.AddComponent(type);
            InternalAddComponent(used);

            return used;
        }

        /// <summary>
        /// Removes an <see cref="EntityComponent"/> from this <see cref="Entity"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool RemoveEntityComponent<T>() where T : Component, IEntityComponent
        {
            T toUse = (T)Components.Find(c => c is T);

            if (!toUse)
                return false;

            Components.Remove(toUse);
            toUse.OnDestroyed();
            Destroy(toUse);
            return true;
        }

        /// <summary>
        /// Removes an <see cref="EntityComponent"/> from this <see cref="Entity"/>.
        /// </summary>
        /// <returns></returns>
        public bool RemoveEntityComponent(Type type)
        {
            IEntityComponent toUse = Components.Find(c => c.GetType() == type);

            if (toUse == null)
                return false;

            Components.Remove(toUse);
            Destroy(gameObject.GetComponent(type));
            return true;
        }

#pragma warning disable IDE0051

        internal bool InitiatedViaManager = false;

        private void Awake()
        {
            if (!InitiatedViaManager)
            {
                AdvancedDebug.LogWarning(EntityException.MESSAGE_WARNING_NOT_INITIATED_PROPERLY, AdvancedDebug.DEBUG_LAYER_WWW_INDEX);
                Stats.Initiate(this);
                EntityManager.InitiatedEntities.Add(this);
                EntityManager.OnEntityInstantiatedCall(this);
            }

            CallComponentMethods(CallType.awake);
            OnAwake();
            OnCallEventTrigger?.Invoke(this, CallType.awake);
        }

        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.Awake().
        /// </summary>
        protected virtual void OnAwake() { }

        private void Start()
        {
            CallComponentMethods(CallType.start);
            OnStart();
        }

        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.Start().
        /// </summary>
        protected virtual void OnStart() { }


        private void OnEnable()
        {
            CallComponentMethods(CallType.enable);
            OnEnabled();
            OnCallEventTrigger?.Invoke(this, CallType.enable);
        }

        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnEnable().
        /// </summary>
        protected virtual void OnEnabled() { }
        internal void InternalUpdate()
        {
            CallComponentMethods(CallType.update);
            OnUpdate();
        }

        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.Update().
        /// </summary>
        protected virtual void OnUpdate() { }
        internal void InternalFixedUpdate()
        {
            CallComponentMethods(CallType.fixedU);
            OnFixed();
        }

        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.FixedUpdate().
        /// </summary>
        protected virtual void OnFixed() { }
        private void OnDisable()
        {
            CallComponentMethods(CallType.disable);
            OnDisabled();
            OnCallEventTrigger?.Invoke(this, CallType.disable);
        }
        #pragma warning restore IDE0051
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnDisable().
        /// </summary>
        protected virtual void OnDisabled() { }

        /// <summary>
        /// Called after the entity is destroyed, but not cleared.
        /// </summary>
        protected virtual void OnDestroyed() { }
        /// <summary>
        /// Called right before the entity is destroyed.
        /// </summary>
        protected virtual void OnBeforeDestroy() { }

        internal bool CallsEventDestroy = true;

        /// <summary>
        /// Destroys this entity officially, removing it from all Entity lists inside the WWW library and calls it's <see cref="OnDestroyed"/> method.
        /// </summary>
        public void Destroy()
        {
            OnBeforeDestroy();
            EntityManager.InitiatedEntities.Remove(this);
            CallComponentMethods(CallType.destroy);
            OnCallEventTrigger?.Invoke(this, CallType.destroy);
            OnDestroyed();
            if (CallsEventDestroy)
                EntityManager.OnEntityDestroyedCall(this, true);
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Destroys the entity without invoking it's destroy method(s) or it's component's destroy method(s).
        /// </summary>
        public void DestroyUnofficially()
        {
            EntityManager.InitiatedEntities.Remove(this);
            EntityManager.OnEntityDestroyedCall(this, false);
            Destroy(gameObject);
        }

        /// <summary>
        /// Removes a component from the list of <see cref="IEntityComponent"/>s.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool RemoveComponent<T>()
        {
            IEntityComponent toRemove = Components.Find(c => c is T);
            
            if(Components.Remove(toRemove))
            {
                toRemove.OnDestroyed();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a specific element, countrary to <see cref="RemoveComponent{T}"/> which removes the first element of type given.
        /// </summary>
        /// <param name="toRemove"></param>
        /// <returns></returns>
        public bool RemoveComponent(IEntityComponent toRemove)
        {
            IEntityComponent removed = Components.Find(c => c == toRemove);
            if (Components.Remove(removed))
            {
                removed.OnDestroyed();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the entity's .transform.
        /// </summary>
        /// <param name="e"></param>
        public static implicit operator Transform(Entity e)
            => e.transform;

    }
}