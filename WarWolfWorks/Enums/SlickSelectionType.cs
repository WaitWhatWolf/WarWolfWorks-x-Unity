namespace WarWolfWorks.Enums
{
    /// <summary>
    /// Describes how a menu option/cell is "selected".
    /// </summary>
    public enum SlickSelectionType
	{
		/// <summary>
		/// The menu cell is not selected.
		/// </summary>
		Unselected,
		/// <summary>
		/// The menu is about to be selected, but isn't.
		/// </summary>
		Hovered,
		/// <summary>
		/// The meny cell is selected.
		/// </summary>
		Selected,
		/// <summary>
		/// The menu cell is both selected and hovered on.
		/// </summary>
		SelectedHovered,
	}
}
