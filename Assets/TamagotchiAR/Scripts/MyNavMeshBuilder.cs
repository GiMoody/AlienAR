using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using System;

/// <summary>
/// Versione modificata del NavMeshBuilder fornito da unity che ricalcola la navmesh utilizzando una coroutine e non a ogni update
/// </summary>
public class MyNavMeshBuilder : MonoBehaviour {
    /// <summary>
    /// Frequenza di aggiornamento della NavMesh (in secondi)
    /// </summary>
    public float NavMeshSinchFrequency=3.0f;

    /// <summary>
    /// Tempo di attesa per il ricalcolo della NavMesh nel caso il sistema non abbia ancora tutti i prefab pronti
    /// </summary>
    public float SyncWaitTime=1.0f; 

    // The center of the build
    public Transform m_Tracked;

    // The size of the build bounds
    public Vector3 m_Size = new Vector3(80.0f, 20.0f, 80.0f);

    NavMeshData m_NavMesh;
    AsyncOperation m_Operation;
    NavMeshDataInstance m_Instance;
    List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();

    private Coroutine _UpdateNavMeshCoroutine;
    void OnEnable()
    {
        // Construct and add navmesh
        m_NavMesh = new NavMeshData();
        m_Instance = NavMesh.AddNavMeshData(m_NavMesh);

        if (m_Tracked == null)
            m_Tracked = transform;
        UpdateNavMesh(false);
        Debug.Log("NavMesh created");
        _UpdateNavMeshCoroutine = StartCoroutine(UpdateNavMeshCoroutine());
    }


    private IEnumerator UpdateNavMeshCoroutine()
    {
        while (true)
        {
            if (GameManager.instance.CurrentGameStatus != (int)GameManager.GameStatus.Build)
            {
                UpdateNavMesh(true);
                GameManager.instance.isNavMeshReady = true;
             //   Debug.Log("NavMesh updated");
                yield return new WaitForSecondsRealtime(NavMeshSinchFrequency);
            }
            else
                yield return new WaitForSecondsRealtime(SyncWaitTime);
            
        }
    }

    void UpdateNavMesh(bool asyncUpdate = false)
    {
        NavMeshSourceTag.Collect(ref m_Sources);
        var defaultBuildSettings = NavMesh.GetSettingsByID(0);
        var bounds = QuantizedBounds();

        if (asyncUpdate)
            m_Operation = NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
        else
            NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, m_Sources, bounds);


    }

    static Vector3 Quantize(Vector3 v, Vector3 quant)
    {
        float x = quant.x * Mathf.Floor(v.x / quant.x);
        float y = quant.y * Mathf.Floor(v.y / quant.y);
        float z = quant.z * Mathf.Floor(v.z / quant.z);
        return new Vector3(x, y, z);
    }

    Bounds QuantizedBounds()
    {
        // Quantize the bounds to update only when theres a 10% change in size
        var center = m_Tracked ? m_Tracked.position : transform.position;
        return new Bounds(Quantize(center, 0.1f * m_Size), m_Size);
    }


}
