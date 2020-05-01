using System;
using System.Threading.Tasks;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.NyuEntities.Statistics;
using WarWolfWorks.Utility;

namespace WarWolfWorks.NyuEntities.HealthSystem
{
    /// <summary>
    /// Core class of the health system. (sealed)
    /// </summary>
    public sealed class NyuHealth : NyuComponent, IAdvancedHealth, INyuAwake
    {
        private Action<IAdvancedHealth, object, IHealthDamage> OnDmgHandler;
        /// <summary>
        /// What happends when <see cref="DamageHealth(object)"/> is successfully invoked.
        /// </summary>
        public event Action<IAdvancedHealth, object, IHealthDamage> OnDamaged
        {
            add
            {
                AdvancedDebug.LogFormat("{0} has been added to {1}({2})'s NyuHealth component!", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, NyuMain.Tag, NyuMain.GetType().Name);
                OnDmgHandler += value;
            }

            remove
            {
                AdvancedDebug.LogFormat("{0} has been removed from {1}({2})'s NyuHealth component.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, NyuMain.Tag, NyuMain.GetType().Name);
                OnDmgHandler -= value;
            }
        }

        private Action<IHealth> OnDthHandler;
        /// <summary>
        /// What happends when the <see cref="CurrentHealth"/> reaches 0.
        /// </summary>
        public event Action<IHealth> OnDeath
        {
            add
            {
                AdvancedDebug.LogFormat("{0} will now trigger when {1}({2})'s health reaches 0.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, NyuMain.Tag, NyuMain.GetType().Name);
                OnDthHandler += value;
            }
            remove
            {
                AdvancedDebug.LogFormat("{0} will no longer trigger when {1}({2})'s health reaches 0.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, NyuMain.Tag, NyuMain.GetType().Name);
                OnDthHandler -= value;
            }
        }

        private event Action<IHealth, float> onHlthAdd;
        /// <summary>
        /// Ivoked when <see cref="AddHealth(float)"/> is called. Float value is the amount of health that was added.
        /// </summary>
        public event Action<IHealth, float> OnHealthAdded
        {
            add
            {

                AdvancedDebug.LogFormat("{0} will now trigger when {1}({2})'s IHealth.AddHealth is called.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, NyuMain.Tag, NyuMain.GetType().Name);
                onHlthAdd += value;
            }
            remove
            {
                AdvancedDebug.LogFormat("{0} will no longer trigger when {1}({2})'s IHealth.AddHealth is called.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, NyuMain.Tag, NyuMain.GetType().Name);
                onHlthAdd -= value;
            }
        }

        /// <summary>
        /// Invoked when any <see cref="NyuHealth"/> gets successfully damaged.
        /// </summary>
        public static event Action<NyuHealth> OnAnyDamaged;
        /// <summary>
        /// Invoked when any <see cref="NyuHealth"/> invokes <see cref="AddHealth(float)"/> successfully.
        /// </summary>
        public static event Action<NyuHealth> OnAnyHealed;

        /// <summary>
        /// The current health of the <see cref="NyuComponent.NyuMain"/>.
        /// </summary>
        public float CurrentHealth { get; private set; }

        [SerializeField]
        private Stat s_MaxHealth = new Stat(0, 0, 0);
        /// <summary>
        /// The maximum value at which <see cref="CurrentHealth"/> can be.
        /// </summary>
        public float MaxHealth
        {
            get => NyuMain.Stats.CalculatedValue(s_MaxHealth);
            set => s_MaxHealth.SetValue = value;
        }

        [SerializeField]
        private bool s_DamageParent = true;
        /// <summary>
        /// If true and this component's <see cref="Nyu"/> is <see cref="IEntityParentable"/>,
        /// it will damage the parent's Health instead.
        /// </summary>
        public bool DamageParent { get => s_DamageParent; set => s_DamageParent = value; }

        private IAdvancedHealth Damaged { get; set; }

        /// <summary>
        /// Damage previously passed in <see cref="DamageHealth(object)"/>. (Only accepted)
        /// </summary>
        public object PreviousDamage { get; private set; }

        /// <summary>
        /// Unity's Awake method called by <see cref="NyuComponent"/>.
        /// </summary>
        void INyuAwake.NyuAwake()
        {
            CurrentHealth = MaxHealth;
            if (s_DestroyOnDeath)
            {
                OnDeath += Event_DeathDestroy;
            }

            if (Calculator == null) Calculator = ScriptableObject.CreateInstance<DefaultHealthDamage>();
            else Calculator = Instantiate(s_Calculator);
            if (s_ImmunityEffect != null) ImmunityEffect = Instantiate(s_ImmunityEffect);

            Event_SetDamagedHealth(NyuMain, NyuManager.OldestOf(NyuMain, true));
            if (NyuMain is INyuEntityParentable parentable) parentable.OnNyuParentSet += Event_SetDamagedHealth;
        }

        private void Event_SetDamagedHealth(Nyu child, Nyu parent)
        {
            if (DamageParent && parent != null) Damaged = parent.GNC<IAdvancedHealth>();
            if (Damaged == null) Damaged = this;
        }

        private void Event_DeathDestroy(IHealth h)
        {
            NyuManager.Destroy(NyuMain);
        }

        [SerializeField]
        private Stat s_ImmnunityDuration = new Stat(0, 0, 0);
        /// <summary>
        /// Duration in which the <see cref="Nyu"/> is unable to take damage. (Only works if <see cref="UsesImmunity"/> is true)
        /// </summary>
        public float ImmunityDuration => NyuMain.Stats.CalculatedValue(s_ImmnunityDuration);

        [SerializeField]
        private bool s_UsesImmunity;
        /// <summary>
        /// Determines if the <see cref="Nyu"/> can use the immunity system.
        /// </summary>
        public bool UsesImmunity => s_UsesImmunity;

        [SerializeField]
        private bool s_DestroyOnDeath;

        /// <summary>
        /// Is the <see cref="Nyu"/> currently immune?
        /// </summary>
        public bool IsImmune { get; set; }

        [SerializeField]
        private HealthDamage s_Calculator;
        /// <summary>
        /// The calculator used by this <see cref="Nyu"/>. If null on Awake, it will be set to <see cref="DefaultHealthDamage"/> by default.
        /// </summary>
        public IHealthDamage Calculator
        {
            get => s_Calculator;
            set => s_Calculator = value is HealthDamage healthDmg ? healthDmg : s_Calculator;
        }

        /// <summary>
        /// The amount that was successfully healed using <see cref="AddHealth(float)"/> on it's most recent use.
        /// </summary>
        public float PreviousHeal { get; private set; }

        /// <summary>
        /// Adds the specified amount to <see cref="CurrentHealth"/>.
        /// </summary>
        /// <param name="amount"></param>
        public void AddHealth(float amount)
        {
            amount = amount.ToPositive();
            float prevHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
            float added = CurrentHealth - prevHealth;
            if (added > 0)
            {
                PreviousHeal = added;
                onHlthAdd?.Invoke(this, added);
                OnAnyHealed?.Invoke(this);
            }
        }

        /// <summary>
        /// Removes the specified amount from <see cref="CurrentHealth"/>.
        /// </summary>
        /// <param name="amount"></param>
        public void RemoveHealth(float amount)
        {
            CurrentHealth -= amount;
            if (CurrentHealth < 0) CurrentHealth = 0;
        }

        /// <summary>
        /// To make this method work, you will need to assign a custom or premade <see cref="HealthDamage"/>.
        /// </summary>
        /// <param name="damage"></param>
        public void DamageHealth(object damage)
        {
            try
            {
                if (!IsImmune && Calculator.AcceptableValue(damage))
                {
                    Damaged.RemoveHealth(Calculator.FinalValue(damage, this, out bool triggerImmunity));
                    PreviousDamage = damage;
                    if (triggerImmunity && UsesImmunity) TriggerImmunity(ImmunityDuration);
                    if (Damaged == (IAdvancedHealth)this)
                    {
                        OnDmgHandler?.Invoke(Damaged, damage, Calculator);
                        OnAnyDamaged?.Invoke(Damaged as NyuHealth);
                    }
                    if (CurrentHealth <= 0)
                        OnDthHandler?.Invoke(this);
                }
            }
            catch
            {
                AdvancedDebug.LogFormat("Couldn't damage {0}({1})'s as either the damage given was of incorrect type," +
                    " an event generated an Exception or Caclucator was not set.", 0, NyuMain.GetType().Name, NyuMain.Tag);
            }
        }

        /// <summary>
        /// Triggers the immunity of this <see cref="Nyu"/>. (Works only if <see cref="UsesImmunity"/> is true)
        /// </summary>
        public async void TriggerImmunity(float @for)
        {
            if (UsesImmunity && !IsImmune)
            {
                ImmunityTask = Task.Run(() =>
                {
                    IsImmune = true;
                    if (!s_ImmunityEffect)
                        return;

                    ImmunityEffect.OnTrigger();
                    s_ImmunityEffect.ImmunityTime = s_ImmunityEffect.ImmunityCountdown = @for;
                    float timer = @for;

                    while (timer > 0)
                    {
                        timer -= Time.deltaTime;
                        s_ImmunityEffect.ImmunityCountdown = timer;
                        s_ImmunityEffect.WhileTrigger();
                    }

                    IsImmune = false;

                    ImmunityEffect?.OnEnd();
                });

                await ImmunityTask;
            }
        }

        [SerializeField]
        private ImmunityEffect s_ImmunityEffect;
        /// <summary>
        /// Effect which will be Invoked when Immunity is triggered.
        /// </summary>
        public IImmunityEffect<NyuHealth> ImmunityEffect
        {
            get => s_ImmunityEffect;
            set
            {
                if (s_ImmunityEffect.Equals(value))
                    return;

                s_ImmunityEffect = value is ImmunityEffect immunityFX ? immunityFX : s_ImmunityEffect;

                s_ImmunityEffect.internalParent = this;
                s_ImmunityEffect.OnAdded();
            }
        }

        /// <summary>
        /// Must be of type <see cref="IImmunityEffect{T}"/> where T is <see cref="NyuHealth"/>.
        /// </summary>
        IImmunityEffect<IAdvancedHealth> IAdvancedHealth.ImmunityEffect
        {
            get
            {
                try { return (IImmunityEffect<IAdvancedHealth>)ImmunityEffect; }
                catch { return null; }
            }
            set
            {
                try { ImmunityEffect = (IImmunityEffect<NyuHealth>)value; }
                catch { }
            }
        }

        /// <summary>
        /// The current immunity task.
        /// </summary>
        private Task ImmunityTask;

        /// <summary>
        /// Stops an immunity previously triggered with <see cref="TriggerImmunity(float)"/>.
        /// </summary>
        public void StopImmunity()
        {
            if (IsImmune)
            {
                ImmunityTask.Dispose();
                ImmunityEffect.OnEnd();
                IsImmune = false;
            }
        }
    }
}
