using UnityEngine;
using UnityEditor;
using WarWolfWorks.CollisionSystem;

namespace WarWolfWorks.EditorBase.CollisionSystem
{
    [CustomEditor(typeof(RaycastDetector2D))]
    public class RaycastDetector2DEditor : Editor
    {
        private SerializedProperty[] Properties;

        private readonly string[] DesiredNames = new string[]
        {
            "Display Gizmos",
            "Use Fixed Update",
            "Raycast Filter",
            "Raycast Type",
            "Raycast Direction",
            "Raycast Distance",
            "Raycast Size",
            "Overlap Area Cast Point A",
            "Overlap Area Cast Point B",
            "Raycast Center"
        };

        private void OnEnable()
        {
            Properties = new SerializedProperty[]
            {
                serializedObject.FindProperty("ディスプレイギズモー"),
                serializedObject.FindProperty("ヒクセード"),
                serializedObject.FindProperty("ヒールター"),
                serializedObject.FindProperty("RD2Dタイプ"),
                serializedObject.FindProperty("レーキヤスト方面"),
                serializedObject.FindProperty("レーキヤスト距離"),
                serializedObject.FindProperty("レーキヤスト面積"),
                serializedObject.FindProperty("打ち合わせ丸キヤストポイントA"),
                serializedObject.FindProperty("打ち合わせ丸キヤストポイントB"),
                serializedObject.FindProperty("中心ポイント")
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            for (int i = 0; i < Properties.Length; i++)
            {
                EditorGUILayout.PropertyField(Properties[i], new GUIContent(DesiredNames[i]), true);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
