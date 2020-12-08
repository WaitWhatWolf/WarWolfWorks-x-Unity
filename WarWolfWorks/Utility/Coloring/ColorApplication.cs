namespace WarWolfWorks.Utility.Coloring
{
    /// <summary>
    /// How the color is applied for a final result inside a <see cref="ColorManager"/>.
    /// </summary>
    public enum ColorApplication : short
    {
        /// <summary>
        /// The <see cref="CMColor"/> will be ignored completely.
        /// </summary>
        ignore,
        /// <summary>
        /// Adds the <see cref="CMColor"/>'s raw color value on the final result.
        /// </summary>
        FlatAdd,
        /// <summary>
        /// Removes the <see cref="CMColor"/>'s raw color value from the final result.
        /// </summary>
        FlatRemove,
        /// <summary>
        /// Adds the <see cref="CMColor"/>'s color value divided by the amount of colors applied on the final result.
        /// </summary>
        AverageAdd,
        /// <summary>
        /// Adds the <see cref="CMColor"/>'s color value divided by the amount of colors applied on the final result.
        /// </summary>
        AverageRemove,
        /// <summary>
        /// Adds the <see cref="CMColor"/>'s color value on the final result with a force equal to the duration left.
        /// </summary>
        AscendingAdd,
        /// <summary>
        /// Removes the <see cref="CMColor"/>'s color value from the final result with a force equal to the duration left.
        /// </summary>
        AscendingRemove,
        /// <summary>
        /// Adds the <see cref="CMColor"/>'s color value divided by the amount of colors applied on the final result with a force equal to the duration left.
        /// </summary>
        AscendingAverageAdd,
        /// <summary>
        /// Removes the <see cref="CMColor"/>'s color value divided by the amount of colors applied from the final result with a force equal to the duration left.
        /// </summary>
        AscendingAverageRemove
    }
}
