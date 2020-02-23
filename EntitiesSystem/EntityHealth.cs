using UnityEngine;

namespace WarWolfWorks.EntitiesSystem
{
    using System;
    using System.Collections;
    using Interfaces;
    using Statistics;
    using WarWolfWorks;
    using WarWolfWorks.Utility;

    /// <summary>
    /// Class which acts as the Entity's health.
    /// </summary>
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
        /// The current health of the <see cref="Entity"/>.
        /// </summary>
        public float CurrentHealth { get; private set; }

        [SerializeField]
        private Stat maxHealth;
        /// <summary>
        /// The maximum value at which <see cref="CurrentHealth"/> can be.
        /// </summary>
        public float MaxHealth
        {
            get => EntityMain.Stats.CalculatedValue(maxHealth);
            set => maxHealth.SetValue = value;
        }

        [SerializeField]
        private Stat defense;
        /// <summary>
        /// The <see cref="Entity"/>'s defense.
        /// </summary>
        public float Defense => EntityMain.Stats.CalculatedValue(defense);

        [SerializeField]
        private bool damageParent = true;
        /// <summary>
        /// If true and this component's <see cref="Entity"/> is <see cref="IEntityParentable"/>,
        /// it will damage the parent's Health instead.
        /// </summary>
        public bool DamageParent { get => damageParent; set => damageParent = value; }

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
            if (DestroyOnDeath)
            {
                OnDeath += DeathDestroy;
            }

            if (Calculator == null) Calculator = ScriptableObject.CreateInstance<DefaultHealthDamage>();
            if (immunityEffect != null) ImmunityEffect = Instantiate(immunityEffect);

            SetDamagedHealth(EntityMain, EntityManager.OldestOf(EntityMain, true));
            if (EntityMain is IEntityParentable) ((IEntityParentable)EntityMain).OnParentSet += SetDamagedHealth; 
        }

        private void SetDamagedHealth(Entity child, Entity parent)
        {
            if (DamageParent && parent != null) Damaged = parent.GEC<IAdvancedHealth>();
            if (Damaged == null) Damaged = this;
        }

        private void DeathDestroy(IHealth h)
        {
            EntityManager.Destroy(EntityMain);
        }

        [SerializeField]
        private Stat immnunityDuration;
        /// <summary>
        /// Duration in which the <see cref="Entity"/> is unable to take damage. (Only works if <see cref="UsesImmunity"/> is true)
        /// </summary>
        public float ImmunityDuration => EntityMain.Stats.CalculatedValue(immnunityDuration);

        [SerializeField]
        private bool usesImmunity;
        /// <summary>
        /// Determines if the <see cref="Entity"/> can use the immunity system.
        /// </summary>
        public bool UsesImmunity => usesImmunity;

        [SerializeField]
        private bool DestroyOnDeath;

        /// <summary>
        /// Is the <see cref="Entity"/> currently immune?
        /// </summary>
        public bool IsImmune { get; set; }

        [SerializeField]
        private HealthDamage calculator;
        /// <summary>
        /// The calculator used by this <see cref="Entity"/>. If null on Awake, it will be set to <see cref="DefaultHealthDamage"/> by default.
        /// </summary>
        public IHealthDamage Calculator
        {
            get => calculator;
            set => calculator = value is HealthDamage ? (HealthDamage)value : calculator;
        }

        /// <summary>
        /// Adds the specified amount to <see cref="CurrentHealth"/>.
        /// </summary>
        /// <param name="amount"></param>
        public void AddHealth(float amount)
        {
            float added = (Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth) - MaxHealth).ToPositive();
            if (added > 0)
            {
                CurrentHealth += added;
                onHlthAdd?.Invoke(this, added);
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
                    if(Damaged == (IAdvancedHealth)this) OnDmgHandler?.Invoke(this, damage, Calculator);
                    if(CurrentHealth <= 0)
                        OnDthHandler?.Invoke(this);
                }
            }
            catch
            {
                AdvancedDebug.LogFormat("Couldn't damage {0} as either the damage given was of incorrect type or Caclucator was not set.", 0, EntityMain.Name);
            }
        }

        /// <summary>
        /// Triggers the immunity of this <see cref="Entity"/>. (Works only if <see cref="UsesImmunity"/> is true)
        /// </summary>
        public void TriggerImmunity(float @for)
        {
            if(UsesImmunity && !IsImmune)StartCoroutine(IImmunity(@for));
        }

        [SerializeField]
        private ImmunityEffect immunityEffect;
        /// <summary>
        /// Effect which will be Invoked when Immunity is triggered.
        /// </summary>
        public IImmunityEffect<EntityHealth> ImmunityEffect
        {
            get => immunityEffect;
            set
            {
                if (immunityEffect.Equals(value))
                    return;

                immunityEffect = value is ImmunityEffect ? (ImmunityEffect)value : immunityEffect;

                immunityEffect.internalParent = this;
                immunityEffect.OnAdded();
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

        private IEnumerator IImmunity(float @for)
        {
            IsImmune = true;

            if(!immunityEffect)
            {
                StopCoroutine(IImmunity(@for));
                yield break;
            }

            ImmunityEffect.OnTrigger();

            immunityEffect.ImmunityTime = immunityEffect.ImmunityCountdown = @for;
            float timer = @for;

            while(timer > 0)
            {
                timer -= Time.deltaTime;
                immunityEffect.ImmunityCountdown = timer;
                immunityEffect.WhileTrigger();
                yield return null;
            }

            StopImmunityInternal(@for);
        }

        private void StopImmunityInternal(float @for)
        {
            IsImmune = false;

            ImmunityEffect?.OnEnd();
            StopCoroutine(IImmunity(@for));
        }

        /// <summary>
        /// Stops an immunity previously triggered with <see cref="TriggerImmunity(float)"/>.
        /// </summary>
        public void StopImmunity()
        {
            if (IsImmune) StopImmunityInternal(ImmunityDuration);
        }
    }
}