using UnityEngine;

namespace WarWolfWorks.EntitiesSystem
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using Interfaces;
    using Statistics;
    using WarWolfWorks;

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
                AdvancedDebug.LogFormat("{0} has been added to {1}'s EntityHealth component!", 1, value.Method.Name, EntityMain.Name);
                OnDmgHandler += value;
            }

            remove
            {
                AdvancedDebug.LogFormat("{0} has been removed from {1}'s EntityHealth component.", 1, value.Method.Name, EntityMain.Name);
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
            EntityMain.Destroy();
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
        public void AddHealth(float amount) => CurrentHealth += amount;

        /// <summary>
        /// Removes the specified amount from <see cref="CurrentHealth"/>.
        /// </summary>
        /// <param name="amount"></param>
        public void RemoveHealth(float amount)
        {
            CurrentHealth -= amount;
        }

#if WWW2_5_OR_HIGHER
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
#else
        [System.Obsolete("This is an old version, consider using WWW2_0.")]
        public void DamageHealth((float damage, bool trueDamage, float penetration, Vector3 position) vars)
        {
            if (IsImmune)
                return;
            
            float dmgRemover = vars.trueDamage ? 0 : Defense != 0 && vars.penetration != 0 ? Defense / vars.penetration : 0;
            float actDamage = vars.damage - dmgRemover;

            OnDmgHandler?.Invoke(this);
            RemoveHealth(actDamage);
            if (CurrentHealth <= 0)
                OnDthHandler?.Invoke(this);
            TriggerImmunity();
        }
#endif

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
        public IImmunityEffect ImmunityEffect
        {
            get => immunityEffect;
            set
            {
                immunityEffect = value is ImmunityEffect ? (ImmunityEffect)value : immunityEffect;
                if(immunityEffect) immunityEffect.StopwatchDuration = ImmunityCounter;
            }
        }

        private Stopwatch ImmunityCounter = new Stopwatch();
        private IEnumerator IImmunity(float @for)
        {
            IsImmune = true;
            ImmunityCounter.Reset();
            ImmunityCounter.Start();
            ImmunityEffect?.OnTrigger(EntityMain);
            yield return new WaitForSeconds(@for);
            StopImmunityInternal(@for);
        }

        private void StopImmunityInternal(float @for)
        {
            IsImmune = false;
            ImmunityCounter.Stop();
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