using System;

namespace WarWolfWorks.Enums
{
    /// <summary>
    /// Which sides to apply the slick border to.
	/// </summary>
    [Flags]
    public enum SlickBorderFlags
    {
        None = 0,
        Left = 4,
        Right = 8,
        Top = 16,
        Bot = 32,

        All = Left | Right | Top | Bot,
        ExceptBot = Left | Right | Top,
        ExceptTop = Left | Right | Bot,
        ExceptLeft = Right | Top | Bot,
        ExceptRight = Left | Top | Bot,
    }
}
