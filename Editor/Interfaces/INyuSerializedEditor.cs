using System;
using UnityEditor;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.EditorBase.Interfaces
{
    /// <summary>
    /// Class used to make custom editors of <see cref="INyuComponent"/>.
    /// </summary>
    public interface INyuSerializedEditor
    {
        /// <summary>
        /// What component is it an editor for.
        /// </summary>
        Type EditorOf { get; }
        /// <summary>
        /// Equivalent to Editor's OnEnable.
        /// </summary>
        void OnEnable(SerializedObject serializedObject);
        /// <summary>
        /// Equivalent to Editor's OnDisable.
        /// </summary>
        void OnDisable();
        /// <summary>
        /// Equivalent to Editor's OnInspectorGUI.
        /// </summary>
        void OnInspectorGUI();
    }
}
