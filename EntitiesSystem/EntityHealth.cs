using UnityEngine;
using System;
using System.Threading.Tasks;
using WarWolfWorks.Interfaces;
using WarWolfWorks.EntitiesSystem.Statistics;
using UnityEngine.Serialization;
using WarWolfWorks.Attributes;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EntitiesSystem
{

    /// <summary>
    /// Class which acts as the Entity's health.
    /// </summary>
    [CompleteNoS]
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public sealed class EntityHealth : EntityComponent, IAdvancedHealth
    {
        private Action<IAdvancedHealth, object, IHealthDamage> OnDmgHandler;
        /// <summary>
        /// What happends when <see cref="DamageHealth(object)"/> is successfully invoked.
        /// </summary>
        public event Action<IAdvancedHealth, object, IHealthDamage> OnDamaged
        {
            add
            {
                AdvancedDebug.LogFormat("{0} has been added to {1}'s EntityHealth component!", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, EntityMain.Name);
                OnDmgHandler += value;
            }

            remove
            {
                AdvancedDebug.LogFormat("{0} has been removed from {1}'s EntityHealth component.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, EntityMain.Name);
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
                AdvancedDebug.LogFormat("{0} will now trigger when {1}'s health reaches 0.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, EntityMain.Name);
                OnDthHandler += value;
            }
            remove
            {
                AdvancedDebug.LogFormat("{0} will no longer trigger when {1}'s health reaches 0.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, EntityMain.Name);
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

                AdvancedDebug.LogFormat("{0} will now trigger when {1}'s IHealth.AddHealth is called.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, EntityMain.Name);
                onHlthAdd += value;
            }
            remove
            {
                AdvancedDebug.LogFormat("{0} will no longer trigger when {1}'s IHealth.AddHealth is called.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, value.Method.Name, EntityMain.Name);
                onHlthAdd -= value;
            }
        }

        /// <summary>
        /// Invoked when any <see cref="EntityHealth"/> gets successfully damaged.
        /// </summary>
        public static event Action<EntityHealth> OnAnyDamaged;
        /// <summary>
        /// Invoked when any <see cref="EntityHealth"/> invokes <see cref="AddHealth(float)"/> successfully.
        /// </summary>
        public static event Action<EntityHealth> OnAnyHealed;

        /// <summary>
        /// The current health of the <see cref="Entity"/>.
        /// </summary>
        public float CurrentHealth { get; private set; }

        [SerializeField, FormerlySerializedAs("maxHealth")]
        private Stat s_MaxHealth;
        /// <summary>
        /// The maximum value at which <see cref="CurrentHealth"/> can be.
        /// </summary>
        public float MaxHealth
        {
            get => EntityMain.Stats.CalculatedValue(s_MaxHealth);
            set => s_MaxHealth.SetValue = value;
        }

        [SerializeField ,FormerlySerializedAs("damageParent")]
        private bool s_DamageParent = true;
        /// <summary>
        /// If true and this component's <see cref="Entity"/> is <see cref="IEntityParentable"/>,
        /// it will damage the parent's Health instead.
        /// </summary>
        public bool DamageParent { get => s_DamageParent; set => s_DamageParent = value; }

        private IAdvancedHealth Damaged { get; set; }

        /// <summary>
        /// Damage previously passed in <see cref="DamageHealth(object)"/>. (Only accepted)
        /// </summary>
        public object PreviousDamage { get; private set; }

        /// <summary>
        /// Unity's Awake method called by <see cref="EntityComponent"/>.
        /// </summary>
        public override void OnAwake()
        {
            CurrentHealth = MaxHealth;
            if (s_DestroyOnDeath)
            {
                OnDeath += Event_DeathDestroy;
            }

            if (Calculator == null) Calculator = ScriptableObject.CreateInstance<DefaultHealthDamage>();
            else Calculator = Instantiate(s_Calculator);
            if (s_ImmunityEffect != null) ImmunityEffect = Instantiate(s_ImmunityEffect);

            Event_SetDamagedHealth(EntityMain, EntityManager.OldestOf(EntityMain, true));
            if (EntityMain is IEntityParentable) ((IEntityParentable)EntityMain).OnParentSet += Event_SetDamagedHealth; 
        }

        private void Event_SetDamagedHealth(Entity child, Entity parent)
        {
            if (DamageParent && parent != null) Damaged = parent.GEC<IAdvancedHealth>();
            if (Damaged == null) Damaged = this;
        }

        private void Event_DeathDestroy(IHealth h)
        {
            EntityManager.Destroy(EntityMain);
        }

        [SerializeField, FormerlySerializedAs("immnunityDuration")]
        private Stat s_ImmnunityDuration;
        /// <summary>
        /// Duration in which the <see cref="Entity"/> is unable to take damage. (Only works if <see cref="UsesImmunity"/> is true)
        /// </summary>
        public float ImmunityDuration => EntityMain.Stats.CalculatedValue(s_ImmnunityDuration);

        [SerializeField, FormerlySerializedAs("usesImmunity")]
        private bool s_UsesImmunity;
        /// <summary>
        /// Determines if the <see cref="Entity"/> can use the immunity system.
        /// </summary>
        public bool UsesImmunity => s_UsesImmunity;

        [SerializeField, FormerlySerializedAs("DestroyOnDeath")]
        private bool s_DestroyOnDeath;

        /// <summary>
        /// Is the <see cref="Entity"/> currently immune?
        /// </summary>
        public bool IsImmune { get; set; }

        [SerializeField, FormerlySerializedAs("calculator")]
        private HealthDamage s_Calculator;
        /// <summary>
        /// The calculator used by this <see cref="Entity"/>. If null on Awake, it will be set to <see cref="DefaultHealthDamage"/> by default.
        /// </summary>
        public IHealthDamage Calculator
        {
            get => s_Calculator;
            set => s_Calculator = value is HealthDamage ? (HealthDamage)value : s_Calculator;
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
                        OnAnyDamaged?.Invoke(Damaged as EntityHealth);
                    }
                    if(CurrentHealth <= 0)
                        OnDthHandler?.Invoke(this);
                }
            }
            catch
            {
                AdvancedDebug.LogFormat("Couldn't damage {0} as either the damage given was of incorrect type," +
                    " an event generated an Exception or Caclucator was not set.", 0, EntityMain.Name);
            }
        }

        /// <summary>
        /// Triggers the immunity of this <see cref="Entity"/>. (Works only if <see cref="UsesImmunity"/> is true)
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

        [SerializeField, FormerlySerializedAs("immunityEffect")]
        private ImmunityEffect s_ImmunityEffect;
        /// <summary>
        /// Effect which will be Invoked when Immunity is triggered.
        /// </summary>
        public IImmunityEffect<EntityHealth> ImmunityEffect
        {
            get => s_ImmunityEffect;
            set
            {
                if (s_ImmunityEffect.Equals(value))
                    return;

                s_ImmunityEffect = value is ImmunityEffect ? (ImmunityEffect)value : s_ImmunityEffect;

                s_ImmunityEffect.internalParent = this;
                s_ImmunityEffect.OnAdded();
            }
        }

        /// <summary>
        /// Must be of type <see cref="IImmunityEffect{T}"/> where T is <see cref="EntityHealth"/>.
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
                try { ImmunityEffect = (IImmunityEffect<EntityHealth>)value; }
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