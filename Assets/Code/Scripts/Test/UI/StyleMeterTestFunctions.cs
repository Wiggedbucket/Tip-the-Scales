using UnityEngine;

public class StyleMeterTestFunctions : MonoBehaviour
{
    [SerializeField]
    private int stylePoints = 50;

    [ContextMenu("Add Style Points")]
    private void AddStylePoints()
    {
        //Debug.Log("Raising StyleGainEvent");

        EventBus<StyleGainEvent>.Raise(new StyleGainEvent()
        {
            Amount = stylePoints,
            Reason = "TEST",
            TextColor = Color.orange,
        });
    }
}
