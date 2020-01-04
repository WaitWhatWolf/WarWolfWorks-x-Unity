using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Attacking
{
    /// <summary>
    /// Class used for entity attacking. (sealed)
    /// </summary>
    public sealed partial class EntityAttack : EntityComponent, ILockable
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

#if !WWW2_5_OR_HIGHER
        [Serializable]
        public class SubAttackComponent
        {
            public Attack Attack;
            public Condition Condition;
            public Transform AttackPoint;
            public bool CanAttack;

            internal EntityAttack ParentClass;

            public void SetAttack(Attack attack, Entity parent)
            {
                Attack = ParentClass.InstantiatesAttacks ? Instantiate(attack) : attack;
                Attack.Initiate(parent);
            }
            public void SetCondition(Condition condition)
            {
                Condition = ParentClass.InstantiatesConditions ? Instantiate(condition) : condition;
            }
            public void SetAttackPoint(Transform point) => AttackPoint = point;
            public void SetCanAttack(bool to) => CanAttack = to;

            public SubAttackComponent(Attack atk, Condition condition,
                Transform attackPoint, bool canatk)
            {
                Attack = atk;

                if (condition == null)
                    Condition = Attack.DefaultAttackCondition;
                else
                    Condition = condition;

                AttackPoint = attackPoint;

                CanAttack = canatk;
            } 
        }
#else
        [Serializable]
        private class SubAttackComponent
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
#endif
    }

    public sealed partial class EntityAttack
    {
        [SerializeField]
        private List<SubAttackComponent> AllAttacks = new List<SubAttackComponent>();
        /// <summary>
        /// Triggers when this entity successfully attacks.
        /// </summary>
        public event Action<Attack> OnAttackTriggered;
        /// <summary>
        /// Triggers right before this entity attack. (only triggers if the attack was successful)
        /// </summary>
        public event Action<Attack> OnBeforeAttackTriggered;

#if !WWW2_5_OR_HIGHER
        [SerializeField]
        [Tooltip("If true, the attacks used will be instantiated and a clone will be used instead of the original one.")]
        private bool InstantiateAttacksOnSet;
        [SerializeField]
        [Tooltip("If true, the conditions used will be instantiated and a clone will be used instead of the original one.")]
        private bool InstantiateConditionsOnSet;
        internal bool InstantiatesAttacks => InstantiateAttacksOnSet;
        internal bool InstantiatesConditions => InstantiateConditionsOnSet;


        public SubAttackComponent GetSubAttack(int index) => AllAttacks[index];
        public SubAttackComponent GetSubAttack(Attack atk) => AllAttacks.Find(a => a.Attack.Equals(atk));
        public int GetSubAttackIndex(SubAttackComponent component)
            => AllAttacks.FindIndex(sac => sac.Equals(component));
        public int GetSubAttackIndex(Attack component)
            => AllAttacks.FindIndex(t => t.Attack == component);
        /// <summary>
        /// Tries to Get the SubAttack index while handling all errors automatically. If an error occurs or if the SubAttack was not found, -1 will be returned.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        [System.Obsolete("This method is now the exact same as GetSubAttackIndex.")]
        public int TryGetSubAttackIndex(Attack component)
            => AllAttacks.FindIndex(t => t.Attack == component);

        public void UseAllSubAttacks(Action<SubAttackComponent> action)
            => AllAttacks.ForEach(action);

        public override void OnAwake()
        {
            for (int i = 0; i < AllAttacks.Count; i++)
            {
                AllAttacks[i].ParentClass = this;
                if (InstantiateAttacksOnSet) AllAttacks[i].SetAttack(AllAttacks[i].Attack, EntityMain);
                if (InstantiateConditionsOnSet) AllAttacks[i].SetCondition(AllAttacks[i].Condition);
            }
            AllAttacks.ForEach(sac => sac.Attack?.Initiate(EntityMain));
        }

        public override void OnUpdate()
        {
            foreach (SubAttackComponent sac in AllAttacks)
            {
                try
                {
                    sac.Attack.Update();

                    if (sac.Condition && sac.Condition.ConditionIsMet(sac.Attack) && sac.CanAttack)
                    {
                        if (sac.Attack.Trigger())
                            OnAttackTriggered?.Invoke(sac.Attack);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
#else
        #region Getting

        /// <summary>
        /// Gets all attacks of given indexes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ofIndex"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"/>
        public List<T> GetAttacks<T>(params int[] ofIndex) where T : Attack
        {
            List<T> toReturn = new List<T>(ofIndex.Length);
            foreach(int i in ofIndex)
            {
                if(AllAttacks[i].Attack is T) toReturn.Add((T)AllAttacks[i].Attack);
            }

            return toReturn;
        }

        /// <summary>
        /// Gets the <see cref="Attack"/> scriptable object through index. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Attack GetAttack(int index) => AllAttacks[index].Attack;

        /// <summary>
        /// Gets the <see cref="Attack"/> scriptable object through index; 
        /// In case of an exception, it will simply return null.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Attack TryGetAttack(int index)
        {
            try { return AllAttacks[index].Attack; }
            catch { return null; }
        }

        /// <summary>
        /// Gets the <see cref="Condition"/> of an attack group through index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Condition GetCondition(int index) => AllAttacks[index].Condition;

        /// <summary>
        /// Returns the first <see cref="Condition"/> inside an attack group which has the <see cref="Attack"/> given.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public Condition GetCondition(Attack of) => AllAttacks.Find(sac => sac.Attack == of).Condition;

        /// <summary>
        /// Gets the <see cref="Condition"/> of an attack group through index; 
        /// In case of an exception, it will simply return null.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Condition TryGetCondition(int index)
        {
            try { return AllAttacks[index].Condition; }
            catch { return null; }
        }

        /// <summary>
        /// Returns the first <see cref="Condition"/> inside an attack group which has the <see cref="Attack"/> given.
        /// In case of an exception, it will simply return null.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public Condition TryGetCondition(Attack of)
        {
            try { return AllAttacks.Find(sac => sac.Attack == of).Condition; }
            catch { return null; }
        }

        /// <summary>
        /// Returns the attack point (<see cref="Transform"/>) of an attack group through index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Transform GetPoint(int index) => AllAttacks[index].Point;
        /// <summary>
        /// Returns the first attack point (<see cref="Transform"/>) inside an attack group which has the <see cref="Attack"/> given.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public Transform GetPoint(Attack of) => AllAttacks.Find(sac => sac.Attack == of).Point;
        /// <summary>
        /// Returns the attack point (<see cref="Transform"/>) of an attack group through index.
        /// In case of an exception, it will simply return null.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Transform TryGetPoint(int index)
        {
            try { return AllAttacks[index].Point; }
            catch { return null; }
        }

        /// <summary>
        /// Returns the first attack point (<see cref="Transform"/>) inside an attack group which has the <see cref="Attack"/> given.
        /// In case of an exception, it will simply return null.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public Transform TryGetPoint(Attack of)
        {
            try { return AllAttacks.Find(sac => sac.Attack == of).Point; }
            catch { return null; }
        }

        /// <summary>
        /// Returns the CanAttack variable if the given group index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool GetCanAttack(int index) => AllAttacks[index].CanAttack;

        /// <summary>
        /// Gets the index of the group in which the given <see cref="Attack"/> component resides.
        /// Returns -1 if Group of <see cref="Attack"/> given was not found.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public int GetIndex(Attack of) => AllAttacks.FindIndex(sac => sac.Attack == of);
        /// <summary>
        /// Gets the index of the group in which the given <see cref="Condition"/> component resides.
        /// Returns -1 if Group of <see cref="Condition"/> given was not found.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public int GetIndex(Condition of) => AllAttacks.FindIndex(sac => sac.Condition == of);
        /// <summary>
        /// Gets the index of the group in which the given attack point (<see cref="Transform"/>) component resides.
        /// Returns -1 if Group of attack point given was not found.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public int GetIndex(Transform of) => AllAttacks.FindIndex(sac => sac.Point == of);

        #endregion

        #region Setting
        /// <summary>
        /// Sets the <see cref="Attack"/> component under groupIndex.
        /// </summary>
        /// <param name="groupIndex"></param>
        /// <param name="atk"></param>
        public void SetAttack(int groupIndex, Attack atk)
        {
            try
            {
                Attack use = AllAttacks[groupIndex].InstantiatesAttack ? Instantiate(atk) : atk;
                use.Initiate(EntityMain);
                AllAttacks[groupIndex].Attack = use;/*= new SubAttackComponent
                (
                    use,
                    AllAttacks[groupIndex].Condition,
                    AllAttacks[groupIndex].Point,
                    AllAttacks[groupIndex].CanAttack,
                    AllAttacks[groupIndex].InstantiatesAttack,
                    AllAttacks[groupIndex].InstantiatesCondition,
                    AllAttacks[groupIndex].ParentClass
                );*/
            }
            catch (Exception e)
            {
                AdvancedDebug.LogWarningFormat("Couldn't add/set {0} under Attack Group {1}: {2}", AdvancedDebug.ExceptionLayerIndex, atk, groupIndex, e);
            }
        }

        public void SetAttack(int groupIndex, ref Attack atk)
        {
            try
            {
                Attack use = AllAttacks[groupIndex].InstantiatesAttack ? Instantiate(atk) : atk;
                use.Initiate(EntityMain);
                AllAttacks[groupIndex].Attack = use;/*= new SubAttackComponent
                (
                    use,
                    AllAttacks[groupIndex].Condition,
                    AllAttacks[groupIndex].Point,
                    AllAttacks[groupIndex].CanAttack,
                    AllAttacks[groupIndex].InstantiatesAttack,
                    AllAttacks[groupIndex].InstantiatesCondition,
                    AllAttacks[groupIndex].ParentClass
                );*/
            }
            catch (Exception e)
            {
                AdvancedDebug.LogWarningFormat("Couldn't add/set {0} under attak group {1}: {2}", AdvancedDebug.ExceptionLayerIndex, atk, groupIndex, e);
            }
        }

        /// <summary>
        /// Sets the <see cref="Condition"/> component under groupIndex.
        /// </summary>
        /// <param name="groupIndex"></param>
        /// <param name="condition"></param>
        public void SetCondition(int groupIndex, Condition condition)
        {
            try
            {
                AllAttacks[groupIndex].Condition = AllAttacks[groupIndex].InstantiatesCondition ? Instantiate(condition) : condition;
                /* = new SubAttackComponent
                (
                    AllAttacks[groupIndex].Attack,
                    AllAttacks[groupIndex].InstantiatesCondition ? Instantiate(condition) : condition,
                    AllAttacks[groupIndex].Point,
                    AllAttacks[groupIndex].CanAttack,
                    AllAttacks[groupIndex].InstantiatesAttack,
                    AllAttacks[groupIndex].InstantiatesCondition,
                    AllAttacks[groupIndex].ParentClass
                );*/
            }
            catch (Exception e)
            {
                AdvancedDebug.LogWarningFormat("Couldn't add/set {0} under attak group {1}: {2}", 0, condition, groupIndex, e);
            }
        }

        /// <summary>
        /// Sets the attack point (<see cref="Transform"/>) component under groupIndex.
        /// </summary>
        /// <param name="groupIndex"></param>
        /// <param name="point"></param>
        public void SetAttackPoint(int groupIndex, Transform point)
        {
            try
            {
                AllAttacks[groupIndex].Point = point;
                    /*= new SubAttackComponent
                (
                    AllAttacks[groupIndex].Attack,
                    AllAttacks[groupIndex].Condition,
                    point,
                    AllAttacks[groupIndex].CanAttack,
                    AllAttacks[groupIndex].InstantiatesAttack,
                    AllAttacks[groupIndex].InstantiatesCondition,
                    AllAttacks[groupIndex].ParentClass
                );*/
            }
            catch (Exception e)
            {
                AdvancedDebug.LogWarningFormat("Couldn't add/set {0} under attak group {1}: {2}", 0, point, groupIndex, e);
            }
        }

        /// <summary>
        /// Set the CanAttack variable under groupIndex.
        /// </summary>
        /// <param name="groupIndex"></param>
        /// <param name="to"></param>
        public void SetCanAttack(int groupIndex, bool to)
        {
            AllAttacks[groupIndex].CanAttack = to;
                /*= new SubAttackComponent
            (
                AllAttacks[groupIndex].Attack,
                AllAttacks[groupIndex].Condition,
                AllAttacks[groupIndex].Point,
                to,
                AllAttacks[groupIndex].InstantiatesAttack,
                AllAttacks[groupIndex].InstantiatesCondition,
                AllAttacks[groupIndex].ParentClass
            );*/
        }
        #endregion

        #region Utility
        /// <summary>
        /// Loops through each Group's <see cref="Attack"/> component and calls given action with it.
        /// </summary>
        /// <param name="action"></param>
        public void ForEachAttack(Action<Attack> action) => AllAttacks.ForEach(a => action.Invoke(a.Attack));
        /// <summary>
        /// Loops through each Group's <see cref="Condition"/> component and calls given action with it.
        /// </summary>
        /// <param name="action"></param>
        public void ForEachCondition(Action<Condition> action)
            => AllAttacks.ForEach(a => action.Invoke(a.Condition));
        /// <summary>
        /// Loops through each Group's attack point (<see cref="Transform"/>) and calls given action with it.
        /// </summary>
        /// <param name="action"></param>
        public void ForEachPoint(Action<Transform> action)
            => AllAttacks.ForEach(a => action.Invoke(a.Point));
        /// <summary>
        /// Loops through each Group using their main variables and invokes given action based upon them.
        /// </summary>
        /// <param name="action"></param>
        public void ForEachGroup(Action<Attack, Condition, Transform> action)
            => AllAttacks.ForEach(a => action.Invoke(a.Attack, a.Condition, a.Point));

        /// <summary>
        /// A safer but slighly less optimal <see cref="ForEachAttack(Action{Attack})"/>.
        /// </summary>
        /// <param name="action"></param>
        public void SafeForEachAttack(Action<Attack> action)
        {
            for(int i = 0; i < AllAttacks.Count; i++)
            {
                try
                {
                    action.Invoke(AllAttacks[i].Attack);
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// A safer but slighly less optimal <see cref="ForEachCondition(Action{Condition})"/>.
        /// </summary>
        /// <param name="action"></param>
        public void SafeForEachCondition(Action<Condition> action)
        {
            for (int i = 0; i < AllAttacks.Count; i++)
            {
                try
                {
                    action.Invoke(AllAttacks[i].Condition);
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// A safer but slighly less optimal <see cref="ForEachPoint(Action{Transform})"/>.
        /// </summary>
        /// <param name="action"></param>
        public void SafeForEachPoint(Action<Transform> action)
        {
            for (int i = 0; i < AllAttacks.Count; i++)
            {
                try
                {
                    action.Invoke(AllAttacks[i].Point);
                }
                catch
                {
                    continue;
                }
            }
        }
        #endregion

        /// <summary>
        /// Unity's Awake method called by EntityComponent.
        /// </summary>
        public override void OnAwake()
        {
            InstantiateAllAttacks();
        }

        private void InstantiateAllAttacks()
        {

            for (int i = 0; i < AllAttacks.Count; i++)
            {
                SubAttackComponent stored = AllAttacks[i];
                Attack use = stored.InstantiatesAttack && stored.Attack ?
                Instantiate(stored.Attack) : stored.Attack;
                use?.Initiate(EntityMain);
                AllAttacks[i] = new SubAttackComponent(
                use,

                stored.InstantiatesCondition && stored.Condition ?
                Instantiate(stored.Condition) : stored.Condition,

                stored.Point,
                stored.CanAttack,
                stored.InstantiatesAttack,
                stored.InstantiatesCondition,
                this
                );
            }
        }

        /// <summary>
        /// Unity's Update method called by EntityComponent.
        /// </summary>
        public override void OnUpdate()
        {
            foreach (SubAttackComponent sac in AllAttacks)
            {
                try
                {
                    sac.Attack.TimeScale = Locked ? 0 : 1;
                    sac.Attack.Update();

                    if (sac.Condition && sac.Condition.ConditionIsMet(sac.Attack) && sac.CanAttack)
                    {
                        if (sac.Attack.CanAttack)
                            OnBeforeAttackTriggered?.Invoke(sac.Attack);
                        if (sac.Attack.Trigger())
                            OnAttackTriggered?.Invoke(sac.Attack);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
#endif
    }
}

