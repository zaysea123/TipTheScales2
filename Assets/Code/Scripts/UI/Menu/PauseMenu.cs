using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private UIDocument pauseDocument;

    [SerializeField]
    private UIDocument inputSettingsDocument;

    private VisualElement pauseContainer;
    private VisualElement inputSettingsContainer;

    private Button resumeButton;
    private Button changeBindingsButton;
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
        inputSettingsContainer = inputSettingsDocument.rootVisualElement.Q<VisualElement>("PanelBackground");

        resumeButton = pauseContainer.Q<Button>("ResumeButton");
        changeBindingsButton = pauseContainer.Q<Button>("ChangeBindingsButton");
        mainMenuButton = pauseContainer.Q<Button>("MainMenuButton");

        resumeButton.clicked += Resume;
        changeBindingsButton.clicked += ChangeBindings;
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

    private void ChangeBindings()
    {
        inputSettingsContainer.RemoveFromClassList("hidden");
    }

    private void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}
