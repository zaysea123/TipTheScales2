using UnityEngine;
using UnityEngine.UIElements;

public class CreditsMenu : MonoBehaviour
{
    [SerializeField]
    private UIDocument creditsDocument;

    private VisualElement root;

    private VisualElement container;

    private Button closeButton;

    private void Awake()
    {
        root = creditsDocument.rootVisualElement;

        container = root.Q<VisualElement>("Container");

        closeButton = root.Q<Button>("CloseButton");

        closeButton.clicked += CloseCreditsMenu;
    }

    public void CloseCreditsMenu()
    {
        container.AddToClassList("hidden");
    }
}
