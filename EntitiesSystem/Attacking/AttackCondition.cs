using System;
using UnityEngine;

namespace WarWolfWorks.EntitiesSystem.Attacking
{
    public abstract class Condition : ScriptableObject
    {
        public abstract Predicate<Attack> ConditionIsMet { get; }
    }
}
