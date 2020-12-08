using System;
using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.AttackSystemV2
{
    /// <summary>
    /// A <see cref="Freki"/> with a reload/magazine system.
    /// </summary>
    public abstract class Fenrir : Freki, INyuAwake, INyuUpdate
    {
        /// <summary>
        /// Invoked when a reload is successfully performed.
        /// </summary>
        public event Action<Fenrir> OnReloadEnd;
        /// <summary>
        /// Invoked when a reload is started.
        /// </summary>
        public event Action<Fenrir> OnReloadStart;

        /// <summary>
        /// The current amount of "bullets" inside this attack.
        /// </summary>
        /// <returns></returns>
        public abstract int GetMagazine();
        /// <summary>
        /// Returns the maximum capacity of the magazine.
        /// </summary>
        /// <returns></returns>
        public abstract int GetMagazineCapacity();

        private float h_MagazineReloadProgress;
        /// <summary>
        /// The reload progress of the magazine.
        /// </summary>
        public float MagazineReloadProgress
        {
            get => h_MagazineReloadProgress;
            protected set => h_MagazineReloadProgress = Math.Max(0, value);
        }

        private bool h_IsReloading;
        /// <summary>
        /// Returns the reloading state of this <see cref="Fenrir"/>.
        /// </summary>
        /// <returns></returns>
        public bool IsReloading() => h_IsReloading;

        /// <summary>
        /// The speed at which this <see cref="Fenrir"/> reloads.
        /// </summary>
        /// <returns></returns>
        public abstract float GetReloadSpeed();

        /// <summary>
        /// The amount of ammo consumed when the attack is triggered.
        /// </summary>
        /// <returns></returns>
        public abstract int GetMagazineConsumption();

        /// <summary>
        /// Used by <see cref="Fenrir"/> to set the magazine to full once reloading is finished.
        /// Make sure to point to the same value <see cref="GetMagazine"/> does unless you're absolutely sure of what you're doing.
        /// </summary>
        /// <param name="to"></param>
        protected abstract void SetMagazine(int to);

        /// <summary>
        /// Returns true if the APM allows it, the attack is not reloading and magazine is higher than 0.
        /// </summary>
        /// <returns></returns>
        public override bool CanAttack()
        {
            return base.CanAttack() && !h_IsReloading && GetMagazine() > 0;
        }

        /// <summary>
        /// Consumes the magazine based on <see cref="GetMagazineConsumption"/>.
        /// </summary>
        /// <param name="reloadOnEmpty">If true and the magazine reaches 0, it will automatically start the reload.</param>
        protected void ConsumeMagazine(bool reloadOnEmpty)
        {
            SetMagazine(Math.Max(0, GetMagazine() - GetMagazineConsumption()));
            if (reloadOnEmpty && GetMagazine() == 0)
                Reload();
        }

        /// <summary>
        /// Starts the reload of this <see cref="Fenrir"/>.
        /// </summary>
        public void Reload()
        {
            if (!h_IsReloading)
            {
                MagazineReloadProgress = 1;
                h_IsReloading = true;
                OnReloadStart?.Invoke(this);
            }
        }

        /// <summary>
        /// When overriding, make sure to include "base.NyuUpdate();" as it handles the magazine's reload as well as <see cref="Freki.APMCounter"/>'s countdown.
        /// </summary>
        public virtual void NyuUpdate()
        {
            APMCounter -= Time.deltaTime;

            if (h_IsReloading)
            {
                MagazineReloadProgress -= Time.deltaTime * GetReloadSpeed();
                if(MagazineReloadProgress == 0)
                {
                    h_IsReloading = false;
                    SetMagazine(GetMagazineCapacity());
                    OnReloadEnd?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Sets the magazine to it's max capacity.
        /// </summary>
        public virtual void NyuAwake()
        {
            SetMagazine(GetMagazineCapacity());
        }
    }
}
