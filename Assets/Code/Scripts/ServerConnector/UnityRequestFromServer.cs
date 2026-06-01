using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class GameData
{
    public int LightScore;
    public int DarkScore;
}
public class UnityRequestFromServer : MonoBehaviour
{
    IEnumerator Start()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:8080/getGameData");
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            string jsonResponse = www.downloadHandler.text;
            GameData gameData = JsonUtility.FromJson<GameData>(jsonResponse);
            Debug.Log("Light Score: " + gameData.LightScore);
            Debug.Log("Dark Score: " + gameData.DarkScore);
        }
    }
}
