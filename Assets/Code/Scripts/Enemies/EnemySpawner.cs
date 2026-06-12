using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private EnemyRegistry Registry => EnemyRegistryDatabase.Registry;

    [SerializeField] private HordeDirector hordeDirector;

    [SerializeField] private List<Transform> spawnPoints;

    [SerializeField] private bool allEnemiesSpawned = false;

    [SerializeField] private float spawnDelay = 0.2f;

    [SerializeField] private int maxEnemiesAlive = 500;

    private readonly HashSet<GameObject> aliveEnemies = new();

    private EventBinding<EnemyDiedEvent> enemyDiedEventBinding;

    private void OnEnable()
    {
        enemyDiedEventBinding = new EventBinding<EnemyDiedEvent>(EnemyDied);
        EventBus<EnemyDiedEvent>.Register(enemyDiedEventBinding);
    }

    private void OnDisable()
    {
        EventBus<EnemyDiedEvent>.Deregister(enemyDiedEventBinding);
    }

    public IEnumerator SpawnWave(int budget, int wave)
    {
        allEnemiesSpawned = false;

        CleanUpEnemyHashSet();

        int safety = 1000;

        Debug.Log($"Starting wave {wave} with a budget of {budget}");
        while (budget > 0 && safety-- > 0)
        {
            if (aliveEnemies.Count >= maxEnemiesAlive)
                break;

            EnemyData enemy = GetEnemyForBudget(budget, wave);

            if (enemy.enemy == null)
                break;

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            SpawnEnemy(enemy, spawnPoint);

            budget -= enemy.cost;

            if (budget > 0 && safety-- > 0)
                yield return new WaitForSeconds(spawnDelay);
        }

        allEnemiesSpawned = true;
    }

    private EnemyData GetEnemyForBudget(int budget, int wave)
    {
        List<EnemyData> validEnemies = new();

        float totalWeight = 0f;

        // Find out which enemies are valid to spawn and calculate the total weight for the weighted random selection
        foreach (EnemyData enemy in Registry.enemies)
        {
            if (enemy.enemy == null)
                continue;

            if (enemy.cost > budget)
                continue;

            if (wave < enemy.startWave)
                continue;

            if (enemy.stopSpawningWave > 0 && wave > enemy.stopSpawningWave)
                continue;

            float curveValue = EvaluateAppearanceChance(enemy, wave);

            float finalWeight = curveValue * enemy.weight;

            if (finalWeight <= 0f)
                continue;

            validEnemies.Add(enemy);

            totalWeight += finalWeight;
        }

        if (validEnemies.Count == 0)
            return default;

        float random = Random.Range(0f, totalWeight);

        float current = 0f;

        // Select an enemy based on the weighted random value
        foreach (EnemyData enemy in validEnemies)
        {
            float curveValue = EvaluateAppearanceChance(enemy, wave);

            float finalWeight = curveValue * enemy.weight;

            current += finalWeight;

            if (random <= current)
                return enemy;
        }

        return validEnemies[0];
    }

    private float EvaluateAppearanceChance(EnemyData enemy, int wave)
    {
        if (wave < enemy.startWave)
            return 0f;

        int curveLength = enemy.curveEndWave - enemy.startWave;

        if (curveLength <= 0)
            return enemy.appearanceChance.Evaluate(1f);

        float normalizedProgress = Mathf.Clamp01((float)(wave - enemy.startWave) / curveLength);

        return enemy.appearanceChance.Evaluate(normalizedProgress);
    }

    private void SpawnEnemy(EnemyData enemyData, Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyData.enemy, spawnPoint.position, spawnPoint.rotation, transform);
        if (enemy.TryGetComponent<EnemyFSM>(out var enemyFSM))
        {
            enemyFSM.roomId = hordeDirector.id;
        }

        aliveEnemies.Add(enemy);
    }

    private void EnemyDied(EnemyDiedEvent enemyDiedEvent)
    {
        aliveEnemies.Remove(enemyDiedEvent.EnemyObject);
        if (aliveEnemies.Count == 0 && allEnemiesSpawned)
            EventBus<AllEnemiesDeadEvent>.Raise(new AllEnemiesDeadEvent()
            {
                IsFinalWave = false
            });
    }

    [ContextMenu("Kill All Enemies")]
    private void KillAllEnemies()
    {
        EventBus<KillAllEnemiesEvent>.Raise(new KillAllEnemiesEvent()
        {
            RoomId = hordeDirector.id
        });
    }

    private void CleanUpEnemyHashSet()
    {
        aliveEnemies.RemoveWhere(e => e == null);
    }
}