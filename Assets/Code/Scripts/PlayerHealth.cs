using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    private Health health;

    [SerializeField]
    private GameObject player;

    [Header("Input binding")]
    [SerializeField] private InputAction look;

    private void Start()
    {
        health = GetComponent<Health>();
        health.OnDeath += HandleDeath;
    }

    private void HandleDeath()
    {
        if (GameState.Instance.InPermaDeathRange)
        {
            MatchEndMenu.Instance.OpenMenu(false);

            GameState.Instance.MatchEnded = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            look.Disable();

            // Send death to the server (If in multiplayer)
        }
        else
        {
            PlayerCage.Instance.SendPlayerToJail(player);
            health.Revive();
        }
    }
}