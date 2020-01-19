using System;
using UnityEngine;

namespace WarWolfWorks.EditorBase.PropertyDrawers
{
    public class IntToEnumFlagsAttribute : PropertyAttribute
    {
        public Type EnumUse;

        public IntToEnumFlagsAttribute(Type enumerator)
        {
            EnumUse = enumerator;
        }
    }
}
