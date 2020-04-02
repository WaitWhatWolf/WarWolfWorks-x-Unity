using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

[assembly: InternalsVisibleTo("WarWolfWorks.EditorBase")]
namespace WarWolfWorks.EntitiesSystem.Attacking
{
    /// <summary>
    /// Class used for entity attacking. (sealed)
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public sealed partial class EntityAttack : EntityComponent, ILockable, INyuAwake //This part takes care of inheriting monobehaviour and implementing ILockable as well as making the SubAttackComponent class.
    {
        /// <summary>
        /// The object's Lock state; See <see cref="ILockable"/> for more info.
        /// </summary>
        public bool Locked { get; private set; }

        private bool nss_NyuInitiated;
        /// <summary>
        /// Initiated state of the <see cref="EntityAttack"/>. (<see cref="INyuAwake"/> implementation, avoid setting manually.)
        /// </summary>
        public bool NyuInitiated { get => nss_NyuInitiated; set { if (!nss_NyuInitiated) { nss_NyuInitiated = value; } } }

        /// <summary>
        /// Called when the object is locked (<see cref="ILockable"/> implementation).
        /// </summary>
        public event Action<ILockable> OnLocked;
        /// <summary>
        /// Called when the object is unlocked (<see cref="ILockable"/> implementation).
        /// </summary>
        public event Action<ILockable> OnUnlocked;

        void INyuAwake.NyuAwake()
        {
            InstantiateAllAttacks();
        }

        /// <summary>
        /// Locks or Unlocks this object (<see cref="ILockable"/> implementation).
        /// </summary>
        /// <param name="to"></param>
        public void SetLock(bool to)
        {
            if (to == Locked)
                return;

            Locked = to;
            if (Locked) OnLocked?.Invoke(this);
            else OnUnlocked?.Invoke(this);
        }

        [Serializable]
        internal class SubAttackComponent
        {
            [FormerlySerializedAs("attack"), SerializeField]
            private Attack s_Attack;
            public Attack Attack { get => s_Attack; set => s_Attack = value; }
            [FormerlySerializedAs("condition"), SerializeField]
            private Condition s_Condition;
            public Condition Condition { get => s_Condition; set => s_Condition = value; }
            [FormerlySerializedAs("AttackPoint")]
            public Transform Point;
            [FormerlySerializedAs("CanAttack")]
            public bool Active;
            [Tooltip("If true, all attacks added/changed (outside of the inspector) will be duplicated instead of using the original.")]
            public bool InstantiatesAttack;
            [Tooltip("If true,all attacks added/changed (outside of the inspector) will be duplicated instead of using the original.")]
            public bool InstantiatesCondition;

            internal EntityAttack ParentClass;

            public SubAttackComponent(Attack attack, Condition condition, Transform attackPoint, bool canAttack, bool instantiatesAttack, bool instantiatesCondition, EntityAttack parent)
            {
                Attack = attack;
                Condition = condition;
                Point = attackPoint;
                Active = canAttack;
                InstantiatesAttack = instantiatesAttack;
                InstantiatesCondition = instantiatesCondition;
                ParentClass = parent;
            }
        }
    }
}

