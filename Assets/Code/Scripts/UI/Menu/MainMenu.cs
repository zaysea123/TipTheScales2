using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private UIDocument mainMenuDocument;

    [SerializeField]
    private UIDocument settingsDocument;

    [SerializeField]
    private UIDocument creditsDocument;

    private VisualElement mainMenuRoot;
    private VisualElement settingsRoot;
    private VisualElement creditsRoot;

    private VisualElement settingsContainer;
    private VisualElement creditsContainer;

    private Button singleplayerButton;
    private Button multiplayerButton;
    private Button settingsButton;
    private Button creditsButton;
    private Button quitButton;

    private void Awake()
    {
        mainMenuRoot = mainMenuDocument.rootVisualElement;
        settingsRoot = settingsDocument.rootVisualElement;
        creditsRoot = creditsDocument.rootVisualElement;

        settingsContainer = settingsRoot.Q<VisualElement>("Container");
        creditsContainer = creditsRoot.Q<VisualElement>("Container");

        singleplayerButton = mainMenuRoot.Q<Button>("SingleplayerButton");
        multiplayerButton = mainMenuRoot.Q<Button>("MultiplayerButton");
        settingsButton = mainMenuRoot.Q<Button>("SettingsButton");
        creditsButton = mainMenuRoot.Q<Button>("CreditsButton");
        quitButton = mainMenuRoot.Q<Button>("QuitButton");

        singleplayerButton.clicked += StartSingleplayer;
        multiplayerButton.clicked += StartMultiplayer;
        settingsButton.clicked += OpenSettings;
        creditsButton.clicked += OpenCredits;
        quitButton.clicked += QuitGame;
    }

    public void StartSingleplayer()
    {
        // Sets up a SinglePlayer game state
        GameMode.IsMultiplayer = false;
        SceneManager.LoadScene("Dungeon");
    }

    public void StartMultiplayer()
    {
        // Sets up multiplayer game state
        GameMode.IsMultiplayer = true;
        SceneManager.LoadScene("Dungeon");
    }

    public void OpenSettings()
    {
        settingsContainer.RemoveFromClassList("hidden");
    }

    public void OpenCredits()
    {
        creditsContainer.RemoveFromClassList("hidden");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
