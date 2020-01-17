using UnityEditor;
using UnityEngine;

namespace WarWolfWorks.EditorBase
{
    /// <summary>
    /// Like <see cref="WarWolfWorks.Utility.Hooks"/>, but for the editor :Pog:.
    /// </summary>
    public static class EditorHooks
    {
        /// <summary>
        /// Calls <see cref="EditorGUILayout.Space"/> in the multitude of counts.
        /// </summary>
        /// <param name="count"></param>
        public static void MultiSpace(int count)
        {
            for(int i = 0; i < count; i++)
                EditorGUILayout.Space();
        }

        /// <summary>
        /// Short for <see cref="Rect.width"/> / <see cref="int"/>.
        /// </summary>
        /// <param name="of"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static float DividedWidth(this Rect of, int by)
        {
            return of.width / by;
        }

        /// <summary>
        /// Makes a separator line by exploiting <see cref="EditorGUILayout"/>.
        /// </summary>
        public static void SlickSeparator()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Makes a label with <see cref="EditorGUILayout.Space"/> before and after it.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="titleStyle"></param>
        public static void SpacedLabel(string title, GUIStyle titleStyle)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, titleStyle);
            EditorGUILayout.Space();
        }
    }
}
