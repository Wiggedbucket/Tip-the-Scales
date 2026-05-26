using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    #region Singleton Setup
    public static GameState Instance { get; private set; }
    public static bool InstanceExists => Instance != null;

    void Awake()
    {
        if (InstanceExists)
        {
            Destroy(gameObject);
        }
        else
        {
            // Registers the first valid instance before the rest of the scene startup flow.
            Instance = this;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    public float Scale = 0f;

    public CombatPoints GlobalCombatPoints;
    public List<CombatPoints> RoomCombatPointsList;
}

[System.Serializable]
public struct CombatPoints
{
    public int angelPoints;
    public int demonPoints;
}