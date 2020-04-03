using System;
using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.NyuEntities.Statistics;
using WarWolfWorks.Security;

namespace WarWolfWorks.NyuEntities
{
    /// <summary>
    /// Core class of the <see cref="NyuEntities"/> system.
    /// All usable MonoBehaviour methods have an alternative in "INyu" interfaces,
    /// located inside the <see cref="WarWolfWorks.Interfaces.NyuEntities"/> namespace.
    /// Supported interfaces: <see cref="INyuAwake"/>, <see cref="INyuUpdate"/>, <see cref="INyuFixedUpdate"/>, <see cref="INyuLateUpdate"/>,
    /// <see cref="INyuOnDestroyQueued"/>, <see cref="INyuOnDestroy"/>, 
    /// <see cref="INyuOnTriggerEnter"/>, <see cref="INyuOnTriggerEnter2D"/>,
    /// <see cref="INyuOnTriggerExit"/>, <see cref="INyuOnTriggerExit2D"/>,
    /// <see cref="INyuOnCollisionEnter"/>, <see cref="INyuOnCollisionEnter2D"/>,
    /// <see cref="INyuOnCollisionExit"/> and <see cref="INyuOnCollisionExit2D"/>.
    /// </summary>
    public abstract class Nyu : MonoBehaviour, IPosition, IRotation, IEulerAngles
    {
        /// <summary>
        /// The entity's stats manager.
        /// </summary>
        public Stats Stats { get; internal set; }

        [SerializeField]
        private string s_Tag;

        private string ns_Tag;
        private bool nsi_Tag;
        /// <summary>
        /// The additional tag of the entity; Used for additional indentification. 
        /// (Does not change the value of the serialized tag, only the return value; To change the serialized value, use Reflection on "s_Tag" field.)
        /// </summary>
        public string Tag
        {
            get
            {
                if(!nsi_Tag)
                {
                    ns_Tag = s_Tag;
                    nsi_Tag = true;
                }

                return ns_Tag;
            }
            set
            {
                ns_Tag = value;
            }
        }

        #region Pointers
        /// <summary>
        /// Pointer to transform.position.
        /// </summary>
        public Vector3 Position { get => transform.position; set => transform.position = value; }
        /// <summary>
        /// Pointer to transform.rotation.
        /// </summary>
        public Quaternion Rotation { get => transform.rotation; set => transform.rotation = value; }
        /// <summary>
        /// Pointer to transform.eulerAngles.
        /// </summary>
        public Vector3 EulerAngles { get => transform.eulerAngles; set => transform.eulerAngles = value; }
        #endregion

        #region Enabling and Disabling
        /// <summary>
        /// Invoked when the entity is disabled.
        /// </summary>
        public event Action OnEnabled;
        /// <summary>
        /// Invoked when the entity is enabled.
        /// </summary>
        public event Action OnDisabled;
#pragma warning disable
        private void OnEnable()
        {
            OnEnabled?.Invoke();
            for(int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i] is INyuOnEnable nyuEnable)
                    nyuEnable.NyuOnEnable();
            }
        }

        private void OnDisable()
        {
            OnDisabled?.Invoke();
            for (int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i] is INyuOnDisable nyuEnable)
                    nyuEnable.NyuOnDisable();
            }
        }
#pragma warning restore
        #endregion

        #region Instantiating
        private bool Initiated;

        private void Start()
        {
            if (!Initiated)
            {
                AdvancedDebug.LogWarning("When instantiating an Entity, make sure to do so with EntityManager.New" +
                    " as doing otherwise can cause an inconsistent execution-order.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);
                Stats = new Stats(this);
                hs_Components = new List<INyuComponent>(GetComponents<INyuComponent>());
                NyuManager.AllEntities.Add(this);
                CallInit();
                NyuManager.CallEntityBegin(this);
            }

            if (this is INyuStart thisStart)
                thisStart.NyuStart();

            for (int i = 0; i < hs_Components.Count; i++)
                if (hs_Components[i] is INyuStart nyuStart)
                    nyuStart.NyuStart();
        }

        internal void CallInit()
        {
            if (Initiated)
                return;

            Application.quitting += Event_RemoveDestroyCheck;

            NyuManager.Exception3Check(this);

            if (this is INyuAwake thisInit)
                thisInit.NyuAwake();

            for (int i = 0; i < hs_Components.Count; i++)
            {
                if(hs_Components[i] is NyuComponent component)
                    component.NyuMain = this;

                if (hs_Components[i] is INyuAwake init)
                {
                    init.NyuAwake();
                }
            }

            Initiated = true;
        }

        private void Event_RemoveDestroyCheck()
        {
            ns_DestroyedCorrectly = true;
        }
        #endregion

        #region Components
        [SerializeField, HideInInspector]
        internal List<INyuComponent> hs_Components = new List<INyuComponent>();

        [SerializeField, HideInInspector]
        internal bool[] hs_SerializePlotArmor = new bool[256];

        /// <summary>
        /// Adds a new entity component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddNyuComponent<T>() where T : Component, INyuComponent
        {
            if (hs_Components.FindIndex(c => c is T) != -1)
                throw new NyuEntityException(1);

            T toReturn = gameObject.AddComponent<T>();
            if(toReturn is NyuComponent component)
                component.NyuMain = this;
            else
            {
                toReturn.GetType().GetProperty("NyuMain").SetValue(toReturn, this);
            }
            hs_Components.Add(toReturn);
            if (toReturn is INyuAwake init)
            {
                init.NyuAwake();
            }

            return toReturn;
        }

        /// <summary>
        /// Equivalent to <see cref="AddNyuComponent{T}"/> with a shorter name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ANC<T>() where T : NyuComponent
        {
            if (hs_Components.FindIndex(c => c is T) != -1)
                throw new NyuEntityException(1);

            T toReturn = gameObject.AddComponent<T>();

            if (toReturn is NyuComponent component)
                component.NyuMain = this;
            else
                toReturn.GetType().GetProperty("NyuMain").SetValue(toReturn, this);

            hs_Components.Add(toReturn);
            if (toReturn is INyuAwake init)
            {
                init.NyuAwake();
            }

            return toReturn;
        }

        /// <summary>
        /// Adds a new entity component.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public INyuComponent AddNyuComponent(Type type)
        {
            if (hs_Components.FindIndex(c => c.GetType() == type) != -1)
                throw new NyuEntityException(1);

            if (!type.IsAssignableFrom(typeof(INyuComponent)))
                throw new NyuEntityException(5);

            INyuComponent toReturn = gameObject.AddComponent(type) as INyuComponent;

            if(toReturn is NyuComponent component)
                component.NyuMain = this;
            else
                toReturn.GetType().GetProperty("NyuMain").SetValue(toReturn, this);

            hs_Components.Add(toReturn);
            if (toReturn is INyuAwake init)
            {
                init.NyuAwake();
            }

            return toReturn;
        }

        /// <summary>
        /// Equivalent to <see cref="AddNyuComponent(Type)"/> with a shorter name.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public INyuComponent ANC(Type type)
        {
            if (hs_Components.FindIndex(c => c.GetType() == type) != -1)
                throw new NyuEntityException(1);

            if (!type.IsAssignableFrom(typeof(INyuComponent)))
                throw new NyuEntityException(5);

            INyuComponent toReturn = gameObject.AddComponent(type) as INyuComponent;

            if (toReturn is NyuComponent component)
                component.NyuMain = this;
            else
                toReturn.GetType().GetProperty("NyuMain").SetValue(toReturn, this);

            hs_Components.Add(toReturn);
            if (toReturn is INyuAwake init)
            {
                init.NyuAwake();
            }

            return toReturn;
        }

        /// <summary>
        /// Attempts to remove an entity component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool RemoveNyuComponent<T>()
        {
            for(int i = 0; i < hs_Components.Count; i++)
            {
                if(hs_Components[i] is T)
                {
                    if (hs_Components[i] is INyuOnDestroyQueued nyuQueue)
                        nyuQueue.NyuOnDestroyQueued();

                    if (hs_Components[i] is INyuOnDestroy nyuDestroy)
                        nyuDestroy.NyuOnDestroy();

                    if (hs_Components.Remove(hs_Components[i]))
                    {
                        Destroy(hs_Components[i] as Component);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Equivalent to <see cref="RemoveNyuComponent{T}"/> with a shorter name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool RNC<T>()
        {
            for (int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i] is T)
                {
                    if (hs_Components[i] is INyuOnDestroyQueued nyuQueue)
                        nyuQueue.NyuOnDestroyQueued();

                    if (hs_Components[i] is INyuOnDestroy nyuDestroy)
                        nyuDestroy.NyuOnDestroy();

                    if (hs_Components.Remove(hs_Components[i]))
                    {
                        Destroy(hs_Components[i] as Component);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to remove an entity component.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool RemoveNyuComponent(Type type)
        {
            for (int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i].GetType() == type)
                {
                    if (hs_Components[i] is INyuOnDestroyQueued nyuQueue)
                        nyuQueue.NyuOnDestroyQueued();

                    if (hs_Components[i] is INyuOnDestroy nyuDestroy)
                        nyuDestroy.NyuOnDestroy();

                    if (hs_Components.Remove(hs_Components[i]))
                    {
                        Destroy(hs_Components[i] as Component);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Equivalent to <see cref="RemoveNyuComponent(Type)"/> with a shorter name.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool RNC(Type type)
        {
            for (int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i].GetType() == type)
                {
                    if (hs_Components[i] is INyuOnDestroyQueued nyuQueue)
                        nyuQueue.NyuOnDestroyQueued();

                    if (hs_Components[i] is INyuOnDestroy nyuDestroy)
                        nyuDestroy.NyuOnDestroy();

                    if (hs_Components.Remove(hs_Components[i]))
                    {
                        Destroy(hs_Components[i] as Component);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes a <see cref="NyuComponent"/> by reference.
        /// </summary>
        /// <param name="existing"></param>
        /// <returns></returns>
        public bool RemoveNyuComponent(INyuComponent existing)
        {
            int index = hs_Components.IndexOf(existing);
            if (index != -1)
            {
                if (existing is INyuOnDestroyQueued nyuQueue)
                    nyuQueue.NyuOnDestroyQueued();

                if (existing is INyuOnDestroy nyuDestroy)
                    nyuDestroy.NyuOnDestroy();

                if (hs_Components.Remove(existing))
                {
                    Destroy(existing as Component);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Equivalent to <see cref="RemoveNyuComponent(INyuComponent)"/> with a shorter name.
        /// </summary>
        /// <param name="existing"></param>
        /// <returns></returns>
        public bool RNC(INyuComponent existing)
        {
            int index = hs_Components.IndexOf(existing);
            if (index != -1)
            {
                if (existing is INyuOnDestroyQueued nyuQueue)
                    nyuQueue.NyuOnDestroyQueued();

                if (existing is INyuOnDestroy nyuDestroy)
                    nyuDestroy.NyuOnDestroy();

                if (hs_Components.Remove(existing))
                {
                    Destroy(existing as Component);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to retrieve a component of given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetNyuComponent<T>()
        {
            foreach(NyuComponent ec in hs_Components)
            {
                if (ec is T toReturn)
                    return toReturn;
            }

            return default;
        }

        /// <summary>
        /// Equivalent to <see cref="GetNyuComponent{T}"/> with a shorter name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GNC<T>()
        {
            foreach (NyuComponent ec in hs_Components)
            {
                if (ec is T toReturn)
                    return toReturn;
            }

            return default;
        }

        /// <summary>
        /// Attempts to retrieve a component of given non-generic type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public NyuComponent GetNyuComponent(Type type)
        {
            foreach (NyuComponent ec in hs_Components)
            {
                if (ec.GetType().IsAssignableFrom(type))
                    return ec;
            }

            return default;
        }

        /// <summary>
        /// Equivalent to <see cref="GetNyuComponent(Type)"/> with a shorter name.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public NyuComponent GNC(Type type)
        {
            foreach (NyuComponent ec in hs_Components)
            {
                if (ec.GetType().IsAssignableFrom(type))
                    return ec;
            }

            return default;
        }

        /// <summary>
        /// Attempts to retrieve a component of given T type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGetNyuComponent<T>(out T component)
        {
            foreach(NyuComponent ec in hs_Components)
            {
                if(ec is T toReturn)
                {
                    component = toReturn;
                    return true;
                }
            }

            component = default;
            return false;
        }

        /// <summary>
        /// Equivalent to <see cref="TryGetNyuComponent{T}(out T)"/> with a shorter name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TGNC<T>(out T component)
        {
            foreach (NyuComponent ec in hs_Components)
            {
                if (ec is T toReturn)
                {
                    component = toReturn;
                    return true;
                }
            }

            component = default;
            return false;
        }

        /// <summary>
        /// Returns a list of components of given T type. (Useful for interface searching)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetNyuComponents<T>()
        {
            List<T> toReturn = new List<T>(hs_Components.Count);
            foreach(INyuComponent ec in hs_Components)
            {
                if (ec is T toAdd)
                    toReturn.Add(toAdd);
            }

            return toReturn;
        }

        /// <summary>
        /// Returns all <see cref="NyuComponent"/> objects present in this <see cref="Nyu"/>.
        /// </summary>
        /// <returns></returns>
        public List<INyuComponent> GetNyuComponents() => hs_Components;
        #endregion

        #region Destroying
        /// <summary>
        /// Invoked right after an entity is queued for destruction.
        /// </summary>
        public event Action OnDestroyBegin;

        internal bool ns_DestroyedCorrectly;

        private void OnDestroy()
        {
            if (!ns_DestroyedCorrectly)
                throw new NyuEntityException(2);

            Application.quitting -= Event_RemoveDestroyCheck;
        }
        #endregion

        #region Collision
        private void OnTriggerEnter(Collider other)
        {
            if (this is INyuOnTriggerEnter enter)
                enter.NyuOnTriggerEnter(other);

            for(int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i] is INyuOnTriggerEnter nyuEnter)
                    nyuEnter.NyuOnTriggerEnter(other);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (this is INyuOnTriggerEnter2D enter2D)
                enter2D.NyuOnTriggerEnter2D(collision);

            for (int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i] is INyuOnTriggerEnter2D nyuEnter2D)
                    nyuEnter2D.NyuOnTriggerEnter2D(collision);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (this is INyuOnTriggerExit exit)
                exit.NyuOnTriggerExit(other);

            for (int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i] is INyuOnTriggerExit nyuExit)
                    nyuExit.NyuOnTriggerExit(other);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (this is INyuOnTriggerExit2D exit2D)
                exit2D.NyuOnTriggerExit2D(collision);

            for (int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i] is INyuOnTriggerExit2D nyuExit2D)
                    nyuExit2D.NyuOnTriggerExit2D(collision);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (this is INyuOnCollisionEnter enter)
                enter.NyuOnCollisionEnter(collision);

            for (int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i] is INyuOnCollisionEnter nyuEnter)
                    nyuEnter.NyuOnCollisionEnter(collision);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (this is INyuOnCollisionEnter2D enter2D)
                enter2D.NyuOnCollisionEnter2D(collision);

            for (int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i] is INyuOnCollisionEnter2D nyuEnter2D)
                    nyuEnter2D.NyuOnCollisionEnter2D(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (this is INyuOnCollisionExit exit)
                exit.NyuOnCollisionExit(collision);

            for (int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i] is INyuOnCollisionExit nyuExit)
                    nyuExit.NyuOnCollisionExit(collision);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (this is INyuOnCollisionExit2D exit2D)
                exit2D.NyuOnCollisionExit2D(collision);

            for (int i = 0; i < hs_Components.Count; i++)
            {
                if (hs_Components[i] is INyuOnCollisionExit2D nyuExit2D)
                    nyuExit2D.NyuOnCollisionExit2D(collision);
            }
        }
        #endregion

        /// <summary>
        /// Returns the Nyu's .transform.
        /// </summary>
        /// <param name="e"></param>
        public static implicit operator Transform(Nyu e)
            => e.transform;
    }
}
