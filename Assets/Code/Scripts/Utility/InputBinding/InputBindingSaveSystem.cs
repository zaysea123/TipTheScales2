using UnityEngine;
using UnityEngine.InputSystem;

public static class InputBindingSaveSystem
{
    private const string SaveKey = "InputBindingsOverrides";

    public static void Save(InputActionAsset asset)
    {
        PlayerPrefs.SetString(asset.name + SaveKey, asset.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
    }

    public static void Load(InputActionAsset asset)
    {
        if (!PlayerPrefs.HasKey(asset.name + SaveKey))
            return;

        asset.LoadBindingOverridesFromJson(PlayerPrefs.GetString(asset.name + SaveKey));
    }

    public static void Reset(InputActionAsset asset)
    {
        asset.RemoveAllBindingOverrides();
    }
}