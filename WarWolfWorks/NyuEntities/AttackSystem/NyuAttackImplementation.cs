using System;
using UnityEngine;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.NyuEntities.AttackSystem
{
    /// <summary>
    /// Core class of the attack system. (sealed)
    /// </summary>
    public sealed partial class NyuAttack : NyuComponent, ILockable
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

            for (int i = 0; i < AllAttacks.Count; i++)
                AllAttacks[i].Attack.TimeScale = Locked ? 0 : 1;

            if (Locked)
            {
                OnLocked?.Invoke(this);
            }
            else OnUnlocked?.Invoke(this);
        }

        [Serializable]
        internal class SubAttackComponent : IParentable<NyuAttack>
        {
            [SerializeField]
            private Attack s_Attack;
            public Attack Attack { get => s_Attack; set => s_Attack = value; }
            [SerializeField]
            private Condition s_Condition;
            public Condition Condition { get => s_Condition; set => s_Condition = value; }

            public Transform Point;

            public bool Active;

            public bool InstantiatesAttack;

            public bool InstantiatesCondition;

            public NyuAttack Parent { get; }

            public SubAttackComponent(Attack attack, Condition condition, Transform attackPoint, bool canAttack, bool instantiatesAttack, bool instantiatesCondition, NyuAttack parent)
            {
                Attack = attack;
                Condition = condition;
                Point = attackPoint;
                Active = canAttack;
                InstantiatesAttack = instantiatesAttack;
                InstantiatesCondition = instantiatesCondition;
                Parent = parent;
            }
        }
    }
}
