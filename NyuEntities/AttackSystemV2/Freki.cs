using System;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.AttackSystemV2
{
    /// <summary>
    /// The base class to attack with.
    /// Supported interfaces: 
    /// <see cref="INyuAwake"/>, <see cref="INyuStart"/>, 
    /// <see cref="INyuUpdate"/>, <see cref="INyuFixedUpdate"/>,
    /// <see cref="INyuLateUpdate"/>, <see cref="INyuOnDestroy"/>.
    /// </summary>
    public abstract class Freki : ScriptableObject, IParentable<Odin>, IIndexable, INyuReferencable
    {
        /// <summary>
        /// Invoked when a successful (and properly activated) <see cref="OnTrigger"/> was called.
        /// </summary>
        public event Action<Freki> OnTriggerSuccess;

        /// <summary>
        /// The attacks per minute (APM) of this attack; The higher the value the faster the attack will be.
        /// </summary>
        /// <returns></returns>
        public abstract float GetAPM();

        private float h_APMCounter;
        /// <summary>
        /// The counter which counts down the timer between attack of the RPM; It is considered as finished when it's value returns 0.
        /// </summary>
        public float APMCounter { get => h_APMCounter; set => h_APMCounter = Math.Max(0, value); }

        /// <summary>
        /// The index of this attack in it's <see cref="NyuOdinHandler"/>.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// The parent which handles this attack.
        /// </summary>
        public Odin Parent { get; internal set; }

        /// <summary>
        /// Pointer to the parent's NyuMain.
        /// </summary>
        public Nyu NyuMain => Parent.Parent.NyuMain;

        /// <summary>
        /// The <see cref="NyuOdinHandler"/> which manages this attack.
        /// </summary>
        public NyuOdinHandler Handler { get; internal set; }

        /// <summary>
        /// Returns the point at the same index as this <see cref="Freki"/> from it's parent.
        /// </summary>
        /// <returns></returns>
        protected Transform GetPoint() => Parent.Parent.Points[Index];

        /// <summary>
        /// Resets the <see cref="APMCounter"/> to restart it's countdown (make it count down again).
        /// </summary>
        public void RestartAPMCounter()
        {
            APMCounter = 60 / GetAPM();
        }

        /// <summary>
        /// Returns true when all conditions for this attack are met.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanAttack()
        {
            return APMCounter == 0;
        }

        /// <summary>
        /// Invoked when CanAttack returns true and the condition attached to the same Odin group as this attack returns true.
        /// </summary>
        protected abstract void OnTrigger();

        internal void CallTrigger()
        {
            OnTrigger();
            OnTriggerSuccess?.Invoke(this);
        }
    }
}
