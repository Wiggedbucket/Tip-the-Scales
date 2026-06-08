using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public int roomId = -1;

    [SerializeField] Material basecolour;
    [SerializeField] Renderer rend;

    public enum State { Idle, Recover, Chase, Attack, Kite }

    [SerializeField] public EnemyStats stats;
    public Transform player;

    public float lastKiteEndTime;
    public State currentState = State.Idle;

    NavMeshAgent agent;

    float currentHealth;
    public float lastAttackTime;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player object not found in the scene. Please ensure there is a GameObject tagged 'Player'.");
        }

        agent = GetComponent<NavMeshAgent>();
        agent.speed = stats.moveSpeed;
        currentHealth = stats.maxHealth;

        if (stats.canKite)
            agent.stoppingDistance = stats.fireRange * 0.8f;
        else if (stats.useLunge)
            agent.stoppingDistance = stats.lungeRange * 0.8f;
        else
            agent.stoppingDistance = stats.attackRange * 0.8f;
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
                
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                directionToPlayer.y = 0f;
                if (directionToPlayer != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(directionToPlayer);

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


        if (distance <= triggerRange)
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
        rend.material = basecolour;
    }

    void HandleKite()
    {
        agent.stoppingDistance = 0f;
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 kiteTarget = transform.position + directionAwayFromPlayer * 5f;
        agent.SetDestination(kiteTarget);
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance >= stats.kiteDistance * 2f)
        {
            agent.stoppingDistance = stats.fireRange * 0.8f;
            currentState = State.Attack;
            lastKiteEndTime = Time.time;
            
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
