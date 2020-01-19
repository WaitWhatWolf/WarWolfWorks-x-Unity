﻿using System;
using UnityEngine;

namespace WarWolfWorks.EntitiesSystem.Attacking
{
    /// <summary>
    /// Condition used for <see cref="EntityAttack"/>.
    /// </summary>
    public abstract class Condition : ScriptableObject
    {
        /// <summary>
        /// Predicate which determines if the <see cref="Attack"/> assigned with this <see cref="Condition"/> inside <see cref="EntityAttack"/> will be able to shoot.
        /// </summary>
        public abstract Predicate<Attack> ConditionIsMet { get; }
    }
}
