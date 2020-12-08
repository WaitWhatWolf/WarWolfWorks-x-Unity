using System;
using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Interfaces.UnityMethods;
using static WarWolfWorks.Constants;

namespace WarWolfWorks.NyuEntities.AttackSystem
{
    public sealed partial class NyuAttack : INyuPreAwake, INyuUpdate, INyuFixedUpdate, INyuLateUpdate, INyuOnEnable, INyuOnDisable, INyuOnDestroy
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
            foreach (int i in ofIndex)
            {
                if (AllAttacks[i].Attack is T) toReturn.Add((T)AllAttacks[i].Attack);
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
        public bool GetCanAttack(int index) => AllAttacks[index].Active;

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
                use.Initiate(NyuMain);
                AllAttacks[groupIndex].Attack = use;
            }
            catch (Exception e)
            {
                AdvancedDebug.LogWarningFormat("Couldn't add/set {0} under Attack Group {1}: {2}", DEBUG_LAYER_EXCEPTIONS_INDEX, atk, groupIndex, e);
            }
        }

        /// <summary>
        /// Sets the <see cref="Attack"/> component under groupIndex.
        /// </summary>
        /// <param name="groupIndex"></param>
        /// <param name="atk"></param>
        public void SetAttack(int groupIndex, ref Attack atk)
        {
            try
            {
                Attack use = AllAttacks[groupIndex].InstantiatesAttack ? Instantiate(atk) : atk;
                use.Initiate(NyuMain);
                AllAttacks[groupIndex].Attack = use;
            }
            catch (Exception e)
            {
                AdvancedDebug.LogWarningFormat("Couldn't add/set {0} under attak group {1}: {2}", DEBUG_LAYER_EXCEPTIONS_INDEX, atk, groupIndex, e);
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
            AllAttacks[groupIndex].Active = to;
        }

        /// <summary>
        /// Adds an attack group to all attacks of this <see cref="EntityAttack"/>.
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="condition"></param>
        /// <param name="point"></param>
        /// <param name="canAttack"></param>
        /// <returns></returns>
        public int AddAttack(Attack attack, Condition condition, Transform point, bool canAttack)
        {
            int toReturn = AllAttacks.Count;
            AllAttacks.Add(new SubAttackComponent(attack, condition, point, canAttack, false, false, this));

            return toReturn;
        }

        /// <summary>
        /// Removes an attack at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveAttack(int index)
        {
            try { AllAttacks.RemoveAt(index); return true; }
            catch { return false; }
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
            for (int i = 0; i < AllAttacks.Count; i++)
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

        private void InstantiateAllAttacks()
        {
            for (int i = 0; i < AllAttacks.Count; i++)
            {
                SubAttackComponent stored = AllAttacks[i];
                Attack use = stored.InstantiatesAttack && stored.Attack ?
                Instantiate(stored.Attack) : stored.Attack;
                if (use)
                {
                    use.IsInitiated = false;
                    use.Initiate(NyuMain);
                }

                AllAttacks[i] = new SubAttackComponent(
                use,

                stored.InstantiatesCondition && stored.Condition ?
                Instantiate(stored.Condition) : stored.Condition,

                stored.Point,
                stored.Active,
                stored.InstantiatesAttack,
                stored.InstantiatesCondition,
                this
                );
            }
        }

        /// <summary>
        /// Unity's Update method called by NyuComponent.
        /// </summary>
        void INyuUpdate.NyuUpdate()
        {
            for (int i = 0; i < AllAttacks.Count; i++)
            {
                AllAttacks[i].Attack.TimeScale = Locked ? 0 : 1;
                AllAttacks[i].Attack.NyuUpdate();

                if (AllAttacks[i].Condition && AllAttacks[i].Condition.Met(AllAttacks[i].Attack) && AllAttacks[i].Active)
                {
                    if (AllAttacks[i].Attack.CanAttack)
                        OnBeforeAttackTriggered?.Invoke(AllAttacks[i].Attack);
                    if (AllAttacks[i].Attack.Trigger())
                        OnAttackTriggered?.Invoke(AllAttacks[i].Attack);
                }
            }
        }

        void INyuFixedUpdate.NyuFixedUpdate()
        {
            for (int i = 0; i < AllAttacks.Count; i++)
            {
                if (AllAttacks[i] is IFixedUpdate attackFixedUpdate)
                    attackFixedUpdate.FixedUpdate();
            }
        }

        void INyuLateUpdate.NyuLateUpdate()
        {
            for (int i = 0; i < AllAttacks.Count; i++)
            {
                if (AllAttacks[i] is IFixedUpdate attackFixedUpdate)
                    attackFixedUpdate.FixedUpdate();
            }
        }

        void INyuOnDisable.NyuOnDisable()
        {
            for (int i = 0; i < AllAttacks.Count; i++)
            {
                if (AllAttacks[i] is IOnDisable attackOnDisable)
                    attackOnDisable.OnDisable();
            }
        }

        void INyuOnEnable.NyuOnEnable()
        {
            for (int i = 0; i < AllAttacks.Count; i++)
            {
                if (AllAttacks[i] is IOnEnable attackOnEnable)
                    attackOnEnable.OnEnable();
            }
        }

        void INyuOnDestroy.NyuOnDestroy()
        {
            for (int i = 0; i < AllAttacks.Count; i++)
            {
                if (AllAttacks[i] is INyuOnDestroy attackOnDestroy)
                    attackOnDestroy.NyuOnDestroy();
            }
        }

        void INyuPreAwake.NyuPreAwake()
        {
            InstantiateAllAttacks();
        }
    }
}
