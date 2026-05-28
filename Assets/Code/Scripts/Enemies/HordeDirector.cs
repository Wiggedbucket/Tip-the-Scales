using UnityEngine;
using Utilities;

public class HordeDirector : MonoBehaviour
{
    public int id = 0;

    [SerializeField] private EnemySpawner enemySpawner;

    private CombatPoints combatPoints;

    public static int TotalCurrentWave = 0;

    private int currentWave = 0;
    public int CurrentWave
    {
        get
        {
            // Makes sure that all the rooms don't stray too far from each other in terms of wave count
            if (currentWave + maxWaveDeviation < TotalCurrentWave)
                currentWave = TotalCurrentWave - maxWaveDeviation;
            return currentWave;
        }
        set
        {
            currentWave = value;
            TotalCurrentWave += currentWave - value;
        }
    }

    private int maxWaveDeviation = 5;

    private float GameTime => GameState.Instance.GameTime;

    public int baseBudget = 10;

    private CountdownTimer waveCountdownTimer;

    [Tooltip("The amount of time it takes for the next wave to start, gets ignored for first wave.")]
    private float waveTimeInterval = 20f;

    private void Start()
    {
        combatPoints = GameState.Instance.RoomCombatPointsList[id];

        waveCountdownTimer = new CountdownTimer(waveTimeInterval);
        waveCountdownTimer.OnTimerStop += StartWave;
        waveCountdownTimer.Start();
    }

    private void Update()
    {
        waveCountdownTimer.Tick(Time.deltaTime);
    }

    private void StartWave()
    {
        TotalCurrentWave++;

        int budget = CalculateBudget();

        StartCoroutine(enemySpawner.SpawnWave(budget, CurrentWave));

        waveCountdownTimer.Reset(waveTimeInterval);
        waveCountdownTimer.Start();
    }

    private int CalculateBudget()
    {
        int timeScaling = Mathf.FloorToInt(GameTime / 60f);

        int waveScaling = CurrentWave * 2;

        int scoreDifference = Mathf.Abs(combatPoints.demonPoints - combatPoints.angelPoints);

        int pressureScaling = Mathf.Clamp(scoreDifference / 10, 0, 10);

        return baseBudget
               + timeScaling
               + waveScaling
               + pressureScaling;
    }
}
