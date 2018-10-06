using UnityEngine;
using UnityEngine.AI;

// Walk to a random position and repeat
[RequireComponent(typeof(NavMeshAgent))]
public class RandomWalk : MonoBehaviour
{
    public float m_Range = 25.0f;
    NavMeshAgent _navAgent;

    void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (_navAgent.pathPending || _navAgent.remainingDistance > 0.1f)
            return;

        _navAgent.destination = m_Range * Random.insideUnitCircle;
    }
}
