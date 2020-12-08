using UnityEditor;
using UnityEngine;
using WarWolfWorks.EditorBase.Interfaces;
using WarWolfWorks.EditorBase.Utility;
using static WarWolfWorks.EditorBase.Constants;

namespace WarWolfWorks.EditorBase.Services
{
    /// <summary>
    /// The class which displays the WarWolfWorks settings menu.
    /// </summary>
    public sealed class ServicesWindow : EditorWindow
    {
        /// <summary>
        /// The menu currently selected.
        /// </summary>
        private int Selected = 0;

        /// <summary>
        /// All current service tabs.
        /// </summary>
        private IService[] Tabs;

        private GUIStyle TabStyle;
        private readonly string Tab_RichStart = $"<color=#{ColorUtility.ToHtmlStringRGB(new Color(.8f, .8f, .8f, 1f))}>";
        private readonly string Tab_RichEnd = "</color>";

        [MenuItem("WarWolfWorks/Settings")]
        private static void Enable()
        {
            ServicesWindow window = GetWindow<ServicesWindow>("Settings");
            window.Show();
            window.minSize = Services_Window_Size_Min;
        }

        private Color GetSelectionColor(int itteration)
        {
            return Selected == itteration ? Settings_Tab_Color_Selected : Settings_Tab_Color_Default;
        }

        private void OnEnable()
        {
            TabStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true,
                fontStyle = FontStyle.Bold,
            };
            Tabs = Settings_Tab_Menus;
            for (int i = 0; i < Tabs.Length; i++)
                Tabs[i].OnEnable();
        }

        private void OnDisable()
        {
            for (int i = 0; i < Tabs.Length; i++)
                Tabs[i].OnDisable();

            WarWolfWorks.Internal.Settings.Apply();
        }

        private void OnGUI()
        {
            Repaint();

            Rect TabsPosition = new Rect();
            TabsPosition.height = V_SERVICES_TAB_HEIGHT;
            TabsPosition.width = position.width / Settings_Tab_Menus.Length;

            EditorGUILayout.Space(TabsPosition.height);

            Rect backgroundRect = new Rect(Vector2.zero, position.size);
            backgroundRect.yMin = TabsPosition.yMax;
            EditorHooks.DrawColoredSquare(backgroundRect, Settings_Tab_Color_Selected);

            Repaint();

            for (int i = 0; i < Tabs.Length; i++)
            {
                EditorHooks.DrawColoredSquare(TabsPosition, GetSelectionColor(i));
                GUI.Label(TabsPosition, string.Format("{0}{1}{2}", Tab_RichStart, Settings_Tab_Menus[i].Name, Tab_RichEnd), TabStyle);

                if (Event.current.rawType == EventType.MouseDown && EditorHooks.EventMouseInRect(TabsPosition))
                {
                    Tabs[Selected].OnDisable();
                    WarWolfWorks.Internal.Settings.Apply();
                    Selected = i;
                }

                TabsPosition.x = TabsPosition.xMax;
                if (i != Settings_Tab_Menus.Length - 1)
                {
                    EditorHooks.DrawColoredSquare(new Rect(TabsPosition.position, new Vector2(V_SERVICES_TAB_SEPARATOR_WIDTH, TabsPosition.height)), Color.black);
                    TabsPosition.xMin += V_SERVICES_TAB_SEPARATOR_WIDTH;
                }

                if(i == Selected)
                {
                    try { Tabs[i].Draw(); } catch { }
                }
            }
        }
    }
}
