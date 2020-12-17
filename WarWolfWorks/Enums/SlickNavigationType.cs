using WarWolfWorks.UI.MenusSystem.SlickMenu;

namespace WarWolfWorks.Enums
{
	/// <summary>
	/// Navigation type used by <see cref="SlickMenu"/>.
	/// </summary>
    public enum SlickNavigationType
    {
		/// <summary>
		/// No navigation; All elements are visual only, and are not meant to be interacted with.
		/// </summary>
		None,
		/// <summary>
		/// Standard navigation; All navigation is handled with <see cref="Direction"/>.
		/// (Important Note: When the hovered index is selected, the menu's SelectionIndex will NOT change,
		/// it only will if you put "SelectionIndex = Index;" at the start of <see cref="LHMenu{T}.OnSelectionAccepted(int)"/>.)
		/// </summary>
		Standard,
		/// <summary>
		/// All navigation is handled with <see cref="Direction"/>, but a cell must first be selected in order to be accepted,
		/// unlike <see cref="Standard"/> which gets accepted immediately.
		/// </summary>
		LongNavigation,
		/// <summary>
		/// Uses long navigation, but ignores it's long navigation rules with left-mouse button.
		/// </summary>
		LongNavigationExceptMouse,
	}
}
