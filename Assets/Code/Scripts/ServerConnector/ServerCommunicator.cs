using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class ServerCommunicator : MonoBehaviour
{
    private const int RequiredRooms = 3;

    [Header("Server Settings")]
    [SerializeField] private string url = "http://145.93.81.29:8080";
    [SerializeField] private string clientId = "unityClient1";
    [SerializeField] private float heartbeatInterval = 5f;

    private bool IsMultiplayer => GameMode.IsMultiplayer;

    /// <summary>
    /// Creates an empty GameData object with 3 initialized objectives.
    /// This is used as the base object before filling it with room data.
    /// </summary>
    private static GameData CreateEmptyGameData()
    {
        return new GameData
        {
            score = 0,
            objectives = RequiredRooms,
            obj1 = new Objective(),
            obj2 = new Objective(),
            obj3 = new Objective(),
        };
    }

    /// <summary>
    /// Copies combat points from a room into an objective that can be sent to the server.
    /// </summary>
    private static void CopyRoomToObjective(CombatPoints room, Objective objective)
    {
        objective.light = room.angelPoints;
        objective.dark = room.demonPoints;
    }

    /// <summary>
    /// Copies objective data received from the server back into a room.
    /// </summary>
    private static void CopyObjectiveToRoom(Objective objective, CombatPoints room)
    {
        room.angelPoints = objective.light;
        room.demonPoints = objective.dark;
    }

    /// <summary>
    /// Initializes the server connection when the game starts.
    /// Resets the server state, starts the heartbeat loop,
    /// and sends an initial empty game state.
    /// </summary>
    private IEnumerator Start()
    {
        if (!IsMultiplayer)
        {
            Debug.Log("Server off, Singleplayer On");
            yield break;
        }

        Debug.Log("Server on, Singleplayer Off");

        // Reset the server before starting synchronization.
        yield return SendResetRequest();

        // Begin periodically sending updates.
        StartCoroutine(HeartbeatLoop());

        // Small delay to ensure everything has initialized.
        yield return new WaitForSeconds(1f);

        // Send a fresh empty game state.
        yield return SendGameData(CreateEmptyGameData());
    }

    /// <summary>
    /// Periodically sends the current game state to the server.
    /// </summary>
    private IEnumerator HeartbeatLoop()
    {
        while (true)
        {
            yield return SendCurrentGameData();
            yield return new WaitForSeconds(heartbeatInterval);
        }
    }

    /// <summary>
    /// Builds a GameData object from the current room combat points
    /// and sends it to the server.
    /// </summary>
    private IEnumerator SendCurrentGameData()
    {
        var rooms = GameState.Instance.RoomCombatPointsList;

        if (rooms == null || rooms.Count < RequiredRooms)
        {
            Debug.LogError("Not enough rooms for sync");
            yield break;
        }

        GameData data = CreateEmptyGameData();

        // Copy all room data into the server data structure.
        for (int i = 0; i < RequiredRooms; i++)
        {
            CopyRoomToObjective(rooms[i], GetObjective(data, i));
        }

        yield return SendGameData(data);
    }

    /// <summary>
    /// Returns one of the objectives based on an index.
    /// Used to avoid repetitive code when looping through rooms.
    /// </summary>
    private Objective GetObjective(GameData data, int index)
    {
        return index switch
        {
            0 => data.obj1,
            1 => data.obj2,
            2 => data.obj3,
            _ => null,
        };
    }

    /// <summary>
    /// Sends a reset request to the server so it clears its current state.
    /// </summary>
    private IEnumerator SendResetRequest()
    {
        ResetRequest reset = new()
        {
            objectives = RequiredRooms
        };

        yield return SendRequest(
            $"{url}/reset/{clientId}",
            JsonUtility.ToJson(reset),
            response => Debug.Log("Reset Response: " + response)
        );
    }

    /// <summary>
    /// Sends game data to the server and applies the returned data
    /// back into the local GameState.
    /// </summary>
    private IEnumerator SendGameData(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);

        yield return SendRequest(
            $"{url}/{clientId}",
            json,
            response =>
            {
                Debug.Log("Server Response: " + response);

                // Deserialize the server response.
                GameData updated = JsonUtility.FromJson<GameData>(response);

                if (updated != null)
                {
                    Debug.Log("New Score: " + updated.score);

                    // Update local room values with the server's response.
                    ApplyToGameStateData(updated);
                }
            }
        );
    }

    /// <summary>
    /// Generic helper used for all POST requests.
    /// Handles serialization, sending, error checking,
    /// and invoking a success callback.
    /// </summary>
    private IEnumerator SendRequest(string endpoint, string json, System.Action<string> onSuccess)
    {
        byte[] body = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new(endpoint, "POST")
        {
            uploadHandler = new UploadHandlerRaw(body),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Request Failed: " + request.error);
            yield break;
        }

        onSuccess?.Invoke(request.downloadHandler.text);
    }

    /// <summary>
    /// Updates the local GameState with data received from the server.
    /// </summary>
    private void ApplyToGameStateData(GameData data)
    {
        var state = GameState.Instance;

        if (state == null)
        {
            Debug.LogError("GameState is null");
            return;
        }

        var rooms = state.RoomCombatPointsList;

        // Ensure the list contains enough rooms.
        while (rooms.Count < RequiredRooms)
        {
            rooms.Add(new CombatPoints());
        }

        // Copy each objective into its corresponding room.
        CopyObjectiveToRoom(data.obj1, rooms[0]);
        CopyObjectiveToRoom(data.obj2, rooms[1]);
        CopyObjectiveToRoom(data.obj3, rooms[2]);
    }
}

[System.Serializable]
public class Objective
{
    public int light;
    public int dark;
}

[System.Serializable]
public class GameData
{
    public float score;
    public int objectives;

    public Objective obj1;
    public Objective obj2;
    public Objective obj3;
}

[System.Serializable]
public class ResetRequest
{
    public int objectives;
}