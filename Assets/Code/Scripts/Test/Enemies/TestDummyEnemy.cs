using UnityEngine;
using UnityEngine.AI;

public class TestDummyEnemy : MonoBehaviour
{
    private NavMeshAgent agent;

    private EventBinding<KillAllEnemiesEvent> killAllEnemiesEventBinding;

    private void OnEnable()
    {
        killAllEnemiesEventBinding = new EventBinding<KillAllEnemiesEvent>(Die);
        EventBus<KillAllEnemiesEvent>.Register(killAllEnemiesEventBinding);
    }

    private void OnDisable()
    {
        EventBus<KillAllEnemiesEvent>.Deregister(killAllEnemiesEventBinding);
    }

    private void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(Vector3.zero);
    }

    private void Die(KillAllEnemiesEvent killAllEnemiesEvent)
    {
        EventBus<EnemyDiedEvent>.Raise(new EnemyDiedEvent()
        {
            enemyObject = gameObject
        });
        Destroy(gameObject);
    }
}
