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
    private bool mockDataStart = false;


    IEnumerator Start()
    {
        yield return StartCoroutine(ResetGame());
        StartCoroutine(HeartbeatLoop());
        yield return new WaitForSeconds(1);
        GameData gameData = new GameData();

        gameData.score = 0;
        gameData.objectives = 3;

        gameData.obj1 = new Objective();
        gameData.obj2 = new Objective();
        gameData.obj3 = new Objective();

        gameData.obj1.light = 7;
        gameData.obj1.dark = 0;

        gameData.obj2.light = 7;
        gameData.obj2.dark = 0;

        gameData.obj3.light = 2;
        gameData.obj3.dark = 1;

        yield return StartCoroutine(
            SendGameData(gameData)
        );
        Debug.Log("Reached after reset");

    }
    IEnumerator HeartbeatLoop()
    {
        while (true)
        {
            yield return StartCoroutine(SendCurrentGameData());
            yield return new WaitForSeconds(heartbeatInterval);
        }
    }

    IEnumerator SendCurrentGameData()
    {
        GameData currentData = new GameData();
        currentData.objectives = 3;
        currentData.obj1 = new Objective();
        currentData.obj2 = new Objective();
        currentData.obj3 = new Objective();
        var list = GameState.Instance.RoomCombatPointsList;
        for (int i = 0; i < list.Count; i++)
        {
            CombatPoints room = list[i];
            Debug.Log("Room " + i + " Angel: " + room.angelPoints + " Demon: " + room.demonPoints);
        }
        CombatPoints room0 = list[0];
        CombatPoints room1 = list[1];
        CombatPoints room2 = list[2];

        currentData.obj1.light = room0.angelPoints;
        currentData.obj1.dark = room0.demonPoints;

        currentData.obj2.light = room1.angelPoints;
        currentData.obj2.dark = room1.demonPoints;

        currentData.obj3.light = room2.angelPoints;
        currentData.obj3.dark = room2.demonPoints;
        yield return StartCoroutine(
            SendGameData(currentData)
        );
    }

    IEnumerator ResetGame()
    {
        ResetRequest resetData =
            new ResetRequest();

        resetData.objectives = 3;

        string json =
            JsonUtility.ToJson(resetData);

        byte[] bodyRaw =
            Encoding.UTF8.GetBytes(json);

        UnityWebRequest request =
            new UnityWebRequest(url + "/reset/" + clientId,"POST");

        request.uploadHandler =
            new UploadHandlerRaw(bodyRaw);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                "Reset Failed: " +
                request.error
            );
        }
        else
        {
            Debug.Log(
                "Reset Response: " +
                request.downloadHandler.text
            );
        }
    }

    IEnumerator MockScoreDarkIncrease()
    {
        Debug.Log("Offline Mode Started");
        while (!serverConnected)
        {
            if (GameState.Instance == null)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            var rooms = GameState.Instance.RoomCombatPointsList;
            for (int i = 0; i < rooms.Count; i++)
            {
                CombatPoints room = rooms[i];

                room.demonPoints += Random.Range(1, 6);

                rooms[i] = room;
            }
            yield return new WaitForSeconds(3f);
        }
        Debug.Log("Offline Mode Disabled");
        mockDataStart = false;
    }

    IEnumerator SendGameData(GameData gameData)
    {
        string json =
            JsonUtility.ToJson(gameData);

        Debug.Log(
            "Sending: " + json
        );

        byte[] bodyRaw =
            Encoding.UTF8.GetBytes(json);

        UnityWebRequest request =
            new UnityWebRequest(
                url + "/" + clientId,
                "POST"
            );

        request.uploadHandler =
            new UploadHandlerRaw(bodyRaw);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            serverConnected = false;
            if(!mockDataStart)
            {
                StartCoroutine(MockScoreDarkIncrease());
                mockDataStart = true;
            }
            Debug.LogError(
                "POST Failed: " +
                request.error
            );
        }
        else
        {
            serverConnected = true;
            Debug.Log(
                "Server Response: " +
                request.downloadHandler.text
            );

            GameData updatedData =
                JsonUtility.FromJson<GameData>(
                    request.downloadHandler.text
                );

            Debug.Log("New Score: " + updatedData.score);
            ApplyToGameStateData(updatedData);
        }
    }
    private void ApplyToGameStateData(GameData data)
    {
        var list = GameState.Instance.RoomCombatPointsList;

        CombatPoints room0 = list[0];
        room0.angelPoints = data.obj1.light;
        room0.demonPoints = data.obj1.dark;
        list[0] = room0;

        CombatPoints room1 = list[1];
        room1.angelPoints = data.obj2.light;
        room1.demonPoints = data.obj2.dark;
        list[1] = room1;

        CombatPoints room2 = list[2];
        room2.angelPoints = data.obj3.light;
        room2.demonPoints = data.obj3.dark;
        list[2] = room2;

    }
}