using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WarWolfWorks.EditorBase.Utility
{
    /// <summary>
    /// Like <see cref="WarWolfWorks.Utility.Hooks"/>, but for the editor :Pog:.
    /// </summary>
    public static class EditorHooks
    {
        /// <summary>
        /// Returns all serialized properties inside a serialized object.
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public static List<SerializedProperty> GetAllSerializedProperties(SerializedObject serializedObject, bool enterChildren)
        {
            SerializedProperty sp = serializedObject.GetIterator();
            List<SerializedProperty> toReturn = new List<SerializedProperty>();

            sp.Next(true);
            sp.Next(enterChildren);

            do
            {
                toReturn.Add(sp.Copy());
            }
            while (sp.Next(enterChildren));

            return toReturn;
        }

        /// <summary>
        /// Returns all visible serialized properties inside a serialized object.
        /// (Does not include the first two entries m_Script and Base)
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <param name="enterChildren"></param>
        /// <returns></returns>
        public static List<SerializedProperty> GetAllVisibleProperties(SerializedObject serializedObject, bool enterChildren)
        {
            SerializedProperty sp = serializedObject.GetIterator();
            List<SerializedProperty> toReturn = new List<SerializedProperty>();

            sp.NextVisible(true);

            while (sp.NextVisible(enterChildren))
            {
                toReturn.Add(sp.Copy());
            }
            

            return toReturn;
        }

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
        /// Makes a separator line by exploiting <see cref="EditorGUILayout"/>. Doesn't make spaces before or after the line.
        /// </summary>
        public static void SlickSeparatorNS()
            => EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

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
