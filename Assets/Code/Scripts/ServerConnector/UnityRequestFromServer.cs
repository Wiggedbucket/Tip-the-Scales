using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

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

public class UnityRequestFromServer : MonoBehaviour
{
    private string url = "http://localhost:8080";
    private string clientId = "unityClient1";
    private float heartbeatInterval = 5f;
    private bool serverConnected = false;

    public GameObject Room1;
    public GameObject Room2;
    public GameObject Room3;

    private IEnumerator Start()
    {
        yield return StartCoroutine(ResetGame());

        StartCoroutine(HeartbeatLoop());

        yield return new WaitForSeconds(1);

        GameData gameData = new()
        {
            score = 0,
            objectives = 3,

            obj1 = new Objective(),
            obj2 = new Objective(),
            obj3 = new Objective()
        };

        gameData.obj1.light = 0;
        gameData.obj1.dark = 0;

        gameData.obj2.light = 0;
        gameData.obj2.dark = 0;

        gameData.obj3.light = 0;
        gameData.obj3.dark = 0;

        yield return StartCoroutine(SendGameData(gameData));
        //Debug.Log("Reached after reset");
    }

    private IEnumerator HeartbeatLoop()
    {
        while (true)
        {
            yield return StartCoroutine(SendCurrentGameData());
            yield return new WaitForSeconds(heartbeatInterval);
        }
    }

    private IEnumerator SendCurrentGameData()
    {
        GameData currentData = new()
        {
            objectives = 3,
            obj1 = new Objective(),
            obj2 = new Objective(),
            obj3 = new Objective(),
        };

        RoomData room1 = Room1.GetComponent<RoomData>();
        RoomData room2 = Room2.GetComponent<RoomData>();
        RoomData room3 = Room3.GetComponent<RoomData>();

        currentData.obj1.light = room1.angelPoints;
        currentData.obj1.dark = room1.demonPoints;

        currentData.obj2.light = room2.angelPoints;
        currentData.obj2.dark = room2.demonPoints;

        currentData.obj3.light = room3.angelPoints;
        currentData.obj3.dark = room3.demonPoints;

        yield return StartCoroutine(SendGameData(currentData));
    }

    IEnumerator ResetGame()
    {
        ResetRequest resetData = new()
        {
            objectives = 3
        };

        string json = JsonUtility.ToJson(resetData);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new(url + "/reset/" + clientId, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Reset Failed: " + request.error);
        }
        else
        {
            Debug.Log("Reset Response: " + request.downloadHandler.text);
        }
    }

    private IEnumerator SendGameData(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);

        Debug.Log("Sending: " + json);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new(url + "/" + clientId, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bodyRaw),

            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("POST Failed: " + request.error);
        }
        else
        {
            Debug.Log("Server Response: " + request.downloadHandler.text);

            GameData updatedData = JsonUtility.FromJson<GameData>(request.downloadHandler.text);

            Debug.Log("New Score: " + updatedData.score);
            ApplyToGameStateData(updatedData);
        }
    }
    private void ApplyToGameStateData(GameData data)
    {
        var rooms = GameState.Instance.RoomCombatPointsList;
        CombatPoints room0 = rooms[0];
        room0.angelPoints = data.obj1.light;
        room0.demonPoints = data.obj1.dark;
        rooms[0] = room0;

        CombatPoints room1 = rooms[1];
        room1.angelPoints = data.obj2.light;
        room1.demonPoints = data.obj2.dark;
        rooms[1] = room1;
        
        CombatPoints room2 = rooms[2];
        room2.angelPoints = data.obj3.light;
        room2.demonPoints = data.obj3.dark;
        rooms[3] = room2;
    }
}