using System;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputBindingElement
{
    public VisualElement Root { get; }

    private readonly InputAction action;
    private readonly int bindingIndex;

    private readonly Button bindingButton;
    private readonly Button resetButton;

    private readonly Action onChanged;
    private readonly Action<InputAction> onConflict;

    public InputBindingElement(InputAction action, int bindingIndex, Action onChanged, Action<InputAction> onConflict)
    {
        this.action = action;
        this.bindingIndex = bindingIndex;

        this.onChanged = onChanged;
        this.onConflict = onConflict;

        InputBinding binding = action.bindings[bindingIndex];

        Root = new VisualElement();
        Root.AddToClassList("binding-button-bundle");

        Label partLabel = new();

        if (binding.isPartOfComposite)
        {
            partLabel.text = binding.name;
        }

        partLabel.AddToClassList("binding-part-label");

        bindingButton = new Button();
        bindingButton.AddToClassList("binding-button");

        resetButton = new Button
        {
            text = "Reset"
        };

        resetButton.AddToClassList("reset-binding-button");

        bindingButton.clicked += StartRebind;
        resetButton.clicked += ResetBinding;

        Root.Add(partLabel);
        Root.Add(bindingButton);
        Root.Add(resetButton);

        Refresh();
    }

    private void Refresh()
    {
        bindingButton.text = action.GetBindingDisplayString(bindingIndex);

        resetButton.SetEnabled(InputRebindUtility.HasBindingOverride( action, bindingIndex));
    }

    private void ResetBinding()
    {
        action.RemoveBindingOverride(bindingIndex);

        Refresh();

        onChanged?.Invoke();
    }

    private void StartRebind()
    {
        bindingButton.text = "<...>";

        InputRebindUtility.StartRebind(action, bindingIndex,

            conflict =>
            {
                onConflict?.Invoke(conflict);
            },

            () =>
            {
                Refresh();

                onChanged?.Invoke();
            });
    }
}