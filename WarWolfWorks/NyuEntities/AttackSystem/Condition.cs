using UnityEngine;

namespace WarWolfWorks.NyuEntities.AttackSystem
{
    /// <summary>
    /// Condition used for <see cref="Attack"/>.
    /// </summary>
    public abstract class Condition : ScriptableObject
    {
        /// <summary>
        /// Returns true if the condition is met.
        /// </summary>
        /// <param name="attack"></param>
        /// <returns></returns>
        public abstract bool Met(Attack attack);
    }
}
