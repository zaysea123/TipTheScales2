using UnityEngine;
using UnityEngine.UIElements;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField]
    private UIDocument document;

    private Label ammoLabel;
    private Label healthLabel;

    private VisualElement crosshair;

    [SerializeField]
    private PlayerShooting shooting;

    [SerializeField]
    private Health health;

    [SerializeField]
    private float crosshairAppearTime = 0.1f;

    private float crosshairTimer = 0f;

    private EventBinding<TookDamageEvent> tookDamageBinding;

    private void OnEnable()
    {
        tookDamageBinding = new EventBinding<TookDamageEvent>(ShowHitIndicator);
        EventBus<TookDamageEvent>.Register(tookDamageBinding);
    }

    private void OnDisable()
    {
        EventBus<TookDamageEvent>.Deregister(tookDamageBinding);
    }

    private void Awake()
    {
        var root = document.rootVisualElement;

        ammoLabel = root.Q<Label>("AmmoLabel");
        healthLabel = root.Q<Label>("HealthLabel");
        crosshair = root.Q<VisualElement>("HitIndicator");
    }

    private void Update()
    {
        if (shooting != null)
            ammoLabel.text = $"Ammo: {shooting.CurrentAmmo}/{shooting.CurrentWeapon.MaxAmmo}";

        if (health != null)
            healthLabel.text = $"Health: {health.GetHealth()}/{health.GetMaxHealth()}";

        if (crosshairTimer < crosshairAppearTime)
            crosshairTimer += Time.deltaTime;
        else
            crosshair.AddToClassList("hidden");
    }

    private void ShowHitIndicator(TookDamageEvent e)
    {
        if (!e.IsEnemy)
            return;

        if (e.Died)
            crosshair.AddToClassList("red");
        else
            crosshair.RemoveFromClassList("red");

        crosshair.RemoveFromClassList("hidden");

        crosshairTimer = 0f;
    }
}
