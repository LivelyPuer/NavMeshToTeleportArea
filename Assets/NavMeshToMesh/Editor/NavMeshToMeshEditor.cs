using System.Collections;
using System.Collections.Generic;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer.Operations;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEditor.AI.NavMeshBuilder;

[CustomEditor(typeof(NaveMeshToMeshGenerator))]
public class NavMeshToMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NaveMeshToMeshGenerator now = (NaveMeshToMeshGenerator) target;
        GUILayout.Label("Created by LivelyPuer https://github.com/LivelyPuer");
        now.BakeNaveMesh = GUILayout.Toggle(now.BakeNaveMesh, "Bake NavMesh", EditorStyles.toggle);
        if (GUILayout.Button("GenerateMesh"))
        {
            string curScene = EditorSceneManager.GetActiveScene().name;
            if (now.BakeNaveMesh)
            {
                NavMeshBuilder.BuildNavMesh();
            }
            NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
            Mesh mesh = new Mesh();
            mesh.vertices = triangles.vertices;
            mesh.triangles = triangles.indices;
            SaveMesh(mesh, "Mesh" + curScene, true, false);
            MeshCollider now_Mesh = now.GetComponent<MeshCollider>();
            now_Mesh.sharedMesh = mesh;
            MeshFilter nowMeshFilter = now.GetComponent<MeshFilter>();
            nowMeshFilter.mesh = mesh;
            Debug.Log("TeleportArea was created");
        }
    }

    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        Mesh meshToSave = (makeNewInstance) ? Instantiate(mesh) : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, "Assets/GeneratedNavMeshes/" + name + ".asset");
        AssetDatabase.SaveAssets();
    }
}
