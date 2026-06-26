using UnityEngine;
using UnityEngine.UIElements;

public class RoomPointsDisplay : MonoBehaviour
{
    [SerializeField]
    private int roomId = -1;

    [SerializeField]
    private UIDocument document;

    private Image roomStateSymbol;

    private VisualElement balanceDemonicBarFill;
    private VisualElement balanceAngelicBarFill;

    private Label angelCombatPointsLabel;
    private Label seperatorLabel;
    private Label demonCombatPointsLabel;

    [SerializeField] private StyleBackground roomStateSymbolDemon;
    [SerializeField] private StyleBackground roomStateSymbolAngel;
    [SerializeField] private StyleBackground roomStateSymbolHuman;

    [SerializeField]
    private Color angelLabelColor = Color.yellow;
    [SerializeField]
    private Color demonLabelColor = Color.red;

    [SerializeField] private float barLerpSpeed = 5f;

    private float displayedScale;

    private void Awake()
    {
        var root = document.rootVisualElement;

        roomStateSymbol = root.Q<Image>("RoomStateSymbol");

        balanceDemonicBarFill = root.Q<VisualElement>("BalanceDemonicBarFill");
        balanceAngelicBarFill = root.Q<VisualElement>("BalanceAngelicBarFill");

        angelCombatPointsLabel = root.Q<Label>("AngelCombatPointsLabel");
        seperatorLabel = root.Q<Label>("SeperatorLabel");
        demonCombatPointsLabel = root.Q<Label>("DemonCombatPointsLabel");

        CombatPoints combatPoints = GameState.Instance.RoomCombatPointsList[roomId];
        displayedScale = GameState.Instance.CalculateScale(combatPoints.angelPoints, combatPoints.demonPoints);
    }

    private void Update()
    {
        UpdateRoomStateSymbol();
        UpdateProgressBar();
        UpdateLabels();
    }

    private void UpdateRoomStateSymbol()
    {
        if (displayedScale <= GameState.Instance.ScaleTreshold)
        {
            roomStateSymbol.style.backgroundImage = roomStateSymbolDemon;
        }
        else if (displayedScale >= -GameState.Instance.ScaleTreshold)
        {
            roomStateSymbol.style.backgroundImage = roomStateSymbolAngel;
        }
        else
        {
            roomStateSymbol.style.backgroundImage = roomStateSymbolHuman;
        }
    }

    private void UpdateProgressBar()
    {
        CombatPoints combatPoints = GameState.Instance.RoomCombatPointsList[roomId];

        float targetScale = GameState.Instance.CalculateScale(combatPoints.angelPoints, combatPoints.demonPoints);

        displayedScale = Mathf.MoveTowards(displayedScale, targetScale, barLerpSpeed * Time.deltaTime);

        float angelPercent = (displayedScale + 1f) * 0.5f;
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
