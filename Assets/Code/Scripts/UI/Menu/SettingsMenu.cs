using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private UIDocument settingsDocument;

    [SerializeField]
    private UIDocument bindingsDocument;

    private VisualElement settingsRoot;
    private VisualElement bindingsRoot;

    private VisualElement settingsContainer;
    private VisualElement bindingsContainer;

    private Button changeBindingsButton;
    private Button closeButton;

    private void Awake()
    {
        settingsRoot = settingsDocument.rootVisualElement;
        bindingsRoot = bindingsDocument.rootVisualElement;

        settingsContainer = settingsRoot.Q<VisualElement>("Container");
        bindingsContainer = bindingsRoot.Q<VisualElement>("PanelBackground");

        changeBindingsButton = settingsRoot.Q<Button>("ChangeBindingsButton");
        closeButton = settingsRoot.Q<Button>("CloseButton");

        changeBindingsButton.clicked += OpenBindingsMenu;
        closeButton.clicked += CloseSettings;
    }

    public void OpenBindingsMenu()
    {
        bindingsContainer.RemoveFromClassList("hidden");
    }

    public void CloseSettings()
    {
        settingsContainer.AddToClassList("hidden");
    }
}
