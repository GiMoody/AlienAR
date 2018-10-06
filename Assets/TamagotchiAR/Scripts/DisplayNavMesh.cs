using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayNavMesh : MonoBehaviour {


	
	// Update is called once per frame
	void Update () {
        UnityEngine.AI.NavMeshTriangulation triangles = UnityEngine.AI.NavMesh.CalculateTriangulation();
        Mesh mesh = new Mesh
        {
            vertices = triangles.vertices,
            triangles = triangles.indices
        };
        mesh.RecalculateNormals();

        //GameObject ob = GameObject.Find("PlaneGenerator"); // nome oggetto vuoto
        //ob.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }
}
