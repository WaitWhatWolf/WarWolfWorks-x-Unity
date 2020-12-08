using UnityEditor;
using WarWolfWorks.Utility.Coloring;

namespace WarWolfWorks.EditorBase.Utility.Coloring
{
    [CustomEditor(typeof(ColorManager))]
    [CanEditMultipleObjects]
    public sealed class ColorManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("When using an IColorable to change the color of a material, " +
                "make sure to use sharedMaterial as doing otherwise will cause a memory leak, resulting in constant slowdown of your game.", MessageType.Warning);
            base.OnInspectorGUI();
        }
    }
}
