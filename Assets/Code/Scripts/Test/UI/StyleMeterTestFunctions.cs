using UnityEngine;

public class StyleMeterTestFunctions : MonoBehaviour
{
    [SerializeField]
    private int stylePoints = 50;

    [SerializeField]
    private string reason = "TEST";

    [SerializeField]
    private Color textColor = Color.white;

    [SerializeField]
    private string weapon = "Shotgun";

    [ContextMenu("Add Style Points")]
    public void AddStylePoints()
    {
        //Debug.Log("Raising StyleGainEvent");

        EventBus<StyleGainEvent>.Raise(new StyleGainEvent()
        {
            Amount = stylePoints,
            Reason = reason,
            TextColor = textColor,
        });
    }

    [ContextMenu("Shoot weapon")]
    public void ShootWeapon()
    {
        //Debug.Log("Shooting weapon");

        EventBus<WeaponFiredEvent>.Raise(new WeaponFiredEvent()
        {
            Weapon = weapon,
        });
    }
}
