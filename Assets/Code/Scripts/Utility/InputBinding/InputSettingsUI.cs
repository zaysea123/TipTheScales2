using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputSettingsUI : MonoBehaviour
{
    [SerializeField]
    private UIDocument document;

    [SerializeField]
    private InputActionAsset inputActions;

    [SerializeField]
    private InputDisplayDatabase displayDatabase;

    private VisualElement panelBackground;
    private VisualElement container;

    private Button closeButton;
    private Button saveButton;
    private Button resetAllButton;

    private Label conflictLabel;

    private bool hasUnsavedChanges;

    private EventBinding<PauseGameStateChangedEvent> pauseGameStateChangedBinding;

    private void Awake()
    {
        InputBindingSaveSystem.Load(inputActions);

        VisualElement root = document.rootVisualElement;

        panelBackground = root.Q<VisualElement>("PanelBackground");
        container = root.Q<VisualElement>("BindingsContainer");

        closeButton = root.Q<Button>("CloseButton");
        saveButton = root.Q<Button>("SaveButton");
        resetAllButton = root.Q<Button>("ResetBindingsButton");

        conflictLabel = root.Q<Label>("ConflictLabel");

        closeButton.clicked += Hide;
        saveButton.clicked += SaveChanges;
        resetAllButton.clicked += ResetBindings;

        BuildUI();

        UpdateButtons();
    }

    private void OnEnable()
    {
        pauseGameStateChangedBinding = new EventBinding<PauseGameStateChangedEvent>(OnPauseGameStateChanged);

        EventBus<PauseGameStateChangedEvent>.Register(pauseGameStateChangedBinding);
    }

    private void OnDisable()
    {
        EventBus<PauseGameStateChangedEvent>.Deregister(pauseGameStateChangedBinding);
    }

    private void OnPauseGameStateChanged(PauseGameStateChangedEvent e)
    {
        Hide();
    }

    private void Hide()
    {
        panelBackground.AddToClassList("hidden");
    }

    private void SaveChanges()
    {
        InputBindingSaveSystem.Save(inputActions);

        hasUnsavedChanges = false;

        UpdateButtons();
    }

    private void ResetBindings()
    {
        InputBindingSaveSystem.Reset(inputActions);

        BuildUI();

        hasUnsavedChanges = true;

        UpdateButtons();
    }

    private void MarkDirty()
    {
        hasUnsavedChanges = true;

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        saveButton.SetEnabled(hasUnsavedChanges);

        resetAllButton.SetEnabled(InputRebindUtility.HasAnyOverrides(inputActions));
    }

    private void ShowConflict(InputAction action, InputAction conflictingAction)
    {
        conflictLabel.text =
            $"{displayDatabase.GetDisplayName(action.name)} " +
            $"conflicts with " +
            $"{displayDatabase.GetDisplayName(conflictingAction.name)}";
    }

    private void BuildUI()
    {
        container.Clear();

        foreach (InputActionMap map in inputActions.actionMaps)
        {
            BuildMap(map);
        }
    }

    private void BuildMap(InputActionMap map)
    {
        Label mapLabel = new(map.name);

        mapLabel.AddToClassList("heading");

        container.Add(mapLabel);

        foreach (InputAction action in map.actions)
        {
            BuildAction(action);
        }
    }

    private void BuildAction(InputAction action)
    {
        VisualElement row = new();
        row.AddToClassList("action-row");

        Label actionLabel = new(displayDatabase.GetDisplayName(action.name));
        actionLabel.AddToClassList("heading-two");
        row.Add(actionLabel);

        VisualElement divider = new();
        divider.AddToClassList("divider");
        row.Add(divider);

        VisualElement groupsContainer = new();
        groupsContainer.AddToClassList("binding-groups-container");
        row.Add(groupsContainer);

        Dictionary<string, List<int>> groups = BuildBindingGroups(action);

        foreach (var group in groups)
        {
            BuildGroup(action, group.Key, group.Value, groupsContainer);
        }

        container.Add(row);
    }

    private Dictionary<string, List<int>> BuildBindingGroups(InputAction action)
    {
        Dictionary<string, List<int>> groups = new();

        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding binding = action.bindings[i];

            if (binding.isComposite)
                continue;

            string group = string.IsNullOrEmpty(binding.groups) ? "Other" : binding.groups;

            if (!groups.ContainsKey(group))
                groups[group] = new();

            groups[group].Add(i);
        }

        return groups;
    }

    private void BuildGroup(InputAction action, string groupName, List<int> bindingIndices, VisualElement parent)
    {
        VisualElement group = new();
        group.AddToClassList("binding-group");
        parent.Add(group);

        VisualElement labelContainer = new();
        labelContainer.AddToClassList("binding-group-label-container");
        group.Add(labelContainer);

        Label label = new(GetReadableGroupName(groupName));
        label.AddToClassList("heading-three");
        labelContainer.Add(label);

        VisualElement buttonsContainer = new();
        buttonsContainer.AddToClassList("binding-group-buttons-container");
        group.Add(buttonsContainer);

        foreach (int bindingIndex in bindingIndices)
        {
            InputBindingElement element = new(action, bindingIndex, MarkDirty,
                    conflictingAction =>
                    {
                        ShowConflict(action, conflictingAction);
                    });

            buttonsContainer.Add(element.Root);
        }
    }

    private string GetReadableGroupName(string group)
    {
        return group switch
        {
            ";Keyboard&Mouse" => "Keyboard",
            "Keyboard&Mouse" => "Keyboard",
            "Keyboard&Mouse;Gamepad;Touch;Joystick;XR" => "Any",
            ";Gamepad" => "Controller",
            _ => group
        };
    }
}