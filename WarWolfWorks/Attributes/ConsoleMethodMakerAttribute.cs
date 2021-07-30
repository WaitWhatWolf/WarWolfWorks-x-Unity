using System;
using System.Collections.Generic;
using System.Text;

namespace WarWolfWorks.Attributes
{
    [Obsolete, AttributeUsage(AttributeTargets.Method)]
    internal sealed class ConsoleMethodMakerAttribute : Attribute
    {
        public ConsoleMethodMakerType Type;
        public string Name;
        public string Description;
        public string ArgumentHelper;

        public ConsoleMethodMakerAttribute(ConsoleMethodMakerType type, string name, string description, string argumentHelper)
        {
            Type = type;
            Name = name;
            Description = description;
            ArgumentHelper = argumentHelper;
        }

        public ConsoleMethodMakerAttribute(ConsoleMethodMakerType type, string description, string argumentHelper)
        {
            Type = type;
            Description = description;
            ArgumentHelper = argumentHelper;
        }
    }
}
