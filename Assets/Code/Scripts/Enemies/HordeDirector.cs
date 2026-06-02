using UnityEngine;
using Utilities;

public class HordeDirector : MonoBehaviour
{
    public int id = 0;

    [SerializeField] private EnemySpawner enemySpawner;

    public bool isPlayerInRoom = false;

    private EventBinding<ChangeRoomStateEvent> changeRoomStateEventBinding;

    private CombatPoints combatPoints;

    public static int TotalCurrentWave = 0;

    [SerializeField] private int currentWave = 0;
    public int CurrentWave
    {
        get
        {
            // Makes sure that all the rooms don't stray too far from each other in terms of wave count
            if (currentWave + maxWaveDeviation < TotalCurrentWave)
                currentWave = TotalCurrentWave - maxWaveDeviation;
            return currentWave;
        }
    }

    private readonly int maxWaveDeviation = 5;

    private float GameTime => GameState.Instance.GameTime;

    public int baseBudget = 10;

    private void Start()
    {
        combatPoints = GameState.Instance.RoomCombatPointsList[id];
    }

    private void OnEnable()
    {
        changeRoomStateEventBinding = new EventBinding<ChangeRoomStateEvent>(ChangeRoomState);
        EventBus<ChangeRoomStateEvent>.Register(changeRoomStateEventBinding);
    }

    private void OnDisable()
    {
        EventBus<ChangeRoomStateEvent>.Deregister(changeRoomStateEventBinding);
    }

    private void ChangeRoomState(ChangeRoomStateEvent changeRoomStateEvent)
    {
        if (changeRoomStateEvent.RoomId != id)
        {
            if (changeRoomStateEvent.isPlayerInRoom)
                isPlayerInRoom = false;

            return;
        }

        isPlayerInRoom = changeRoomStateEvent.isPlayerInRoom;

        if (isPlayerInRoom)
            StartWave();
    }

    [ContextMenu("Start Wave")]
    private void StartWave()
    {
        currentWave++;
        TotalCurrentWave++;

        Debug.Log($"Current wave: {CurrentWave}; Total current wave: {TotalCurrentWave}");

        int budget = CalculateBudget();

        StartCoroutine(enemySpawner.SpawnWave(budget, CurrentWave));
    }

    public void TryStartWave()
    {
        if (!isPlayerInRoom)
            return;

        StartWave();
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
