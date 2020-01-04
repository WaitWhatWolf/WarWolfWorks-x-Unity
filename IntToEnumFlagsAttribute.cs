using System;
using UnityEngine;

namespace WarWolfWorks.Utility.Editor
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
