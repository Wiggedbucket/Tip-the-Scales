using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public enum State { Idle, Recover, Chase, Attack }
    [SerializeField] public EnemyStats stats;
    [SerializeField] public Transform player;
    public State currentState = State.Idle;
    NavMeshAgent agent;
    float currentHealth;
    public float lastAttackTime;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stats.attackRange * 0.8f;
        agent.speed = stats.moveSpeed;
        currentHealth = stats.maxHealth;    
    }
    void Update()
    {
        switch (currentState) 
        { 
            case State.Idle:
                HandleIdle();
                break;
            case State.Recover:
                HandleRecover();
                break;
            case State.Chase:
                HandleChase();
                break;
            case State.Attack:
                break;
        }
    }
    void HandleIdle()
    {
        float distance =  Vector3.Distance(transform.position, player.position);
        if (distance < 10f)
        {             
            currentState = State.Chase;
        }
    }
    void HandleChase()
    {
        agent.SetDestination(player.position);
        float distance =  Vector3.Distance(transform.position, player.position);

        float triggerRange = stats.useLunge ? stats.lungeRange : stats.attackRange;

        if (distance <= stats.lungeRange)
        {
            agent.SetDestination(transform.position);
            agent.velocity = Vector3.zero;
            currentState = State.Attack;
        }
    }
    void HandleRecover()
    {
        agent.velocity = Vector3.zero;
        if (Time.time >= lastAttackTime + stats.attackCooldown)
        {
            currentState = State.Chase;
        }
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Debug.Log(stats.enemyName + " has died.");
            Destroy(gameObject);
        }
    }
}
