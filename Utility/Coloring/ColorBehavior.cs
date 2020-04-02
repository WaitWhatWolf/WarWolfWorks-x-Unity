namespace WarWolfWorks.Utility.Coloring
{
    /// <summary>
    /// Determines how a <see cref="CMColor"/> should behave inside a <see cref="ColorManager"/>.
    /// </summary>
    public enum ColorBehavior : short
    {
        /// <summary>
        /// When <see cref="CMColor.MaxDuration"/> reaches 0 or less, it will be removed from it's <see cref="ColorManager"/> host.
        /// </summary>
        RemoveOnDurationEnd = 42,
        /// <summary>
        /// <see cref="CMColor.MaxDuration"/> will still be counted, however it will simply stop at 0 and not remove the color.
        /// </summary>
        StayAfterDurationEnd = 43,
        /// <summary>
        /// <see cref="CMColor.MaxDuration"/> will not be counted.
        /// </summary>
        NoDurationCountdown = 44,
    }
}
