using UnityEngine;

namespace WarWolfWorks.Extensions
{
    /// <summary>
    /// State of a key; Used by <see cref="Keys"/> to give a key's input state.
    /// </summary>
	public enum KeyState : byte
	{
        /// <summary>
        /// The key is not interacted with.
        /// </summary>
		None = 0,
        /// <summary>
        /// The key is first pressed. (Equivalent to <see cref="Input.GetKeyDown(KeyCode)"/>)
        /// </summary>
        Pressed = 1,
        /// <summary>
        /// The key is held down. (Equivalent to <see cref="Input.GetKey(KeyCode)"/>)
        /// </summary>
        Held = 2,
        /// <summary>
        /// The key is being released. (Equivalent to <see cref="Input.GetKeyUp(KeyCode)"/>)
        /// </summary>
        Released = 3
	}
}