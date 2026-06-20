using System;
using UnityEngine.InputSystem;

public static class InputRebindUtility
{
    public static void StartRebind(InputAction action, int bindingIndex, Action<InputAction> onConflict, Action onFinished = null)
    {
        string previousPath = action.bindings[bindingIndex].overridePath;

        action.Disable();

        action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .WithTimeout(10f)
            .OnComplete(operation =>
            {
                operation.Dispose();

                string newPath = action.bindings[bindingIndex].effectivePath;

                if (TryFindConflict(action.actionMap.asset, action, newPath, out InputAction conflicting))
                {
                    onConflict?.Invoke(conflicting);
                }

                action.Enable();

                onFinished?.Invoke();
            })
            .OnCancel(operation =>
            {
                operation.Dispose();

                action.Enable();

                onFinished?.Invoke();
            })
            .Start();
    }

    private static bool TryFindConflict(InputActionAsset asset, InputAction currentAction, string path, out InputAction conflictingAction)
    {
        foreach (var map in asset.actionMaps)
        {
            foreach (var action in map.actions)
            {
                if (action == currentAction)
                    continue;

                foreach (var binding in action.bindings)
                {
                    if (binding.effectivePath == path)
                    {
                        conflictingAction = action;
                        return true;
                    }
                }
            }
        }

        conflictingAction = null;
        return false;
    }

    public static bool HasBindingOverride(InputAction action, int bindingIndex)
    {
        InputBinding binding = action.bindings[bindingIndex];

        return !string.IsNullOrEmpty(binding.overridePath);
    }

    public static bool HasAnyOverrides(InputActionAsset inputActions)
    {
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                foreach (InputBinding binding in action.bindings)
                {
                    if (!string.IsNullOrEmpty(binding.overridePath))
                        return true;
                }
            }
        }

        return false;
    }
}