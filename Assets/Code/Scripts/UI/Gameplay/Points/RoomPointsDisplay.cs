using UnityEngine;
using UnityEngine.UIElements;

public class RoomPointsDisplay : MonoBehaviour
{
    [SerializeField]
    private int roomId = -1;

    [SerializeField]
    private UIDocument document;

    private VisualElement balanceDemonicBarFill;
    private VisualElement balanceAngelicBarFill;

    private Label angelCombatPointsLabel;
    private Label seperatorLabel;
    private Label demonCombatPointsLabel;

    [SerializeField]
    private Color angelLabelColor = Color.yellow;
    [SerializeField]
    private Color demonLabelColor = Color.red;

    private void Awake()
    {
        var root = document.rootVisualElement;

        balanceDemonicBarFill = root.Q<VisualElement>("BalanceDemonicBarFill");
        balanceAngelicBarFill = root.Q<VisualElement>("BalanceAngelicBarFill");

        angelCombatPointsLabel = root.Q<Label>("AngelCombatPointsLabel");
        seperatorLabel = root.Q<Label>("SeperatorLabel");
        demonCombatPointsLabel = root.Q<Label>("DemonCombatPointsLabel");
    }

    private void Update()
    {
        UpdateProgressBar();
        UpdateLabels();
    }

    private void UpdateProgressBar()
    {
        CombatPoints combatPoints = GameState.Instance.RoomCombatPointsList[roomId];
        float scale = Mathf.Clamp(GameState.Instance.CalculateScale(combatPoints.angelPoints, combatPoints.demonPoints), -1f, 1f);

        float angelPercent = (scale + 1f) * 0.5f;
        float demonPercent = 1f - angelPercent;

        balanceAngelicBarFill.style.width = Length.Percent(angelPercent * 100f);
        balanceDemonicBarFill.style.width = Length.Percent(demonPercent * 100f);
    }

    private void UpdateLabels()
    {
        if (roomId < 0)
        {
            angelCombatPointsLabel.text = "";
            seperatorLabel.text = "Room ID not set";
            demonCombatPointsLabel.text = "";
            return;
        }

        var list = GameState.Instance.RoomCombatPointsList;

        if (list == null || roomId >= list.Count)
        {
            angelCombatPointsLabel.text = "";
            seperatorLabel.text = $"Room with ID {roomId} not found";
            demonCombatPointsLabel.text = "";
            return;
        }

        CombatPoints combatPoints = list[roomId];

        angelCombatPointsLabel.text = $"{combatPoints.angelPoints}";
        seperatorLabel.text = "/";
        demonCombatPointsLabel.text = $"{combatPoints.demonPoints}";

        angelCombatPointsLabel.style.color = angelLabelColor;
        demonCombatPointsLabel.style.color = demonLabelColor;
    }
}
