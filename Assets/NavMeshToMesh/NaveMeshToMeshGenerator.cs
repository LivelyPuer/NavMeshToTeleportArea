using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(TeleportArea))]
public class NaveMeshToMeshGenerator : MonoBehaviour
{
    public bool BakeNaveMesh;
}
