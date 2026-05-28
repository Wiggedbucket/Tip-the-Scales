using UnityEngine;
using UnityEngine.AI;

public class MoveToCenter : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(Vector3.zero);
    }
}
