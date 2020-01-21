using System;
using System.Collections;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.CollisionSystem
{
    /// <summary>
    /// Base class for all scripts inside WWW.CollisionSystem.
    /// </summary>
    public abstract class Collidable : MonoBehaviour
    {
        /// <summary>
        /// Determines if the class is usable or not, based on if the required conditions are met.
        /// </summary>
        public bool IsUsable => コンディション.Invoke(this);

        /// <summary>
        /// Condition to be met for Collidable class functions to work.
        /// </summary>
        protected abstract Predicate<Collidable> コンディション { get; }

        /// <summary>
        /// Unity's awake method.
        /// </summary>
        protected virtual void Awake()
        {
            this.StartCoroutine(コンディションチエッカ(), ref チエッカは遊ぶ);
        }

        private bool チエッカは遊ぶ;
        private IEnumerator コンディションチエッカ()
        {
            while(!IsUsable)
            {
                yield return new WaitForSecondsRealtime(1);
                AdvancedDebug.LogWarning($"{gameObject.name}'s {name} cannot function as it's conditions were not met!", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);
            }

            this.StopCoroutine(コンディションチエッカ(), ref チエッカは遊ぶ);
        }
    }
}
