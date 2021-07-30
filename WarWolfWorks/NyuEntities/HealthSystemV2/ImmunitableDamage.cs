using System;
using System.Collections.Generic;
using System.Text;

namespace WarWolfWorks.NyuEntities.HealthSystemV2
{
    /// <summary>
    /// A <see cref="Damage"/> class which is able to trigger an immunity state on <see cref="NyuImmunitableHealth"/>.
    /// </summary>
    public abstract record ImmunitableDamage : Damage
    {
        /// <summary>
        /// If true, it will trigger immunity on <see cref="NyuImmunitableHealth"/>.
        /// </summary>
        public bool TriggersImmunity;

        /// <summary>
        /// Uses base constructor and sets <see cref="TriggersImmunity"/>.
        /// </summary>
        /// <param name="nyuMain"></param>
        /// <param name="triggersImmunity"></param>
        public ImmunitableDamage(Nyu nyuMain, bool triggersImmunity) : base(nyuMain)
        {
            TriggersImmunity = triggersImmunity;
        }
    }
}
