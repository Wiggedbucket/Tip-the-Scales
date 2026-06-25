using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private UIDocument settingsDocument;

    [SerializeField]
    private UIDocument audioDocument;

    [SerializeField]
    private UIDocument bindingsDocument;

    private VisualElement settingsRoot;
    private VisualElement audioRoot;
    private VisualElement bindingsRoot;

    private VisualElement settingsContainer;
    private VisualElement audioContainer;
    private VisualElement bindingsContainer;

    private Button audioButton;
    private Button changeBindingsButton;
    private Button closeButton;

    private void Awake()
    {
        settingsRoot = settingsDocument.rootVisualElement;
        audioRoot = audioDocument.rootVisualElement;
        bindingsRoot = bindingsDocument.rootVisualElement;

        settingsContainer = settingsRoot.Q<VisualElement>("Container");
        audioContainer = audioRoot.Q<VisualElement>("Container");
        bindingsContainer = bindingsRoot.Q<VisualElement>("PanelBackground");

        audioButton = settingsRoot.Q<Button>("AdjustAudioButton");
        changeBindingsButton = settingsRoot.Q<Button>("ChangeBindingsButton");
        closeButton = settingsRoot.Q<Button>("CloseButton");

        audioButton.clicked += OpenAudioMenu;
        changeBindingsButton.clicked += OpenBindingsMenu;
        closeButton.clicked += CloseSettings;
    }

    public void OpenAudioMenu()
    {
        audioContainer.RemoveFromClassList("hidden");
    }

    public void OpenBindingsMenu()
    {
        bindingsContainer.RemoveFromClassList("hidden");
    }

    public void CloseSettings()
    {
        settingsContainer.AddToClassList("hidden");
        audioContainer.AddToClassList("hidden");
        bindingsContainer.AddToClassList("hidden");
    }
}
