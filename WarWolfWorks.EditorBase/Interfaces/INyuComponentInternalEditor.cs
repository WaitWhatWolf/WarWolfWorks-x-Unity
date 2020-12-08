using System;
using UnityEditor;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.EditorBase.Interfaces
{
    /// <summary>
    /// Used for WarWolfWorks pre-made <see cref="INyuComponent"/> editors.
    /// </summary>
    internal interface INyuComponentInternalEditor
    {
        /// <summary>
        /// Does it still draw the default inspector/editor?
        /// </summary>
        bool DrawDefaultEditor { get; }

        /// <summary>
        /// Which component is it used for.
        /// </summary>
        Type EditorType { get; }

        /// <summary>
        /// Editor's OnEnable equivalent.
        /// </summary>
        /// <param name="serializedObject"></param>
        void OnEnable(SerializedObject serializedObject);

        /// <summary>
        /// Draws the inspector.
        /// </summary>
        void OnInspectorGUI();
    }
}
