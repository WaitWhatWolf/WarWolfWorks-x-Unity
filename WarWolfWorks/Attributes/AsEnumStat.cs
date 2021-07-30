using System;
using WarWolfWorks.Enums;
using WarWolfWorks.NyuEntities.Statistics;

namespace WarWolfWorks.Attributes
{
    /// <summary>
    /// Use this attribute to draw a stat's stacking and affection with enum fields instead of int fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AsEnumStatAttribute : Attribute
    {
        /// <summary>
        /// What <see cref="Enum"/> type the stacking is assigned to.
        /// </summary>
        public Type StackingType;
        /// <summary>
        /// What <see cref="Enum"/> type the affection is assigned to.
        /// </summary>
        public Type AffectionType;

        /// <summary>
        /// Creates an <see cref="AsEnumStatAttribute"/> which has a custom <see cref="AffectionType"/> and <see cref="StackingType"/> set to <see cref="EWWWStacking"/>.
        /// </summary>
        public AsEnumStatAttribute(Type affectionType)
        {
            StackingType = typeof(EDefaultNyuStacking);
            AffectionType = affectionType;
        }
        
        /// <summary>
        /// Creates an <see cref="AsEnumStatAttribute"/> which has a custom <see cref="AffectionType"/> and <see cref="StackingType"/>.
        /// </summary>
        public AsEnumStatAttribute(Type stackingType, Type affectionType)
        {
            StackingType = stackingType;
            AffectionType = affectionType;
        }
    }
}
