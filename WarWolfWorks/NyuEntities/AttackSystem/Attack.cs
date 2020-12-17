using System;
using System.Collections;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.NyuEntities.Statistics;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.NyuEntities.AttackSystem
{
    /// <summary>
    /// To be used with <see cref="NyuAttack"/>; Handles attacking with various utilities.
    /// (Supported interfaces: <see cref="INyuAwake"/>, <see cref="INyuOnEnable"/>, <see cref="INyuOnDisable"/>,
    /// <see cref="INyuUpdate"/>, <see cref="INyuFixedUpdate"/>, <see cref="INyuLateUpdate"/>,
    /// and <see cref="INyuOnDestroy"/>)
    /// </summary>
    [CompleteNoS]
    public abstract class Attack : ScriptableObject, INyuReferencable, IParentable<NyuAttack>, INyuUpdate
    {
        /// <summary>
        /// The <see cref="Nyu"/> holder of this attack.
        /// </summary>
        public Nyu NyuMain { get; private set; }
        /// <summary>
        /// The <see cref="Parent"/> component of the holder.
        /// </summary>
        public NyuAttack Parent { get; private set; }
        /// <summary>
        /// Invoked when the Attack successfully invokes <see cref="OnTrigger"/>
        /// </summary>
        public event Action<Attack> OnAttack;
        /// <summary>
        /// Invokes when <see cref="Reload()"/> is called.
        /// </summary>
        public event Action<Attack> OnReloadTrigger;
        /// <summary>
        /// Invokes after <see cref="Reload()"/> finishes.
        /// </summary>
        public event Action<Attack> OnReload;

        /// <summary>
        /// Determines if the attack was assigned to an entity.
        /// </summary>
        public bool IsInitiated { get; internal set; }

        [SerializeField]
        private string s_Name;
        /// <summary>
        /// Name given to this attack through the inspector.
        /// </summary>
        public string Name => s_Name;

        [SerializeField, TextArea]
        private string s_AttackDescription;
        /// <summary>
        /// The description of the Attack, serialized as "Attack Description" in the inspector.
        /// </summary>
        public string Description => s_AttackDescription;

        /// <summary>
        /// Uncalculated damage. (overridable through <see cref="Damage"/>)
        /// </summary>
        [SerializeField]
        protected Stat s_Damage;
        /// <summary>
        /// The calculated damage of the attack. (overridable)
        /// </summary>
        public virtual float Damage => NyuMain.Stats.CalculatedValue(s_Damage);

        /// <summary>
        /// Uncalculated attack speed.
        /// </summary>
        [SerializeField]
        protected Stat s_AttackSpeed;
        /// <summary>
        /// Rate at which Attack.Trigger() can be called. (Calculated as 60 / AttackSpeed for an RPM-like functionality).
        /// </summary>
        public virtual float AttackSpeed => NyuMain.Stats.CalculatedValue(s_AttackSpeed);

        /// <summary>
        /// Uncalculated magazine size.
        /// </summary>
        [SerializeField]
        protected Stat s_MagazineSize;
        /// <summary>
        /// Calculated maximum capacity of the attack's magazine.
        /// </summary>
        public virtual int MagazineSize => (int)NyuMain.Stats.CalculatedValue(s_MagazineSize);

        [SerializeField]
        private bool s_InfiniteAmmo;
        /// <summary>
        /// If true, MagazineSize and ReloadSpeed will be ignored and all shots will not consume any ammunition.
        /// </summary>
        public bool InfiniteAmmo => s_InfiniteAmmo;
        /// <summary>
        /// Sets <see cref="InfiniteAmmo"/> to given value.
        /// </summary>
        /// <param name="to"></param>
        public void SetInfiniteAmmo(bool to)
        {
            s_InfiniteAmmo = to;
        }

        /// <summary>
        /// Used by <see cref="CurrentMagazine"/>.
        /// </summary>
        protected int ns_currentMagazine;
        /// <summary>
        /// How many "bullets" are currently in the magazine.
        /// </summary>
        public virtual int CurrentMagazine { get => ns_currentMagazine; protected set => ns_currentMagazine = value; }

        /// <summary>
        /// The ammo consumption of the attack per <see cref="Trigger"/>. (1 by default)
        /// </summary>
        protected virtual int AmmoConsumption { get; } = 1;

        /// <summary>
        /// Uncalculated reload speed.
        /// </summary>
        [SerializeField]
        protected Stat s_ReloadSpeed;
        /// <summary>
        /// Calculated speed at which <see cref="Reload()"/> is invoked.
        /// The lower this value is the faster it will reload.
        /// </summary>
        public virtual float ReloadSpeed => Mathf.Clamp(NyuMain.Stats.CalculatedValue(s_ReloadSpeed), 0, float.MaxValue);
        [SerializeField]
        private Condition s_DefaultAttackCondition;
        /// <summary>
        /// The default attack condition which is set when an attack group inside <see cref="Parent"/> is not set.
        /// </summary>
        public Condition DefaultAttackCondition => s_DefaultAttackCondition;

        internal void Initiate(Nyu nyu)
        {
            if (IsInitiated)
                return;

            AdvancedDebug.LogFormat("Attack {0} has been added to ({1})!", DEBUG_LAYER_WWW_INDEX, Name, nyu.GetType().Name, nyu.Tag);
            NyuMain = nyu;
            Parent = nyu.GetNyuComponent<NyuAttack>();

            ns_currentMagazine = MagazineSize;
            CancelReload(true);

            IsInitiated = true;

            if (this is INyuAwake thisAwake)
                thisAwake.NyuAwake();
        }

        private float ns_AttackSpeedCounter;
        /// <summary>
        /// Returns true if the interval between attack based on attack speed was met.
        /// </summary>
        public bool AttackSpeedAllowsTrigger => ns_AttackSpeedCounter >= (60 / AttackSpeed);

        /// <summary>
        /// When overriding, make sure you include "base.Update();" as it takes care of the attack speed.
        /// </summary>
        public virtual void NyuUpdate()
        {
            ns_AttackSpeedCounter += Time.deltaTime * TimeScale;
        }

        /// <summary>
        /// Returns true if either: InfiniteAmmo is true, CurrentMagazine is higher than 0 or AmmoConsumption is less than 1.
        /// </summary>
        public bool HasAmmo => InfiniteAmmo || CurrentMagazine > 0 || AmmoConsumption < 1;

        /// <summary>
        /// Returns true if AttackSpeedAllowsTrigger is true, Has ammo is true and IsReloading is false.
        /// Keep in mind this does not take into account <see cref="NyuAttack.GetCanAttack(int)"/>.
        /// </summary>
        public bool CanAttack => AttackSpeedAllowsTrigger && HasAmmo && !IsReloading && TimeScale > 0;

        internal bool Trigger()
        {
            if (!CanAttack)
                return false;

            ForceTrigger(true);

            return true;
        }

        private float timeScale = 1;
        /// <summary>
        /// Multiplier at which <see cref="AttackSpeed"/> is calculated. Works in a similar way to <see cref="Time.timeScale"/>.
        /// </summary>
        public float TimeScale { get => timeScale; internal set => timeScale = Mathf.Clamp(value, 0, float.MaxValue); }

        /// <summary>
        /// Allows you to trigger <see cref="OnTrigger"/> without meeting any required condition.
        /// </summary>
        public void ForceTrigger(bool consumeAmmo)
        {
            if (consumeAmmo) ConsumeAmmo();
            OnTrigger();
            OnAttack?.Invoke(this);
            ns_AttackSpeedCounter = 0;
        }

        /// <summary>
        /// Activates <see cref="ForceTrigger(bool)"/> repeatedly with a Unity Coroutine for a specified duration in seconds.
        /// </summary>
        public void ForceTriggerOnRepeat(bool consumeAmmo, float duration, float delayBetweenShots)
            => NyuMain.StartCoroutine(IForceTrigger(consumeAmmo, duration, delayBetweenShots));
        /// <summary>
        /// Activates <see cref="ForceTrigger(bool)"/> repeatedly with a Unity Coroutine for a specified duration in triggers invoked.
        /// </summary>
        /// <param name="consumeAmmo"></param>
        /// <param name="shotsLength"></param>
        /// <param name="delayBetweenShots"></param>
        public void ForceTriggerOnRepeat(bool consumeAmmo, int shotsLength, float delayBetweenShots)
            => NyuMain.StartCoroutine(IForceTrigger(consumeAmmo, shotsLength, delayBetweenShots));

        /// <summary>
        /// Activates <see cref="ForceTrigger(bool)"/> repeatedly with a Unity Coroutine as long as @while returns true.
        /// </summary>
        /// <param name="consumeAmmo"></param> 
        /// <param name="while"></param>
        /// <param name="delayBetweenShots"></param>
        public void ForceTriggerOnRepeat(bool consumeAmmo, Predicate<Attack> @while, float delayBetweenShots)
            => NyuMain.StartCoroutine(IForceTrigger(consumeAmmo, @while, delayBetweenShots));

        /// <summary>
        /// Here in case an Attack is permanently frozen due to bad exception handling.
        /// </summary>
        [ContextMenu("Troubleshoot")]
        public void TroubleShootTrigger()
        {
            NyuMain.StopCoroutine("IForceTrigger");
            IsInitiated = false;
        }

        private IEnumerator IForceTrigger(bool consumeAmmo, float duration, float delayBetweenShots)
        {
            float counter = 0;
            float shootDelayer = 0; //Creates the delay between shots. 
            while (counter < duration)
            {
                counter += Time.deltaTime;
                shootDelayer -= Time.deltaTime;
                if (shootDelayer <= 0)
                {
                    ForceTrigger(consumeAmmo);
                    shootDelayer = delayBetweenShots;
                }

                yield return null;
            }

            NyuMain.StopCoroutine(IForceTrigger(consumeAmmo, duration, delayBetweenShots));
        }

        private IEnumerator IForceTrigger(bool consumeAmmo, Predicate<Attack> @while, float delayBetweenShots)
        {
            while (true)
            {
                if (!@while.Invoke(this))
                    break;

                ForceTrigger(consumeAmmo);
                yield return new WaitForSeconds(delayBetweenShots);
            }

            NyuMain.StopCoroutine(IForceTrigger(consumeAmmo, @while, delayBetweenShots));
        }

        private IEnumerator IForceTrigger(bool consumeAmmo, int shotsLength, float delayBetweenShots)
        {
            int shots = 0;
            while (shots < shotsLength)
            {
                ForceTrigger(consumeAmmo);
                shots++;
                yield return new WaitForSeconds(delayBetweenShots);
            }

            NyuMain.StopCoroutine(IForceTrigger(consumeAmmo, shotsLength, delayBetweenShots));
        }

        /// <summary>
        /// When called, it will remove AmmoConsumption from CurrentMagazine, and if CurrentMagazine is less than or equal to 0, it will automatically invoke <see cref="Reload()"/>.
        /// </summary>
        protected void ConsumeAmmo()
        {
            if (IsReloading || InfiniteAmmo)
                return;

            CurrentMagazine -= AmmoConsumption;
            if (CurrentMagazine <= 0)
                Reload();
        }

        /// <summary>
        /// Starts a reload.
        /// </summary>
        public void Reload() => NyuMain.StartCoroutine(IReload(0), ref isReloading);

        /// <summary>
        /// Starts a reload from specified point (0 being beginning, 1 being end).
        /// </summary>
        /// <param name="from01"></param>
        public void Reload(float from01) => NyuMain.StartCoroutine(IReload(from01), ref isReloading);

        /// <summary>
        /// Goes through <see cref="Reload()"/> with a delay equal to 0 instead of <see cref="ReloadSpeed"/>.
        /// </summary>
        public void InstantReload()
        {
            OnReloadTrigger?.Invoke(this);
            ReloadProcess();
            NyuMain.StopCoroutine(IReload(GetReloadProgress()), ref isReloading);
        }

        /// <summary>
        /// Returns the current progress of the reload in percent (01). Used by <see cref="Reload()"/>.
        /// You should override this if you have already overridden <see cref="SetReloadProgress(float)"/>,
        /// and it should point to the exact same value, except if you're absolutely sure of what you're doing.
        /// Pointing to a different variable than <see cref="SetReloadProgress(float)"/> is using will
        /// always cause an infinite reload to happen.
        /// </summary>
        /// <returns></returns>
        public virtual float GetReloadProgress() => reloadProgress;
        private float reloadProgress;
        /// <summary>
        /// The progress of the reload in percent (01). Used by <see cref="Reload()"/>.
        /// You should override this only to change what value this is applied to, otherwise
        /// it can generate unwanted behaviour. If you override this method, make sure to override
        /// <see cref="GetReloadProgress"/> to point to the exact same value.
        /// </summary>
        public virtual void SetReloadProgress(float to) => reloadProgress = to;

        /// <summary>
        /// Determines if the <see cref="Reload()"/>'s invokation was paused.
        /// </summary>
        public bool ReloadIsPaused { get; private set; }
        /// <summary>
        /// Determines if the weapon is currently reloading.
        /// </summary>
        public bool IsReloading => isReloading;

        private bool isReloading = false;
        private IEnumerator IReload(float from01)
        {
            OnReloadTrigger?.Invoke(this);

            if (InfiniteAmmo)
                goto ReloadPart;

            SetReloadProgress(from01);
            while (true)
            {
                if (!IsReloading)
                {
                    NyuMain.StopCoroutine(IReload(GetReloadProgress()));
                    break;
                }

                if (!ReloadIsPaused)
                {
                    SetReloadProgress(GetReloadProgress() + ((1 / ReloadSpeed) * Time.deltaTime));
                    if (GetReloadProgress() >= 1)
                        goto ReloadPart;
                }
                yield return null;
            }

            ReloadPart:
            InstantReload();
        }

        /// <summary>
        /// If <see cref="Reload()"/> was called, it's invokation will be canceled.
        /// </summary>
        /// <param name="resetProgress"/>
        public void CancelReload(bool resetProgress)
        {
            NyuMain.StopCoroutine(IReload(GetReloadProgress()));
            isReloading = false;
            if (resetProgress) SetReloadProgress(0);
        }

        /// <summary>
        /// Pauses the invokation of <see cref="Reload()"/> without completely stopping it. To resume, call <see cref="ResumeReload"/>.
        /// </summary>
        public void PauseReload() => ReloadIsPaused = true;

        /// <summary>
        /// Resumes a paused invokation called by <see cref="PauseReload"/>.
        /// </summary>
        public void ResumeReload() => ReloadIsPaused = false;

        private void ReloadProcess()
        {
            CurrentMagazine = MagazineSize;
            OnReload?.Invoke(this);
            SetReloadProgress(0);
        }

        /// <summary>
        /// This method is called when all conditions are met and the Attack successfully attacks.
        /// </summary>
        protected abstract void OnTrigger();
    }
}
