using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private UIDocument pauseDocument;

    [SerializeField]
    private UIDocument settingsDocument;

    private VisualElement pauseContainer;
    private VisualElement settingsContainer;

    private Button resumeButton;
    private Button settingsButton;
    private Button mainMenuButton;

    private EventBinding<PauseGameStateChangedEvent> pauseGameStateChangedBinding;

    private void OnEnable()
    {
        pauseGameStateChangedBinding = new EventBinding<PauseGameStateChangedEvent>(OnPauseGameStateChanged);
        EventBus<PauseGameStateChangedEvent>.Register(pauseGameStateChangedBinding);
    }

    private void OnDisable()
    {
        EventBus<PauseGameStateChangedEvent>.Deregister(pauseGameStateChangedBinding);
    }

    private void Awake()
    {
        pauseContainer = pauseDocument.rootVisualElement.Q<VisualElement>("Container");
        settingsContainer = settingsDocument.rootVisualElement.Q<VisualElement>("Container");

        resumeButton = pauseContainer.Q<Button>("ResumeButton");
        settingsButton = pauseContainer.Q<Button>("SettingsButton");
        mainMenuButton = pauseContainer.Q<Button>("MainMenuButton");

        resumeButton.clicked += Resume;
        settingsButton.clicked += OpenSettings;
        mainMenuButton.clicked += MainMenu;

		pauseContainer.AddToClassList("hidden");
    }

    private void OnPauseGameStateChanged(PauseGameStateChangedEvent e)
    {
        if (e.IsPaused)
			pauseContainer.RemoveFromClassList("hidden");
        else
			pauseContainer.AddToClassList("hidden");
	}

	private void Resume()
    {
        GameState.Instance.TogglePauseGame();  
	}

    private void OpenSettings()
    {
        settingsContainer.RemoveFromClassList("hidden");
    }

    private void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}
