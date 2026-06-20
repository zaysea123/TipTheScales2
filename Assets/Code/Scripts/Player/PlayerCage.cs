using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCage : MonoBehaviour
{
    #region Singleton Setup
    public static PlayerCage Instance { get; private set; }
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
            timeoutTimer = root.Q<Label>("TimeoutTimer");
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

    public float jailTime = 10f;

    [SerializeField]
    private Vector3 playerSpawnOffset;

    [SerializeField]
    private GameObject jailFloor;

    [SerializeField]
    private UIDocument document;

    private Label timeoutTimer;

    private Coroutine cageCoroutine;

    public void SendPlayerToJail(GameObject player)
    {
        jailFloor.SetActive(true);

        player.transform.position = transform.position + playerSpawnOffset;

        StyleMeter.Instance.PauseDecay();

        EventBus<ChangeRoomStateEvent>.Raise(new ChangeRoomStateEvent
        {
            RoomId = GameState.Instance.CurrentRoomId,
            IsPlayerInRoom = false,
        });

        timeoutTimer.RemoveFromClassList("hidden");

        if (cageCoroutine != null)
            StopCoroutine(cageCoroutine);

        cageCoroutine = StartCoroutine(CageCountdown());
    }

    private IEnumerator CageCountdown()
    {
        float remainingTime = jailTime;

        while (remainingTime > 0)
        {
            timeoutTimer.text = "Time: " + Mathf.CeilToInt(remainingTime).ToString("F1");

            remainingTime -= Time.deltaTime;

            yield return null;
        }

        timeoutTimer.text = "Time: 0";

        jailFloor.SetActive(false);

        timeoutTimer.AddToClassList("hidden");
    }
}
