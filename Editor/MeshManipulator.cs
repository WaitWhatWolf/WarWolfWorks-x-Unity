using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace WarWolfWorks.EditorBase
{
    class MeshManipulator : EditorWindow
    {
        public const string WindowName = "Mesh Resizer";

        private Mesh held { get; set; }

        private bool pop, pop2, pop3;
        private bool expand1, expand2, expand3;

        private Vector3 finalSize;
        private Vector3 finalOffset;
        private Vector3 finalRotation;
        private bool recalculateNormals;

        [MenuItem("WarWolfWorks/Mesh Resizer")]
        public static void ShowWindow()
        {
            MeshManipulator window = GetWindow<MeshManipulator>(WindowName);
            window.Show();
            window.minSize = new Vector2(250f, 350f);
            window.maxSize = new Vector2(350f, 450f);
            window.position = new Rect(1200f, 300f, 350f, 500f);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("WARNING: This overrides the original values of the mesh, once you apply these changes they cannot be undone.", MessageType.Warning);
            EditorGUILayout.Space();
            held = (Mesh)EditorGUILayout.ObjectField(held, typeof(Mesh), true);
            recalculateNormals = EditorGUILayout.Toggle("Recalculate Normals", recalculateNormals);
            expand1 = EditorGUILayout.Foldout(expand1, "Resize");
            if (expand1)
            {
                
                finalSize = EditorGUILayout.Vector3Field("Size Scale", finalSize);
                pop = GUILayout.Button("Resize!");

                if (pop)
                {
                    Vector3[] baseVert = held.vertices;

                    var vertices = new Vector3[baseVert.Length];

                    for (var i = 0; i < vertices.Length; i++)
                    {
                        var vertex = baseVert[i];
                        vertex.x *= finalSize.x;
                        vertex.y *= finalSize.y;
                        vertex.z *= finalSize.z;

                        vertices[i] = vertex;
                    }

                    held.vertices = vertices;

                    if (recalculateNormals)
                        held.RecalculateNormals();

                    held.RecalculateBounds();
                }
            }

            expand2 = EditorGUILayout.Foldout(expand2, "Offset");
            if (expand2)
            {
                finalOffset = EditorGUILayout.Vector3Field("Offset", finalOffset);
                pop2 = GUILayout.Button("Offset!");

                if(pop2)
                {
                    Vector3[] baseVert = held.vertices;

                    Vector3[] vertices = new Vector3[baseVert.Length];

                    for (var i = 0; i < vertices.Length; i++)
                    {
                        Vector3 vertex = baseVert[i];
                        vertex += finalOffset;

                        vertices[i] = vertex;
                    }

                    held.vertices = vertices;

                    if (recalculateNormals)
                        held.RecalculateNormals();

                    held.RecalculateBounds();
                }
            }

            expand3 = EditorGUILayout.Foldout(expand3, "Rotate");
            if(expand3)
            {
                GUILayout.Label("OMEGALUL (fuckoff too hard)");
                /*finalRotation = EditorGUILayout.Vector3Field("Final Rotation", finalRotation);
                pop3 = GUILayout.Button("Rotate!");
                if (pop3)
                {
                    
                    Vector3[] baseVert = held.vertices;
                    Vector3[] vertices = new Vector3[baseVert.Length];

                    for (int vert = 0; vert < vertices.Length; vert++)
                    {
                        Quaternion use = Quaternion.AngleAxis(finalOffset.y, Vector3.up);
                        Vector3 vertex = use * baseVert[vert];
                        vertices[vert] = vertex;
                    }
                    
                    held.vertices = vertices;

                    if (recalculateNormals)
                        held.RecalculateNormals();

                    held.RecalculateBounds();
            }*/
            }
        }
    }
}
