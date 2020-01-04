# WarWolfWorks-x-Unity
A library with numerous systems designed to cut developement time by a large amount.

If you're here to use this library, simply copy the Plugins folder and paste it inside your Unity's Assets folder.

This library implements:

- A large-scale entity system, which is usable for any type of game.

- "Hooks" static class which has over 350+ utility methods, functions and variables, all separated into
nested classes for categories.

- DefaultKeys static class used as a key customization, customizable through the unity editor by 
accessing the WarWolfWorks>Default Keys Customizer menu; It also has every method needed to make an in-game menu to customize these very keys.
All keys are saved using Hooks.Streaming, and are saved inside the StreamingAssets folder as DefaultKeys.kfidk.

- AdvancedDebug, which is a more advanced version of UnityEngine.Debug, which debugs information with custom colors, debug layers,
multiple utility debugs (like AdvancedDebug.LogException(Exception) or AdvancedDebug.LogIEnumerable<T>(IEnumerable<T>, layer)),
all customizable through the WarWolfWorks>Settings unity editor menu.

- Menu system, used for, well, in-game menus with UI.

- (currently in the process of being deprecated, avoid usage) Map system which is a way of loading a full list of objects into a scene,
without using the extremely painfull and bad Unity scene system.