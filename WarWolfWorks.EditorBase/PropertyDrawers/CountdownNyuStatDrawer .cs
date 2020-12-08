using UnityEditor;
using UnityEngine;
using WarWolfWorks.NyuEntities.Statistics;
using static WarWolfWorks.EditorBase.Constants;

namespace WarWolfWorks.EditorBase.PropertyDrawers
{
    /// <summary>
    /// Custom property drawer for <see cref="CountdownStat"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(CountdownStat))]
    internal sealed class CountdownNyuStatDrawer : NyuStatDrawer
    {
        protected override float WidthDivider => 4;

        protected override void GUIDraw(ref Rect position, SerializedProperty property, GUIContent label)
        {
            base.GUIDraw(ref position, property, label);
            DrawValue(position, property.FindPropertyRelative("s_Countdown"),
                position.x + FinalWidth(position) * 3, ELS_NyuStat_Countdown, out _);
        }
    }
}
