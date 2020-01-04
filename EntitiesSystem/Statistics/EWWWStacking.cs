namespace WarWolfWorks.EntitiesSystem.Statistics
{
    /// <summary>
    /// Enum value depicting <see cref="WWWStacking"/>'s stacking values. Useful when used with Editor's Enum List Displayer.
    /// </summary>
    public enum EWWWStacking
    {
        /// <summary>
        /// Stacking index: Used as base value.
        /// </summary>
        Original = 0,
        /// <summary>
        /// Stacking index: All stats tagged will be added onto the original value.
        /// </summary>
        Adder = 1,
        /// <summary>
        /// Stacking index: All stats tagged will stack on themselves to form a final value, which will then multiply the original.
        /// </summary>
        StackingOriginalMultiplier = 2,
        /// <summary>
        /// Stacking index: tagged will muliply the original value and Adder value. (Applies before StackingOriginalMultiplier)
        /// </summary>
        StackingMultiplier = 3,
        /// <summary>
        /// Stacking index: Multiplies the original immediatly without stacking.
        /// </summary>
        OriginalMultiplier = 4,
        /// <summary>
        /// Stacking index: Multiplies the original with adder values immediatly without stacking.
        /// </summary>
        Multiplier = 5,
        /// <summary>
        /// Stacking index: All stats tagged will stack on themselves to form a final value, which will then multiply the total value after all multiplications (excluding FullMultiplier)
        /// </summary>
        StackingFullMultiplier = 6,
        /// <summary>
        /// Stacking index: Absolute multiplier.
        /// </summary>
        FullMultiplier = 7
    }
}
