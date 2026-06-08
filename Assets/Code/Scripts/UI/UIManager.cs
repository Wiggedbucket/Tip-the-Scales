using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private UIDocument document;

    [SerializeField]
    private VisualElement root;

    public static UIManager Instance;

    [SerializeField]
    private InputActionAsset asset;

    private bool isMenuOpen = false;

    private void Awake()
    {
        Instance = this;

        document = GetComponent<UIDocument>();
        root = document.rootVisualElement.Q<VisualElement>("Panel");
    }

    [ContextMenu("Open Bindings Menu")]
    public void OnMenuButton()
    {
        if (isMenuOpen)
        {
            CloseMenu();
        } else
        {
            OpenMenu();
        }

        isMenuOpen = !isMenuOpen;
    }

    private void CloseMenu()
    {
        Time.timeScale = 1f;
        root.RemoveFromClassList("hidden");
    }

    private void OpenMenu()
    {
        Time.timeScale = 0f;
        root.AddToClassList("hidden");

        Test();
    }

    private void Test()
    {
        VisualElement entry = root.Q<VisualElement>("Entry");

        InputAction action = asset.actionMaps[0].actions[0];
        entry.dataSource = action;
    }
}
