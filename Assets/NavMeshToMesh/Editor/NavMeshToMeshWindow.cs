using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;
using NavMeshBuilder = UnityEditor.AI.NavMeshBuilder;

public class NavMeshToMeshWindow : EditorWindow
{

    private bool teleportAreaLocked;
    private bool teleportAreaMarkerActive = true;
    private bool bakeNavMesh;
    [MenuItem("Window/NaveMesh to Mesh")]
    public static void ShowWindow()
    {
        GetWindow<NavMeshToMeshWindow>("NavMesh To Mesh");
        Texture2D t = EditorGUIUtility.Load("Icons/UnityEditor.DebugInspectorWindow.png") as Texture2D;

    }

    void OnGUI()
    {
        GUILayout.Label("NameMesh to TeleportArea", EditorStyles.boldLabel);
        GUILayout.Label("Tool for generate TeleportArea for SteamVR 2.x");
        GUILayout.Space(10);
        GUILayout.Label("Teleport option");
        teleportAreaLocked = EditorGUILayout.Toggle("TeleportArea Locked", teleportAreaLocked, EditorStyles.toggle);
        teleportAreaMarkerActive = EditorGUILayout.Toggle("TeleportArea Marker Active", teleportAreaMarkerActive, EditorStyles.toggle);
        GUILayout.Space(10);
        GUILayout.Label("NaveMesh option");
        bakeNavMesh = EditorGUILayout.Toggle("BakeNaveMesh", bakeNavMesh, EditorStyles.toggle);
        if (GUILayout.Button("Generate"))
        {
            GenerateNavMesh();
        }

        GUILayout.Label("Created by LivelyPuer");
        GUILayout.Label("GitHub: https://github.com/LivelyPuer");
        GUILayout.Label("date: 03.11.2021");
    }

    void GenerateNavMesh()
    {
        string curScene = EditorSceneManager.GetActiveScene().name;
        if (bakeNavMesh)
        {
            NavMeshBuilder.BuildNavMesh();
        }

        NavMeshTriangulation triangles = NavMesh.CalculateTriangulation ();
        Mesh mesh = new Mesh ();
        mesh.vertices = triangles.vertices;
        mesh.triangles = triangles.indices;
        SaveMesh(mesh, "Mesh"+curScene, true, false);
        Teleport t = Teleport.instance;
        if (!FindObjectOfType(typeof(Teleport)))
        {
            GameObject tmp_t = Instantiate((GameObject)EditorGUIUtility.Load("TeleportingEditor.prefab"), Vector3.one, Quaternion.identity);
            tmp_t.name = "Teleporting";
            t = tmp_t.GetComponent<Teleport>();
        }
        GameObject now = new GameObject();
        now.name = "Mesh" + curScene;
        MeshCollider now_Mesh = now.AddComponent<MeshCollider>();
        now_Mesh.sharedMesh = mesh;
        MeshRenderer nowMeshRender = now.AddComponent<MeshRenderer>();
        nowMeshRender.sharedMaterial = t.areaVisibleMaterial;
        MeshFilter nowMeshFilter = now.AddComponent<MeshFilter>();
        nowMeshFilter.mesh = mesh;
        
        TeleportArea nowMeshTeleportArea = now.AddComponent<TeleportArea>();
        nowMeshTeleportArea.locked = teleportAreaLocked;
        nowMeshTeleportArea.markerActive = teleportAreaMarkerActive;
        now.AddComponent<NaveMeshToMeshGenerator>();
        Debug.Log("TeleportArea was created");

    }
    
    public static void SaveMesh (Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh) {
        Mesh meshToSave = (makeNewInstance) ? Instantiate(mesh) : mesh;
		
        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);
        
        AssetDatabase.CreateAsset(meshToSave, "Assets/GeneratedNavMeshes/" + name + ".asset");
        AssetDatabase.SaveAssets();
    }
}
