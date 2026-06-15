using UnityEngine;
using System.Collections;

public class SinglePlayerMode : MonoBehaviour
{
    public int demonModifier = 0;
    public int demonTimerAdder = 0;

    private IEnumerator Start()
    {
        while (true)
        {
            var rooms = GameState.Instance.RoomCombatPointsList;

            for (int i = 0; i < rooms.Count; i++)
            {
                CombatPoints room = rooms[i];

                room.demonPoints += demonModifier;

                rooms[i] = room;
            }
            yield return new WaitForSeconds(demonTimerAdder);
        }
    }
}
