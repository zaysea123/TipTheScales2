using UnityEngine;

[CreateAssetMenu(
    fileName = "InputActionDisplayData",
    menuName = "Input/Input Action Display Data"
)]
public class InputActionDisplayData : ScriptableObject
{
    public string actionName;
    public string displayName;
}