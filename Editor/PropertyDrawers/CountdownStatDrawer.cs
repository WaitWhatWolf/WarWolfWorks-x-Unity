﻿using UnityEditor;
using UnityEngine;
using WarWolfWorks.EntitiesSystem.Statistics;

namespace WarWolfWorks.EditorBase.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(CountdownStat))]
    public class CountdownStatDrawer : StatDrawer
    {
        protected override float WidthDivider => 4;

        protected override void GUIDraw(ref Rect position, SerializedProperty property, GUIContent label)
        {
            base.GUIDraw(ref position, property, label);
            DrawValue(position, property.FindPropertyRelative("countdown"),
                position.x + FinalWidth(position) * 3, "Countdown", out _);
        }
    }
}
