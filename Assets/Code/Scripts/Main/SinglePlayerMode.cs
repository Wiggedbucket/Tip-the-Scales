using UnityEngine;
using System.Collections;

public class SinglePlayerMode : MonoBehaviour
{
    public GameObject Room1;
    public GameObject Room2;
    public GameObject Room3;

    private IEnumerator Start()
    {
        while (true)
        {
            Room1.GetComponent<RoomData>().demonPoints += 1;
            Room2.GetComponent<RoomData>().demonPoints += 1;
            Room3.GetComponent<RoomData>().demonPoints += 1;
            yield return new WaitForSeconds(5f);
        }
    }
}
