using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public enum State { Idle, Recover, Chase, Attack, Kite }
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
                if (stats.canKite)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                    if (distanceToPlayer <= stats.kiteDistance)
                    {
                        currentState = State.Kite;
                    }
                }
                break;
            case State.Kite:
                HandleKite();
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

        float triggerRange = stats.canKite ? stats.fireRange :
                             (stats.useLunge ? stats.lungeRange : stats.attackRange);


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

    void HandleKite()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 kiteTarget = transform.position + directionAwayFromPlayer * 5f;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance >= stats.kiteDistance * 2f)
        {
            currentState = State.Attack;
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
