using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Implement this to an <see cref="NyuComponent"/> to implement a custom editor;
    /// Make sure to wrap the implementation and the <see cref="NyuOnInspectorGUI"/> in #if UNITY_EDITOR and #endif for your game to be compilable.
    /// For more info, see preprocessor directives.
    /// </summary>
    public interface INyuComponentEditor
    {
        /// <summary>
        /// Draws the custom inspector.
        /// </summary>
        void NyuOnInspectorGUI();
    }
}
