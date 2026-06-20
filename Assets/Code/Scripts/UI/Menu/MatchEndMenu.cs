using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MatchEndMenu : MonoBehaviour
{
    #region Singleton Setup
    public static MatchEndMenu Instance { get; private set; }
    public static bool InstanceExists => Instance != null;

    void Awake()
    {
        if (InstanceExists)
        {
            Destroy(gameObject);
        }
        else
        {
            // Registers the first valid instance before the rest of the scene startup flow.
            Instance = this;

            var root = document.rootVisualElement;

            container = root.Q<VisualElement>("Container");

            matchResultLabel = root.Q<Label>("MatchResultLabel");

            mainMenuButton = root.Q<Button>("MainMenuButton");

            mainMenuButton.clicked += ToMainMenu;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    [SerializeField]
    private UIDocument document;

    private VisualElement container;

    private Label matchResultLabel;

    private Button mainMenuButton;

    public void OpenMenu(bool playerWon)
    {
        container.RemoveFromClassList("hidden");

        matchResultLabel.text = playerWon ? "You Won!" : "You Lost!";

        Time.timeScale = 0f;
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}
