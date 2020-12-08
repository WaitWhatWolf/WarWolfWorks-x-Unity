using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Utility;
using static WarWolfWorks.Constants;
using static WarWolfWorks.EditorBase.Constants;

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
        /// <param name="enterChildren"></param>
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
        /// Returns all visible serialized properties inside a serialized object,
        /// as well as giving you a list of all <see cref="GUIContent"/> with <see cref="SerializedProperty.displayName"/> who start with "s_"
        /// or "S_" having their names corrected to be without them.
        /// (Does not include the first two entries m_Script and Base)
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <param name="enterChildren"></param>
        /// <param name="properties"></param>
        /// <param name="propertyContents"></param>
        /// <returns></returns>
        public static bool GetAllVisibleProperties(SerializedObject serializedObject, bool enterChildren, out List<SerializedProperty> properties, out List<GUIContent> propertyContents)
        {
            try
            {
                SerializedProperty sp = serializedObject.GetIterator();
                List<SerializedProperty> Properties = new List<SerializedProperty>();
                List<GUIContent> PropertyContents;

                sp.NextVisible(true);

                while (sp.NextVisible(enterChildren))
                {
                    Properties.Add(sp.Copy());
                }

                PropertyContents = new List<GUIContent>(Properties.Count);
                for (int i = 0; i < Properties.Count; i++)
                {
                    PropertyContents.Add(new GUIContent(
                        Expression_NoS.IsMatch(Properties[i].name)
                        ? Properties[i].displayName.Substring(2)
                        : Properties[i].displayName));
                }

                properties = Properties;
                propertyContents = PropertyContents;

                return true;
            }
            catch
            {
                properties = null;
                propertyContents = null;
                return false;
            }
        }

        /// <summary>
        /// Returns all visible serialized properties inside a serialized object.
        /// if removesS is true, it will also give you a list of all <see cref="GUIContent"/> with <see cref="SerializedProperty.displayName"/> who start with "s_"
        /// or "S_" having their names corrected to be without them; Otherwise, gives the original <see cref="GUIContent"/>.
        /// (Does not include the first two entries m_Script and Base)
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <param name="enterChildren"></param>
        /// <param name="properties"></param>
        /// <param name="propertyContents"></param>
        /// <param name="removesS_"></param>
        /// <returns></returns>
        public static bool GetAllVisibleProperties(SerializedObject serializedObject, bool enterChildren, out List<SerializedProperty> properties, out List<GUIContent> propertyContents, bool removesS_)
        {
            try
            {
                SerializedProperty sp = serializedObject.GetIterator();
                List<SerializedProperty> Properties = new List<SerializedProperty>();
                List<GUIContent> PropertyContents;

                sp.NextVisible(true);

                while (sp.NextVisible(enterChildren))
                {
                    Properties.Add(sp.Copy());
                }

                PropertyContents = new List<GUIContent>(Properties.Count);
                for (int i = 0; i < Properties.Count; i++)
                {
                    PropertyContents.Add(new GUIContent(
                        removesS_ && Expression_NoS.IsMatch(Properties[i].name)
                        ? Properties[i].displayName.Substring(2)
                        : Properties[i].displayName));
                }

                properties = Properties;
                propertyContents = PropertyContents;

                return true;
            }
            catch(Exception e)
            {
                AdvancedDebug.LogException(e);
                properties = null;
                propertyContents = null;
                return false;
            }
        }

        /// <summary>
        /// Returns all visible serialized properties inside a serialized property,
        /// as well as giving you a list of all <see cref="GUIContent"/> with <see cref="SerializedProperty.displayName"/> who start with "s_"
        /// or "S_" having their names corrected to be without them.
        /// (Does not include the first two entries m_Script and Base)
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="enterChildren"></param>
        /// <param name="properties"></param>
        /// <param name="propertyContents"></param>
        /// <param name="removesS_"></param>
        /// <returns></returns>
        public static bool GetAllVisibleProperties(SerializedProperty serializedProperty, bool enterChildren, out List<SerializedProperty> properties, out List<GUIContent> propertyContents, bool removesS_)
        {
            try
            {
                SerializedProperty sp = serializedProperty.Copy();
                List<SerializedProperty> Properties = new List<SerializedProperty>();
                List<GUIContent> PropertyContents;

                while (sp.NextVisible(enterChildren))
                {
                    Properties.Add(sp.Copy());
                }

                PropertyContents = new List<GUIContent>(Properties.Count);
                for (int i = 0; i < Properties.Count; i++)
                {
                    PropertyContents.Add(new GUIContent(
                        removesS_ && Expression_NoS.IsMatch(Properties[i].name)
                        ? Properties[i].displayName.Substring(2)
                        : Properties[i].displayName));
                }

                properties = Properties;
                propertyContents = PropertyContents;

                return true;
            }
            catch(Exception e)
            {
                AdvancedDebug.LogException(e);
                properties = null;
                propertyContents = null;
                return false;
            }
        }

        /// <summary>
        /// Returns all visible serialized properties inside a serialized object.
        /// (Does not include the first two entries m_Script and Base)
        /// Note: EXTREMELY SLOW.
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <param name="enterChildren"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static bool GetAllVisiblePropertiesWithAttributes<T>(SerializedObject serializedObject, bool enterChildren, out List<(SerializedProperty, T[])> properties) where T : Attribute
        {
            SerializedProperty sp = serializedObject.GetIterator();
            List<(SerializedProperty, T[])> toReturn = new List<(SerializedProperty, T[])>();

            sp.NextVisible(true);

            while (sp.NextVisible(enterChildren))
            {
                SerializedProperty property = sp.Copy();
                T[] attributes = null;
                try
                {
                    attributes = serializedObject.targetObject.GetType()
                        .GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .GetCustomAttributes(typeof(T), true)
                        .Cast<T>()
                        .ToArray();
                }
                catch { /*AdvancedDebug.LogWarning("There was a problem finding attributes in GetAllVisiblePropertiesWithAttributes.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);*/ }
                toReturn.Add((property, attributes));
            }

            properties = toReturn;

            return toReturn.Count > 0;
        }

        /// <summary>
        /// Returns all visible serialized properties inside a serialized property.
        /// Note: EXTREMELY SLOW.
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="enterChildren"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static bool GetAllVisiblePropertiesWithAttributes<T>(SerializedProperty serializedProperty, bool enterChildren, out List<(SerializedProperty, T[])> properties) where T : Attribute
        {
            SerializedProperty sp = serializedProperty.Copy();
            List<(SerializedProperty, T[])> toReturn = new List<(SerializedProperty, T[])>();

            sp.NextVisible(true);

            while (sp.NextVisible(enterChildren))
            {
                SerializedProperty property = sp.Copy();
                T[] attributes = null;
                try
                {
                    attributes = sp.serializedObject.targetObject.GetType()
                        .GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .GetCustomAttributes(typeof(T), true)
                        .Cast<T>()
                        .ToArray();
                }
                catch { /*AdvancedDebug.LogWarning("There was a problem finding attributes in GetAllVisiblePropertiesWithAttributes.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);*/ }
                toReturn.Add((property, attributes));
            }

            properties = toReturn;

            return toReturn.Count > 0;
        }

        /// <summary>
        /// Calls <see cref="EditorGUILayout.Space"/> in the multitude of counts.
        /// </summary>
        /// <param name="count"></param>
        public static void MultiSpace(int count)
        {
            for (int i = 0; i < count; i++)
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
        /// Makes a separator line by exploiting <see cref="EditorGUILayout"/>.
        /// </summary>
        public static void SlickSeparator(Rect rect)
        {
            Rect used = new Rect(rect);
            used.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, "", GUI.skin.horizontalSlider);
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

        /// <summary>
        /// Returns true if a property is an expand-like property, similar to an array. (All custom Serialized structs/classes return true)
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool PropertyIsArray(SerializedProperty property)
        {
            string path = property.propertyPath;
            int dotIndex = path.IndexOf('.');
            if (dotIndex == -1) return false;
            string propName = path.Substring(0, dotIndex);
            SerializedProperty p = property.serializedObject.FindProperty(propName);
            return p.isArray;
        }

        /// <summary>
        /// Makes a square filled with the given color.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public static void DrawColoredSquare(Rect position, Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            Texture2D prevTexture = GUI.skin.box.normal.background;
            GUI.skin.box.normal.background = texture;
            GUI.Box(position, GUIContent.none);
            GUI.skin.box.normal.background = prevTexture;
            Texture2D.DestroyImmediate(texture);
        }

        /// <summary>
        /// Draws a type parser from a string field.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="original"></param>
        /// <param name="returnType"></param>
        /// <param name="requiredSubtype"></param>
        /// <param name="canBeNull"></param>
        /// <returns></returns>
        public static string DrawTypeParse(GUIContent label, string original, out Type returnType, Type requiredSubtype = null, bool canBeNull = true)
        {
            original = EditorGUILayout.TextField(label, original);
            returnType = Hooks.ParseType(original);
            bool subclassMet = requiredSubtype == null || returnType.IsSubclassOf(requiredSubtype);
            bool nullMet = canBeNull && string.IsNullOrEmpty(original) || returnType != null;

            if (!subclassMet && nullMet)
                EditorGUILayout.HelpBox(
                    "Cannot parse given value! Make sure the type you want to parse is subclass of "
                    + requiredSubtype.Name
                    + " and that it is not null.", MessageType.Warning);
            else if (!nullMet)
                EditorGUILayout.HelpBox(
                    "Cannot parse given value! The string value given cannot be null.", MessageType.Warning);
            else if (!subclassMet)
                EditorGUILayout.HelpBox(
                    "Cannot parse given value! Make sure the type you want to parse is subclass of "
                    + requiredSubtype.Name, MessageType.Warning);

            return original;
        }
    

        /// <summary>
        /// Returns a texture to be used in the editor.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Texture GetColoredTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Makes the given rect go to it's maxY position, as well as calling <see cref="EditorGUILayout.Space"/> equal to the difference
        /// between the current y position and the previous of the given rect.
        /// </summary>
        /// <param name="rect"></param>
        public static void RectSpace(ref Rect rect)
        {
            float dif = rect.yMax - rect.y;
            rect.y = rect.yMax;
            EditorGUILayout.Space(dif * 2);
        }

        /// <summary>
        /// Returns true if current event's mouse position is within the given rect.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool EventMouseInRect(Rect rect)
        {
            return rect.Contains(Event.current.mousePosition);
        }

        /// <summary>
        /// All utility based around <see cref="Rect"/>.
        /// </summary>
        public static class Rects
        {
            #region SetRectsWidth
            /// <summary>
            /// Sets the width of all rects to the given width value.
            /// </summary>
            /// <param name="width"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            public static void SetRectsWidth(float width, ref Rect rect1, ref Rect rect2)
            {
                rect1.width = width;
                rect2.width = width;
            }

            /// <summary>
            /// Sets the width of all rects to the given width value.
            /// </summary>
            /// <param name="width"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            public static void SetRectsWidth(float width, ref Rect rect1, ref Rect rect2, ref Rect rect3)
            {
                rect1.width = width;
                rect2.width = width;
                rect3.width = width;
            }

            /// <summary>
            /// Sets the width of all rects to the given width value.
            /// </summary>
            /// <param name="width"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            public static void SetRectsWidth(float width, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4)
            {
                rect1.width = width;
                rect2.width = width;
                rect3.width = width;
                rect4.width = width;
            }

            /// <summary>
            /// Sets the width of all rects to the given width value.
            /// </summary>
            /// <param name="width"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            /// <param name="rect5"></param>
            public static void SetRectsWidth(float width, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4, ref Rect rect5)
            {
                rect1.width = width;
                rect2.width = width;
                rect3.width = width;
                rect4.width = width;
                rect5.width = width;
            }
            #endregion

            #region SetRectsHeight
            /// <summary>
            /// Sets the height of all rects to the given height value.
            /// </summary>
            /// <param name="height"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            public static void SetRectsHeight(float height, ref Rect rect1, ref Rect rect2)
            {
                rect1.height = height;
                rect2.height = height;
            }
            /// <summary>
            /// Sets the height of all rects to the given height value.
            /// </summary>
            /// <param name="height"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            public static void SetRectsHeight(float height, ref Rect rect1, ref Rect rect2, ref Rect rect3)
            {
                rect1.height = height;
                rect2.height = height;
                rect3.height = height;
            }
            /// <summary>
            /// Sets the height of all rects to the given height value.
            /// </summary>
            /// <param name="height"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            public static void SetRectsHeight(float height, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4)
            {
                rect1.height = height;
                rect2.height = height;
                rect3.height = height;
                rect4.height = height;
            }
            /// <summary>
            /// Sets the height of all rects to the given height value.
            /// </summary>
            /// <param name="height"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            /// <param name="rect5"></param>
            public static void SetRectsHeight(float height, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4, ref Rect rect5)
            {
                rect1.height = height;
                rect2.height = height;
                rect3.height = height;
                rect4.height = height;
                rect5.height = height;
            }
            #endregion

            #region SetRectsXPos
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            public static void SetRectsXPos(float xPos, ref Rect rect1, ref Rect rect2)
            {
                rect1.x = xPos;
                rect2.x = xPos;
            }
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            public static void SetRectsXPos(float xPos, ref Rect rect1, ref Rect rect2, ref Rect rect3)
            {
                rect1.x = xPos;
                rect2.x = xPos;
                rect3.x = xPos;
            }
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            public static void SetRectsXPos(float xPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4)
            {
                rect1.x = xPos;
                rect2.x = xPos;
                rect3.x = xPos;
                rect4.x = xPos;
            }
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            /// <param name="rect5"></param>
            public static void SetRectsXPos(float xPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4, ref Rect rect5)
            {
                rect1.x = xPos;
                rect2.x = xPos;
                rect3.x = xPos;
                rect4.x = xPos;
                rect5.x = xPos;
            }
            #endregion

            #region SetRectsYPos
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            public static void SetRectsYPos(float yPos, ref Rect rect1, ref Rect rect2)
            {
                rect1.y = yPos;
                rect2.y = yPos;
            }
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            public static void SetRectsYPos(float yPos, ref Rect rect1, ref Rect rect2, ref Rect rect3)
            {
                rect1.y = yPos;
                rect2.y = yPos;
                rect3.y = yPos;
            }
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            public static void SetRectsYPos(float yPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4)
            {
                rect1.y = yPos;
                rect2.y = yPos;
                rect3.y = yPos;
                rect4.y = yPos;
            }
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            /// <param name="rect5"></param>
            public static void SetRectsYPos(float yPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4, ref Rect rect5)
            {
                rect1.y = yPos;
                rect2.y = yPos;
                rect3.y = yPos;
                rect4.y = yPos;
                rect5.y = yPos;
            }
            #endregion 
            
            #region SetRectsXMinPos
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            public static void SetRectsXMinPos(float xPos, ref Rect rect1, ref Rect rect2)
            {
                rect1.xMin = xPos;
                rect2.xMin = xPos;
            }
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            public static void SetRectsXMinPos(float xPos, ref Rect rect1, ref Rect rect2, ref Rect rect3)
            {
                rect1.xMin = xPos;
                rect2.xMin = xPos;
                rect3.xMin = xPos;
            }
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            public static void SetRectsXMinPos(float xPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4)
            {
                rect1.xMin = xPos;
                rect2.xMin = xPos;
                rect3.xMin = xPos;
                rect4.xMin = xPos;
            }
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            /// <param name="rect5"></param>
            public static void SetRectsXMinPos(float xPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4, ref Rect rect5)
            {
                rect1.xMin = xPos;
                rect2.xMin = xPos;
                rect3.xMin = xPos;
                rect4.xMin = xPos;
                rect5.xMin = xPos;
            }
            #endregion

            #region SetRectsYMinPos
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            public static void SetRectsYMinPos(float yPos, ref Rect rect1, ref Rect rect2)
            {
                rect1.yMin = yPos;
                rect2.yMin = yPos;
            }
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            public static void SetRectsYMinPos(float yPos, ref Rect rect1, ref Rect rect2, ref Rect rect3)
            {
                rect1.yMin = yPos;
                rect2.yMin = yPos;
                rect3.yMin = yPos;
            }
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            public static void SetRectsYMinPos(float yPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4)
            {
                rect1.yMin = yPos;
                rect2.yMin = yPos;
                rect3.yMin = yPos;
                rect4.yMin = yPos;
            }
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            /// <param name="rect5"></param>
            public static void SetRectsYMinPos(float yPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4, ref Rect rect5)
            {
                rect1.yMin = yPos;
                rect2.yMin = yPos;
                rect3.yMin = yPos;
                rect4.yMin = yPos;
                rect5.yMin = yPos;
            }
            #endregion 
            
            #region SetRectsXMaxPos
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            public static void SetRectsXMaxPos(float xPos, ref Rect rect1, ref Rect rect2)
            {
                rect1.xMax = xPos;
                rect2.xMax = xPos;
            }
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            public static void SetRectsXMaxPos(float xPos, ref Rect rect1, ref Rect rect2, ref Rect rect3)
            {
                rect1.xMax = xPos;
                rect2.xMax = xPos;
                rect3.xMax = xPos;
            }
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            public static void SetRectsXMaxPos(float xPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4)
            {
                rect1.xMax = xPos;
                rect2.xMax = xPos;
                rect3.xMax = xPos;
                rect4.xMax = xPos;
            }
            /// <summary>
            /// Sets the X position of all rects to the given xPos value.
            /// </summary>
            /// <param name="xPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            /// <param name="rect5"></param>
            public static void SetRectsXMaxPos(float xPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4, ref Rect rect5)
            {
                rect1.xMax = xPos;
                rect2.xMax = xPos;
                rect3.xMax = xPos;
                rect4.xMax = xPos;
                rect5.xMax = xPos;
            }
            #endregion

            #region SetRectsYMaxPos
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            public static void SetRectsYMaxPos(float yPos, ref Rect rect1, ref Rect rect2)
            {
                rect1.yMax = yPos;
                rect2.yMax = yPos;
            }
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            public static void SetRectsYMaxPos(float yPos, ref Rect rect1, ref Rect rect2, ref Rect rect3)
            {
                rect1.yMax = yPos;
                rect2.yMax = yPos;
                rect3.yMax = yPos;
            }
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            public static void SetRectsYMaxPos(float yPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4)
            {
                rect1.yMax = yPos;
                rect2.yMax = yPos;
                rect3.yMax = yPos;
                rect4.yMax = yPos;
            }
            /// <summary>
            /// Sets the Y position of all rects to the given yPos value.
            /// </summary>
            /// <param name="yPos"></param>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <param name="rect3"></param>
            /// <param name="rect4"></param>
            /// <param name="rect5"></param>
            public static void SetRectsYMaxPos(float yPos, ref Rect rect1, ref Rect rect2, ref Rect rect3, ref Rect rect4, ref Rect rect5)
            {
                rect1.yMax = yPos;
                rect2.yMax = yPos;
                rect3.yMax = yPos;
                rect4.yMax = yPos;
                rect5.yMax = yPos;
            }
            #endregion
        }

        /// <summary>
        /// All utility based around drawing values/properties in the inspector.
        /// </summary>
        public static class Drawers
        {
            /// <summary>
            /// Draws a stat field; Determines what type of stat it is, and draws it accordingly.
            /// </summary>
            /// <param name="rect"></param>
            /// <param name="property"></param>
            /// <returns></returns>
            public static void AutoStatField<TStacking, TAffection>(Rect rect, SerializedProperty property)
                where TStacking : Enum
                where TAffection : Enum
            {
                SerializedProperty valueProp = property.FindPropertyRelative("value");
                switch (property)
                {
                    default:
                        StatField<TStacking, TAffection>(rect, property);
                        break;
                    case SerializedProperty _ when valueProp.FindPropertyRelative("level") != null:
                        LevelStatField<TStacking, TAffection>(rect, property);
                        break;
                    case SerializedProperty _ when property.FindPropertyRelative("countdown") != null:
                        CountdownStatField<TStacking, TAffection>(rect, property);
                        break;
                }
            }

            /// <summary>
            /// Draws a stat field; Determines what type of stat it is, and draws it accordingly.
            /// </summary>
            /// <param name="rect"></param>
            /// <param name="property"></param>
            /// <param name="stackingType"></param>
            /// <param name="affectionsType"></param>
            /// <returns></returns>
            public static void AutoStatField(Rect rect, SerializedProperty property, Type stackingType, Type affectionsType)
            {
                SerializedProperty valueProp = property.FindPropertyRelative("value");
                switch (property)
                {
                    default:
                        StatField(rect, property, stackingType, affectionsType);
                        break;
                    case SerializedProperty _ when valueProp.FindPropertyRelative("level") != null:
                        LevelStatField(rect, property, stackingType, affectionsType);
                        break;
                    case SerializedProperty _ when property.FindPropertyRelative("countdown") != null:
                        CountdownStatField(rect, property, stackingType, affectionsType);
                        break;
                }
            }

            /// <summary>
            /// Draws a <see cref="IStat"/> field.
            /// </summary>
            /// <param name="rect"></param>
            /// <param name="property"></param>
            public static void StatField<TStacking, TAffection>(Rect rect, SerializedProperty property) 
                where TStacking : Enum
                where TAffection : Enum
            {
                Rect valueRect = new Rect(rect);
                Rect stackingRect = new Rect(rect);
                Rect affectionsRect = new Rect(rect);
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                Rects.SetRectsWidth(rect.width / 3, ref valueRect, ref stackingRect, ref affectionsRect);
                stackingRect.x = valueRect.xMax;
                affectionsRect.x = stackingRect.xMax;
                affectionsRect.xMin += 15;
                
                SerializedProperty sp_Value = property.FindPropertyRelative("s_Value");
                SerializedProperty sp_Stacking = property.FindPropertyRelative("s_Stacking");
                SerializedProperty sp_Affections = property.FindPropertyRelative("s_Affections");

                EditorGUI.LabelField(valueRect, (Expression_NoS.IsMatch(property.name) ? property.displayName.Remove(0, 2) : property.displayName) + " Value");
                EditorGUI.LabelField(stackingRect, EVN_STAT_STACKING);
                EditorGUI.LabelField(affectionsRect, EVN_STAT_AFFECTIONS);

                Rects.SetRectsYPos(rect.yMax, ref valueRect, ref stackingRect, ref affectionsRect);

                EditorGUI.PropertyField(valueRect, sp_Value, GUIContent.none);

                try
                {
                    Enum enumVal = (Enum)Enum.Parse(typeof(TStacking), sp_Stacking.intValue.ToString());
                    enumVal = EditorGUI.EnumPopup(stackingRect, enumVal);
                    sp_Stacking.intValue = (int)Enum.ToObject(typeof(TStacking), enumVal);
                }
                catch
                {
                    EditorGUI.PropertyField(stackingRect, sp_Stacking, GUIContent.none);
                }

                EditorGUI.PropertyField(affectionsRect, sp_Affections, GUIContent.none);

                if (sp_Affections.isExpanded)
                {
                    affectionsRect.y = affectionsRect.yMax;
                    EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                    if (GUI.Button(affectionsRect, "Add Affection"))
                        sp_Affections.InsertArrayElementAtIndex(sp_Affections.arraySize == 0 ? 0 : sp_Affections.arraySize - 1);

                    if (sp_Affections.arraySize > 0)
                    {
                        affectionsRect.y = affectionsRect.yMax;
                        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                        if (GUI.Button(affectionsRect, "Remove Affection"))
                            sp_Affections.DeleteArrayElementAtIndex(sp_Affections.arraySize - 1);
                    }
                    try
                    {
                        for (int i = 0; i < sp_Affections.arraySize; i++)
                        {
                            affectionsRect.y = affectionsRect.yMax;
                            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                            SerializedProperty IndexProperty = sp_Affections.GetArrayElementAtIndex(i);
                            Enum enumVal = (Enum)Enum.Parse(typeof(TAffection), IndexProperty.intValue.ToString());
                            enumVal = EditorGUI.EnumPopup(affectionsRect, enumVal);
                            IndexProperty.intValue = (int)Enum.ToObject(typeof(TAffection), enumVal);
                        }
                    }
                    catch
                    {
                        for (int i = 0; i < sp_Affections.arraySize; i++)
                        {
                            affectionsRect.y = affectionsRect.yMax;
                            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                            EditorGUI.PropertyField(affectionsRect, sp_Affections.GetArrayElementAtIndex(i), GUIContent.none);
                        }
                    }
                }
            }

            /// <summary>
            /// Draws a <see cref="IStat"/> field.
            /// </summary>
            /// <param name="rect"></param>
            /// <param name="property"></param>
            /// <param name="stackingType"></param>
            /// <param name="affectionsType"></param>
            public static void StatField(Rect rect, SerializedProperty property, Type stackingType, Type affectionsType)
            {
                Rect valueRect = new Rect(rect);
                Rect stackingRect = new Rect(rect);
                Rect affectionsRect = new Rect(rect);
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                Rects.SetRectsWidth(rect.width / 3, ref valueRect, ref stackingRect, ref affectionsRect);
                stackingRect.x = valueRect.xMax;
                affectionsRect.x = stackingRect.xMax;
                affectionsRect.xMin += 15;
                
                SerializedProperty sp_Value = property.FindPropertyRelative("s_Value");
                SerializedProperty sp_Stacking = property.FindPropertyRelative("s_Stacking");
                SerializedProperty sp_Affections = property.FindPropertyRelative("s_Affections");
                
                EditorGUI.LabelField(valueRect, (Expression_NoS.IsMatch(property.name) ? property.displayName.Remove(0, 2) : property.displayName) + " Value");
                EditorGUI.LabelField(stackingRect, EVN_STAT_STACKING);
                EditorGUI.LabelField(affectionsRect, EVN_STAT_AFFECTIONS);

                Rects.SetRectsYPos(rect.yMax, ref valueRect, ref stackingRect, ref affectionsRect);

                EditorGUI.PropertyField(valueRect, sp_Value, GUIContent.none);

                try
                {
                    Enum enumVal = (Enum)Enum.Parse(stackingType, sp_Stacking.intValue.ToString());
                    enumVal = EditorGUI.EnumPopup(stackingRect, enumVal);
                    sp_Stacking.intValue = (int)Enum.ToObject(stackingType, enumVal);
                }
                catch
                {
                    EditorGUI.PropertyField(stackingRect, sp_Stacking, GUIContent.none);
                }

                EditorGUI.PropertyField(affectionsRect, sp_Affections, GUIContent.none);

                if (sp_Affections.isExpanded)
                {
                    affectionsRect.y = affectionsRect.yMax;
                    EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                    if (GUI.Button(affectionsRect, "Add Affection"))
                        sp_Affections.InsertArrayElementAtIndex(sp_Affections.arraySize == 0 ? 0 : sp_Affections.arraySize - 1);

                    if (sp_Affections.arraySize > 0)
                    {
                        affectionsRect.y = affectionsRect.yMax;
                        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                        if (GUI.Button(affectionsRect, "Remove Affection"))
                            sp_Affections.DeleteArrayElementAtIndex(sp_Affections.arraySize - 1);
                    }
                    try
                    {
                        for (int i = 0; i < sp_Affections.arraySize; i++)
                        {
                            affectionsRect.y = affectionsRect.yMax;
                            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                            SerializedProperty IndexProperty = sp_Affections.GetArrayElementAtIndex(i);
                            Enum enumVal = (Enum)Enum.Parse(affectionsType, IndexProperty.intValue.ToString());
                            enumVal = EditorGUI.EnumPopup(affectionsRect, enumVal);
                            IndexProperty.intValue = (int)Enum.ToObject(affectionsType, enumVal);
                        }
                    }
                    catch
                    {
                        for (int i = 0; i < sp_Affections.arraySize; i++)
                        {
                            affectionsRect.y = affectionsRect.yMax;
                            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                            EditorGUI.PropertyField(affectionsRect, sp_Affections.GetArrayElementAtIndex(i), GUIContent.none);
                        }
                    }
                }
            }

            /// <summary>
            /// Draws a <see cref="LevelStat"/> field.
            /// </summary>
            /// <typeparam name="TStacking"></typeparam>
            /// <typeparam name="TAffection"></typeparam>
            /// <param name="rect"></param>
            /// <param name="property"></param>
            public static void LevelStatField<TStacking, TAffection>(Rect rect, SerializedProperty property)
                where TStacking : Enum
                where TAffection : Enum
            {
                float finalHeight = EditorGUIUtility.singleLineHeight;

                Rect valueRect = new Rect(rect);
                Rect stackingRect = new Rect(rect);
                Rect affectionsRect = new Rect(rect);

                Rects.SetRectsWidth(rect.width / 3, ref valueRect, ref stackingRect, ref affectionsRect);
                stackingRect.x = valueRect.xMax;
                affectionsRect.x = stackingRect.xMax;
                affectionsRect.xMin += 15;

                SerializedProperty sp_Value = property.FindPropertyRelative("s_Value");
                SerializedProperty sp_Stacking = property.FindPropertyRelative("s_Stacking");
                SerializedProperty sp_Affections = property.FindPropertyRelative("s_Affections");
                SerializedProperty sp_LevelValues = sp_Value.FindPropertyRelative("levelValues");

                EditorGUI.LabelField(valueRect, (Expression_NoS.IsMatch(property.name) ? property.displayName.Remove(0, 2) : property.displayName) + " Value");
                EditorGUI.LabelField(stackingRect, EVN_STAT_STACKING);
                EditorGUI.LabelField(affectionsRect, EVN_STAT_AFFECTIONS);

                Rects.SetRectsYPos(rect.yMax, ref valueRect, ref stackingRect, ref affectionsRect);

                EditorGUI.PropertyField(valueRect, sp_Value, GUIContent.none);

                if (sp_LevelValues.isExpanded)
                {
                    finalHeight += EditorGUIUtility.singleLineHeight * (sp_LevelValues.arraySize > 0 ? 2f : 1f);
                    finalHeight += sp_LevelValues.arraySize * 3f * EditorGUIUtility.singleLineHeight; 
                }

                try
                {
                    Enum enumVal = (Enum)Enum.Parse(typeof(TStacking), sp_Stacking.intValue.ToString());
                    enumVal = EditorGUI.EnumPopup(stackingRect, enumVal);
                    sp_Stacking.intValue = (int)Enum.ToObject(typeof(TStacking), enumVal);
                }
                catch
                {
                    EditorGUI.PropertyField(stackingRect, sp_Stacking, GUIContent.none);
                }

                EditorGUI.PropertyField(affectionsRect, sp_Affections, GUIContent.none);

                if (sp_Affections.isExpanded)
                {
                    float affectionsHeight = EditorGUIUtility.singleLineHeight;
                    affectionsRect.y = affectionsRect.yMax;
                    affectionsHeight += EditorGUIUtility.singleLineHeight;

                    if (GUI.Button(affectionsRect, "Add Affection"))
                        sp_Affections.InsertArrayElementAtIndex(sp_Affections.arraySize == 0 ? 0 : sp_Affections.arraySize - 1);

                    if (sp_Affections.arraySize > 0)
                    {
                        affectionsRect.y = affectionsRect.yMax;
                        affectionsHeight += EditorGUIUtility.singleLineHeight;

                        if (GUI.Button(affectionsRect, "Remove Affection"))
                            sp_Affections.DeleteArrayElementAtIndex(sp_Affections.arraySize - 1);
                    }
                    try
                    {
                        for (int i = 0; i < sp_Affections.arraySize; i++)
                        {
                            affectionsRect.y = affectionsRect.yMax;
                            affectionsHeight += EditorGUIUtility.singleLineHeight;

                            SerializedProperty IndexProperty = sp_Affections.GetArrayElementAtIndex(i);
                            Enum enumVal = (Enum)Enum.Parse(typeof(TAffection), IndexProperty.intValue.ToString());
                            enumVal = EditorGUI.EnumPopup(affectionsRect, enumVal);
                            IndexProperty.intValue = (int)Enum.ToObject(typeof(TAffection), enumVal);
                        }
                    }
                    catch
                    {
                        for (int i = 0; i < sp_Affections.arraySize; i++)
                        {
                            affectionsRect.y = affectionsRect.yMax;
                            affectionsHeight += EditorGUIUtility.singleLineHeight;

                            EditorGUI.PropertyField(affectionsRect, sp_Affections.GetArrayElementAtIndex(i), GUIContent.none);
                        }
                    }

                    if (affectionsHeight > finalHeight)
                        finalHeight = affectionsHeight;
                }

                EditorGUILayout.Space(finalHeight);
            }

            /// <summary>
            /// Draws a <see cref="LevelStat"/> field.
            /// </summary>
            /// <param name="rect"></param>
            /// <param name="property"></param>
            /// <param name="stackingType"></param>
            /// <param name="affectionsType"></param>
            public static void LevelStatField(Rect rect, SerializedProperty property, Type stackingType, Type affectionsType)
            {
                float finalHeight = EditorGUIUtility.singleLineHeight;

                Rect valueRect = new Rect(rect);
                Rect stackingRect = new Rect(rect);
                Rect affectionsRect = new Rect(rect);

                Rects.SetRectsWidth(rect.width / 3, ref valueRect, ref stackingRect, ref affectionsRect);
                stackingRect.x = valueRect.xMax;
                affectionsRect.x = stackingRect.xMax;
                affectionsRect.xMin += 15;

                SerializedProperty sp_Value = property.FindPropertyRelative("s_Value");
                SerializedProperty sp_Stacking = property.FindPropertyRelative("s_Stacking");
                SerializedProperty sp_Affections = property.FindPropertyRelative("s_Affections");
                SerializedProperty sp_LevelValues = sp_Value.FindPropertyRelative("levelValues");

                EditorGUI.LabelField(valueRect, (Expression_NoS.IsMatch(property.name) ? property.displayName.Remove(0, 2) : property.displayName) + " Value");
                EditorGUI.LabelField(stackingRect, EVN_STAT_STACKING);
                EditorGUI.LabelField(affectionsRect, EVN_STAT_AFFECTIONS);

                Rects.SetRectsYPos(rect.yMax, ref valueRect, ref stackingRect, ref affectionsRect);

                EditorGUI.PropertyField(valueRect, sp_Value, GUIContent.none);

                if (sp_LevelValues.isExpanded)
                {
                    finalHeight += EditorGUIUtility.singleLineHeight * (sp_LevelValues.arraySize > 0 ? 2f : 1f);
                    finalHeight += sp_LevelValues.arraySize * 3f * EditorGUIUtility.singleLineHeight;
                }

                try
                {
                    Enum enumVal = (Enum)Enum.Parse(stackingType, sp_Stacking.intValue.ToString());
                    enumVal = EditorGUI.EnumPopup(stackingRect, enumVal);
                    sp_Stacking.intValue = (int)Enum.ToObject(stackingType, enumVal);
                }
                catch
                {
                    EditorGUI.PropertyField(stackingRect, sp_Stacking, GUIContent.none);
                }

                EditorGUI.PropertyField(affectionsRect, sp_Affections, GUIContent.none);

                if (sp_Affections.isExpanded)
                {
                    float affectionsHeight = EditorGUIUtility.singleLineHeight;
                    affectionsRect.y = affectionsRect.yMax;
                    affectionsHeight += EditorGUIUtility.singleLineHeight;

                    if (GUI.Button(affectionsRect, "Add Affection"))
                        sp_Affections.InsertArrayElementAtIndex(sp_Affections.arraySize == 0 ? 0 : sp_Affections.arraySize - 1);

                    if (sp_Affections.arraySize > 0)
                    {
                        affectionsRect.y = affectionsRect.yMax;
                        affectionsHeight += EditorGUIUtility.singleLineHeight;

                        if (GUI.Button(affectionsRect, "Remove Affection"))
                            sp_Affections.DeleteArrayElementAtIndex(sp_Affections.arraySize - 1);
                    }
                    try
                    {
                        for (int i = 0; i < sp_Affections.arraySize; i++)
                        {
                            affectionsRect.y = affectionsRect.yMax;
                            affectionsHeight += EditorGUIUtility.singleLineHeight;

                            SerializedProperty IndexProperty = sp_Affections.GetArrayElementAtIndex(i);
                            Enum enumVal = (Enum)Enum.Parse(affectionsType, IndexProperty.intValue.ToString());
                            enumVal = EditorGUI.EnumPopup(affectionsRect, enumVal);
                            IndexProperty.intValue = (int)Enum.ToObject(affectionsType, enumVal);
                        }
                    }
                    catch
                    {
                        for (int i = 0; i < sp_Affections.arraySize; i++)
                        {
                            affectionsRect.y = affectionsRect.yMax;
                            affectionsHeight += EditorGUIUtility.singleLineHeight;

                            EditorGUI.PropertyField(affectionsRect, sp_Affections.GetArrayElementAtIndex(i), GUIContent.none);
                        }
                    }

                    if (affectionsHeight > finalHeight)
                        finalHeight = affectionsHeight;
                }

                EditorGUILayout.Space(finalHeight);
            }
           
            /// <summary>
            /// Draws a <see cref="CountdownStat"/> field.
            /// </summary>
            /// <typeparam name="TStacking"></typeparam>
            /// <typeparam name="TAffection"></typeparam>
            /// <param name="rect"></param>
            /// <param name="property"></param>
            public static void CountdownStatField<TStacking, TAffection>(Rect rect, SerializedProperty property)
                where TStacking : Enum
                where TAffection : Enum
            {
                Rect countdownRect = new Rect(rect);
                countdownRect.width /= 4;
                rect.width -= rect.width / 4;
                countdownRect.x = rect.xMax;

                EditorGUI.LabelField(countdownRect, "Countdown");
                countdownRect.y = countdownRect.yMax;
                EditorGUI.PropertyField(countdownRect, property.FindPropertyRelative("s_Countdown"), GUIContent.none);

                StatField<TStacking, TAffection>(rect, property);
            }

            /// <summary>
            /// Draws a <see cref="CountdownStat"/> field.
            /// </summary>
            /// <param name="rect"></param>
            /// <param name="property"></param>
            /// <param name="stackingType"></param>
            /// <param name="affectionsType"></param>
            public static void CountdownStatField(Rect rect, SerializedProperty property, Type stackingType, Type affectionsType)
            {
                Rect countdownRect = new Rect(rect);
                countdownRect.width /= 4;
                rect.width -= rect.width / 4;
                countdownRect.x = rect.xMax;

                EditorGUI.LabelField(countdownRect, "Countdown");
                countdownRect.y = countdownRect.yMax;
                EditorGUI.PropertyField(countdownRect, property.FindPropertyRelative("s_Countdown"), GUIContent.none);

                StatField(rect, property, stackingType, affectionsType);
            }
        }

        
    }
}
