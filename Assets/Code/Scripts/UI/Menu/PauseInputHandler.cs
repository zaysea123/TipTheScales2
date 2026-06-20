using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInputHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;

    private InputAction menuAction;

    private void Awake()
    {
        menuAction = inputActions.FindAction("Menu");
    }

    private void OnEnable()
    {
        menuAction.performed += OnMenuPerformed;
        menuAction.Enable();
    }

    private void OnDisable()
    {
        menuAction.performed -= OnMenuPerformed;
        menuAction.Disable();
    }

    private void OnMenuPerformed(InputAction.CallbackContext context)
    {
        if (GameState.Instance.MatchEnded)
            return;

        GameState.Instance.TogglePauseGame();
    }
}