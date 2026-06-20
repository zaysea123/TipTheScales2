using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX = 0.1f;
    public float sensY = 0.1f;

    public Transform Orientation;

    [Header("Input binding")]
    [SerializeField] private InputActionReference lookActionReference;

    private InputAction Look => lookActionReference.action;

    [Header("Camera Mover")]
    public Transform cameraPosition;

    float xRotation;
    float yRotation;

    private EventBinding<PauseGameStateChangedEvent> pauseGameStateChangedBinding;

    private EventBinding<TookDamageEvent> tookDamageBinding;

    private EventBinding<PlayerWonEvent> playerWonBinding;

    private void OnEnable()
    {
        pauseGameStateChangedBinding = new EventBinding<PauseGameStateChangedEvent>(OnPauseGameStateChanged);
        EventBus<PauseGameStateChangedEvent>.Register(pauseGameStateChangedBinding);

        tookDamageBinding = new EventBinding<TookDamageEvent>(OnTookDamage);
        EventBus<TookDamageEvent>.Register(tookDamageBinding);

        playerWonBinding = new EventBinding<PlayerWonEvent>(OnPlayerWon);
        EventBus<PlayerWonEvent>.Register(playerWonBinding);

        Look.Enable();
    }

    private void OnDisable()
    {
        EventBus<PauseGameStateChangedEvent>.Deregister(pauseGameStateChangedBinding);
        EventBus<TookDamageEvent>.Deregister(tookDamageBinding);
        EventBus<PlayerWonEvent>.Deregister(playerWonBinding);

        Look.Disable();
    }

    private void OnPauseGameStateChanged(PauseGameStateChangedEvent e)
    {
        ToggleLookControl(!e.IsPaused);
    }

    private void OnTookDamage(TookDamageEvent e)
    {
        if (!e.IsEnemy && e.Died && GameState.Instance.InPermaDeathRange)
            ToggleLookControl(false);
    }

    private void OnPlayerWon(PlayerWonEvent e)
    {
        ToggleLookControl(false);
    }

    private void ToggleLookControl(bool enable)
    {
        if (enable)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Look.Enable();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Look.Disable();
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 lookInput = Look.ReadValue<Vector2>();

        float mouseX = lookInput.x * sensX;
        float mouseY = lookInput.y * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        Orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void LateUpdate()
    {
        transform.position = cameraPosition.position;
    }
}
