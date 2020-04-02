using System;
using System.Collections.Generic;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Interfaces.UnityMethods;

namespace WarWolfWorks.NyuEntities.YharonSystem
{
    /// <summary>
    /// Entity component which applies <see cref="Yharon"/> effects on an entity.
    /// </summary>
    public sealed class NyuYharon : NyuComponent, 
        INyuUpdate, INyuFixedUpdate, INyuLateUpdate, INyuOnEnable, INyuOnDisable, INyuOnDestroyQueued
    {
        internal List<Yharon> ns_Yharons = new List<Yharon>();

        #region TaCx1 stuff
        internal List<TaCx1> ns_TaCx1s = new List<TaCx1>();

        /// <summary>
        /// Attempts to see if this <see cref="NyuYharon"/> has a resistance to the given <see cref="Yharon"/> type,
        /// and returns in percent (0-1) the resistance amount.
        /// </summary>
        /// <param name="for"></param>
        /// <param name="percent01"></param>
        /// <returns></returns>
        public bool GetTaCx1(Type @for, out float percent01)
        {
            int index = ns_TaCx1s.FindIndex(t => t.Resistance == @for);
            if(index != -1)
            {
                percent01 = ns_TaCx1s[index].Percent;
                return true;
            }

            percent01 = 0;
            return false;
        }

        /// <summary>
        /// Adds a resistance to a specific <see cref="Yharon"/>, and sets how resistant it is. (0-1, 0 being none 1 being completely resistant)
        /// Note: If a resistance type already exists, it will override the existing resistance's value instead of adding a new one.
        /// </summary>
        /// <param name="for"></param>
        /// <param name="percent01"></param>
        public void AddTaCx1(Type @for, float percent01)
        {
            int index = ns_TaCx1s.FindIndex(t => t.Resistance == @for);
            if(index == -1)
            {
                ns_TaCx1s.Add(new TaCx1(@for, percent01));
            }
            else if(ns_TaCx1s[index].Percent != percent01)
            {
                ns_TaCx1s[index] = new TaCx1(@for, percent01);
            }
        }

        /// <summary>
        /// Removes the resistance to a <see cref="Yharon"/> previously set using <see cref="AddTaCx1(Type, float)"/>.
        /// </summary>
        /// <param name="for"></param>
        public bool RemoveTaCx1(Type @for)
        {
            for(int i = 0; i < ns_TaCx1s.Count; i++)
                if(ns_TaCx1s[i].Resistance == @for)
                {
                    ns_TaCx1s.RemoveAt(i);
                    return true;
                }

            return false;
        }
        #endregion

        #region Events
        /// <summary>
        /// Invoked when a <see cref="Yharon"/> is successfully added.
        /// </summary>
        public event Action<Yharon> OnYharonAdded;
        /// <summary>
        /// Invoked when a <see cref="Yharon"/> is successfully removed.
        /// </summary>
        public event Action<Yharon> OnYharonRemoved;
        /// <summary>
        /// Invoked when a <see cref="Yharon"/> is overriden through <see cref="YharonApplication.Override"/>.
        /// </summary>
        public event Action<Yharon> OnYharonOverriden;
        #endregion

        #region Utility
        /// <summary>
        /// Returns true if a <see cref="Yharon"/> has a <see cref="Yharon.YharonType"/> equal to the given type.
        /// </summary>
        /// <param name="yharonType"></param>
        /// <returns></returns>
        public bool Contains(Type yharonType)
        {
            foreach (Yharon yharon in ns_Yharons)
                if (yharon.YharonType == yharonType)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns true if a <see cref="Yharon"/> has a <see cref="Yharon.YharonType"/> equal to the given type,
        /// and returns the first element found.
        /// </summary>
        /// <param name="yharonType"></param>
        /// <param name="found"></param>
        /// <returns></returns>
        public bool Contains(Type yharonType, out Yharon found)
        {
            foreach (Yharon yharon in ns_Yharons)
                if (yharon.YharonType == yharonType)
                {
                    found = yharon;
                    return true;
                }

            found = null;
            return false;
        }

        /// <summary>
        /// Returns true if a <see cref="Yharon"/> has a <see cref="Yharon.YharonType"/> equal to the given type.
        /// </summary>
        /// <returns></returns>
        public bool Contains<T>()
        {
            foreach (Yharon yharon in ns_Yharons)
                if (yharon.YharonType == typeof(T))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns true if a <see cref="Yharon"/> has a <see cref="Yharon.YharonType"/> equal to the given type,
        /// and returns the first element found.
        /// </summary>
        /// <param name="found"></param>
        /// <returns></returns>
        public bool Contains<T>(out Yharon found)
        {
            foreach (Yharon yharon in ns_Yharons)
                if (yharon.YharonType == typeof(T))
                {
                    found = yharon;
                    return true;
                }

            found = null;
            return false;
        }

        /// <summary>
        /// Returns true if the given <see cref="Yharon"/> is currently inside the <see cref="NyuYharon"/>.
        /// (Note: This is not the same as <see cref="Contains(Type)"/>)
        /// </summary>
        /// <param name="yharon"></param>
        /// <returns></returns>
        public bool Contains(Yharon yharon) => ns_Yharons.Contains(yharon);
        #endregion

        #region Adding / Removing
        /// <summary>
        /// Adds a <see cref="Yharon"/> to the list of affecting Yharons.
        /// Returns false if a TaCx1 (resistance) of the yharon's type exists with a value of 1.
        /// </summary>
        /// <param name="yharon"></param>
        public bool AddYharon(Yharon yharon)
        {
            if (GetTaCx1(yharon.YharonType, out float percent01) && percent01 == 1)
                return false;

            if(Contains(yharon.YharonType, out Yharon found))
            {
                if (yharon.Application.HasFlag(YharonApplication.Override))
                    InternalCallYharonOverride(found);

                if (yharon.Application.HasFlag(YharonApplication.Remove))
                    RemoveYharon(found);

                if (yharon.Application.HasFlag(YharonApplication.Add))
                    InternalAddYharon(yharon);
            }
            else
            {
                InternalAddYharon(yharon);
            }

            return true;
        }

        /// <summary>
        /// Removes a given yharon.
        /// </summary>
        /// <param name="yharon"></param>
        public bool RemoveYharon(Yharon yharon)
        {
            if(ns_Yharons.Contains(yharon))
            {
                if (yharon is IOnDestroy yharonDestroy)
                    yharonDestroy.OnDestroy();

                ns_Yharons.Remove(yharon);
                OnYharonRemoved?.Invoke(yharon);
                return true;
            }

            return false;
        }
        #endregion

        #region Internal Handling
        private void InternalAddYharon(Yharon yharon)
        {
            ns_Yharons.Add(yharon);
            yharon.Parent = this;

            if (yharon is IAwake yharonAwake)
                yharonAwake.Awake();

            OnYharonAdded?.Invoke(yharon);
        }

        private void InternalCallYharonOverride(Yharon yharon)
        {
            yharon.CallOnOverride(yharon);
            OnYharonOverriden?.Invoke(yharon);
        }

        void INyuOnDisable.NyuOnDisable()
        {
            foreach(Yharon yharon in ns_Yharons)
            {
                if (yharon is IOnDisable yharonOnDisable)
                    yharonOnDisable.OnDisable();
            }
        }

        void INyuOnEnable.NyuOnEnable()
        {
            foreach (Yharon yharon in ns_Yharons)
            {
                if (yharon is IOnEnable yharonOnEnable)
                    yharonOnEnable.OnEnable();
            }
        }

        void INyuLateUpdate.NyuLateUpdate()
        {
            for (int i = 0; i < ns_Yharons.Count; i++)
                if (ns_Yharons[i] is ILateUpdate yharonLateUpdate)
                    yharonLateUpdate.LateUpdate();
        }

        void INyuFixedUpdate.NyuFixedUpdate()
        {
            for (int i = 0; i < ns_Yharons.Count; i++)
                if (ns_Yharons[i] is IFixedUpdate yharonFixedUpdate)
                    yharonFixedUpdate.FixedUpdate();
        }

        void INyuUpdate.NyuUpdate()
        {
            for (int i = 0; i < ns_Yharons.Count; i++)
                if (ns_Yharons[i] is IUpdate yharonUpdate)
                    yharonUpdate.Update();
        }

        void INyuOnDestroyQueued.NyuOnDestroyQueued()
        {
            for (int i = 0; i < ns_Yharons.Count; i++)
                if (ns_Yharons[i] is IOnDestroy yharonDestroy)
                    yharonDestroy.OnDestroy();
        }
        #endregion
    }
}
