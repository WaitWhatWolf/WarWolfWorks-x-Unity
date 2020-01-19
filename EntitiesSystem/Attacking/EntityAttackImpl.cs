using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;

[assembly: InternalsVisibleTo("WarWolfWorks.EditorBase")]
namespace WarWolfWorks.EntitiesSystem.Attacking
{
    /// <summary>
    /// Class used for entity attacking. (sealed)
    /// </summary>
    public sealed partial class EntityAttack : EntityComponent, ILockable //This part takes care of inheriting monobehaviour and implementing ILockable as well as making the SubAttackComponent class.
    {
        /// <summary>
        /// The object's Lock state; See <see cref="ILockable"/> for more info.
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Called when the object is locked (<see cref="ILockable"/> implementation).
        /// </summary>
        public event Action<ILockable> OnLocked;
        /// <summary>
        /// Called when the object is unlocked (<see cref="ILockable"/> implementation).
        /// </summary>
        public event Action<ILockable> OnUnlocked;

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
            [SerializeField]
            private Attack attack;
            public Attack Attack { get => attack; set => attack = value; }
            [SerializeField]
            private Condition condition;
            public Condition Condition { get => condition; set => condition = value; }
            [FormerlySerializedAs("AttackPoint")]
            public Transform Point;
            public bool CanAttack;
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
                CanAttack = canAttack;
                InstantiatesAttack = instantiatesAttack;
                InstantiatesCondition = instantiatesCondition;
                ParentClass = parent;
            }
        }
    }
}

