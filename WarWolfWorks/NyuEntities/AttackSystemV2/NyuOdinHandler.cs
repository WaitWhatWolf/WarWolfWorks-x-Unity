using System;
using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Utility;
using static WarWolfWorks.Constants;

namespace WarWolfWorks.NyuEntities.AttackSystemV2
{
    /// <summary>
    /// A class which handles <see cref="Odin"/> classes for attacking.
    /// </summary>
    [CompleteNoS]
    public sealed class NyuOdinHandler : NyuComponent, INyuAwake, INyuUpdate, INyuFixedUpdate, INyuLateUpdate
    {
        /// <summary>
        /// Invoked when a <see cref="Freki"/> successfully attacks.
        /// </summary>
        public event Action<Freki> OnAttackTrigger;

        [SerializeField]
        private List<Odin> s_Odins;

        /// <summary>
        /// Returns an array of all odins of this <see cref="NyuOdinHandler"/>.
        /// </summary>
        public Odin[] GetOdins() => s_Odins.ToArray();

        /// <summary>
        /// Returns the amount of odins currently in the <see cref="NyuOdinHandler"/>.
        /// </summary>
        /// <returns></returns>
        public int GetOdinsCount() => s_Odins.Count;

        /// <summary>
        /// The transform points used as firepoint on this <see cref="NyuOdinHandler"/>.
        /// </summary>
        public Transform[] Points;

        [SerializeField, Tooltip("When checked, all Odin components will be instantiated before being used. Otherwise the original object will be used.")]
        private bool s_InstantiateOnStart = true;

        /// <summary>
        /// Returns the point at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Transform GetPoint(int index)
            => Points[index];

        void INyuAwake.NyuAwake()
        {
            if(s_InstantiateOnStart)
            {
                s_Odins = new List<Odin>(Hooks.Enumeration.InstantiateList(s_Odins));
            }

            for(int i = 0; i < s_Odins.Count; i++)
            {
                InitOdin(s_Odins[i], i);
            }
        }

        private void InitOdin(Odin odin, int toIndex)
        {
            if(odin == null)
            {
                throw new NullReferenceException("Cannot set-up a null odin object. Aborting...");
            }
            odin.Index = toIndex;
            odin.Parent = this;
            odin.SetRespectiveParents();

            if (odin.GetFreki() is INyuAwake awake)
                awake.NyuAwake();

            if (odin.GetGeri() is INyuAwake geriAwake)
                geriAwake.NyuAwake();

            odin.SetUpdates();
        }
        
        /// <summary>
        /// Refreshes the index values of all odins.
        /// </summary>
        private void RefreshOdins()
        {
            for(int i = 0; i < s_Odins.Count; i++)
            {
                s_Odins[i].Index = i;
            }
        }

        /// <summary>
        /// Adds an odin to be handled.
        /// </summary>
        /// <param name="odin"></param>
        public bool AddOdin(Odin odin)
        {
            if (odin == null)
                return false;

            s_Odins.Add(odin);
            InitOdin(odin, s_Odins.Count - 1);
            return true;
        }

        /// <summary>
        /// Removes an odin at the specified index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveOdin(int index)
        {
            if (s_Odins[index].GetFreki() is INyuOnDestroy destroy)
                destroy.NyuOnDestroy();

            s_Odins.RemoveAt(index);
            RefreshOdins();
        }

        /// <summary>
        /// Returns the index of an odin currently residing in this <see cref="NyuOdinHandler"/>.
        /// </summary>
        /// <param name="odin"></param>
        /// <returns></returns>
        public int GetOdinIndex(Odin odin) => s_Odins.IndexOf(odin);

        /// <summary>
        /// Returns the <see cref="Odin"/> located at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Odin GetOdin(int index) => s_Odins[index];

        /// <summary>
        /// Returns a <see cref="Odin"/> based on a match.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public Odin GetOdin(Predicate<Odin> match) => s_Odins.Find(match);

        /// <summary>
        /// Attempts to find an odin's index based on a match.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int GetOdinIndex(Predicate<Odin> match) => s_Odins.FindIndex(match);

        /// <summary>
        /// Overrides an existing odin at the given index with another odin.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="to"></param>
        public void SetOdin(int index, Odin to)
        {
            if(to == null)
            {
                AdvancedDebug.LogWarning("Cannot set an odin to a null value. Check of the odin is not null or use RemoveOdin(int) instead.",
                    DEBUG_LAYER_WWW_INDEX);
                return;
            }
            s_Odins[index] = to;
            InitOdin(s_Odins[index], index);
        }

        void INyuUpdate.NyuUpdate()
        {
            for(int i = 0; i < s_Odins.Count; i++)
            {
                Freki freki = s_Odins[i].GetFreki();
                Geri geri = s_Odins[i].GetGeri();

                if (s_Odins[i].IsUpdate[0])
                    s_Odins[i].Freki_Update.NyuUpdate();

                if (freki.CanAttack() && (geri == null || (geri != null && geri.Met())))
                {
                    freki.CallTrigger();
                    OnAttackTrigger?.Invoke(freki);
                }
            }
        }

        void INyuFixedUpdate.NyuFixedUpdate()
        {
            for (int i = 0; i < s_Odins.Count; i++)
            {
                if (s_Odins[i].IsUpdate[1])
                    s_Odins[i].Freki_FixedUpdate.NyuFixedUpdate();
            }
        }

        void INyuLateUpdate.NyuLateUpdate()
        {
            for (int i = 0; i < s_Odins.Count; i++)
            {
                if (s_Odins[i].IsUpdate[2])
                    s_Odins[i].Freki_LateUpdate.NyuLateUpdate();
            }
        }
    }
}
