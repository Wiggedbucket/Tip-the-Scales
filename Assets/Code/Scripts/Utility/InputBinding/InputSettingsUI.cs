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

    private VisualElement root;
    private VisualElement panelBackground;
    private VisualElement container;

    private Button closeButton;
    private Button saveButton;
    private Button resetAllButton;

    private Label conflictLabel;

    private bool hasUnsavedChanges;

    private void Awake()
    {
        InputBindingSaveSystem.Load(inputActions);

        root = document.rootVisualElement;

        panelBackground = root.Q<VisualElement>("PanelBackground");
        container = root.Q<VisualElement>("BindingsContainer");

        closeButton = root.Q<Button>("CloseButton");
        saveButton = root.Q<Button>("SaveButton");
        resetAllButton = root.Q<Button>("ResetBindingsButton");

        conflictLabel = root.Q<Label>("ConflictLabel");

        closeButton.clicked += () => panelBackground.AddToClassList("hidden");
        saveButton.clicked += SaveChanges;
        resetAllButton.clicked += ResetBindings;

        BuildUI();

        UpdateButtons();
    }

    private EventBinding<PauseGameStateChangedEvent> pauseGameStateChangedBinding;

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

    private void UpdateButtons()
    {
        saveButton.SetEnabled(hasUnsavedChanges);

        resetAllButton.SetEnabled(InputRebindUtility.HasAnyOverrides(inputActions));
    }

    private void BuildUI()
    {
        container.Clear();

        foreach (InputActionMap map in inputActions.actionMaps)
        {
            Label mapLabel = new(map.name);
            mapLabel.AddToClassList("heading");

            container.Add(mapLabel);

            foreach (InputAction action in map.actions)
            {
                CreateActionRow(action);
            }
        }
    }

    private void CreateActionRow(InputAction action)
    {
        VisualElement row = new();
        row.AddToClassList("action-row");

        Label actionNameLabel = new(displayDatabase.GetDisplayName(action.name));

        actionNameLabel.AddToClassList("heading-two");

        row.Add(actionNameLabel);

        VisualElement divider = new();
        divider.AddToClassList("divider");

        row.Add(divider);

        VisualElement bindingGroupsContainer = new();
        bindingGroupsContainer.AddToClassList("binding-groups-container");

        row.Add(bindingGroupsContainer);

        Dictionary<string, List<int>> bindingGroups = new();

        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding binding = action.bindings[i];

            if (binding.isComposite)
                continue;

            string group = string.IsNullOrEmpty(binding.groups) ? "Other" : binding.groups;

            if (!bindingGroups.ContainsKey(group))
                bindingGroups[group] = new();

            bindingGroups[group].Add(i);
        }

        foreach (var group in bindingGroups)
        {
            CreateBindingGroup(action, group.Key, group.Value, bindingGroupsContainer);
        }

        container.Add(row);
    }

    private void CreateBindingGroup(InputAction action, string groupName, List<int> bindingIndices, VisualElement parent)
    {
        VisualElement bindingGroup = new();
        bindingGroup.AddToClassList("binding-group");

        parent.Add(bindingGroup);

        VisualElement labelContainer = new();
        labelContainer.AddToClassList("binding-group-label-container");

        bindingGroup.Add(labelContainer);

        //Debug.Log($"Group name: {groupName}");
        Label groupLabel = new(GetReadableGroupName(groupName));

        groupLabel.AddToClassList("heading-three");

        labelContainer.Add(groupLabel);

        VisualElement buttonsContainer = new();
        buttonsContainer.AddToClassList("binding-group-buttons-container");

        bindingGroup.Add(buttonsContainer);

        foreach (int bindingIndex in bindingIndices)
        {
            CreateBindingButtonBundle(action, bindingIndex, buttonsContainer);
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

    private void CreateBindingButtonBundle(InputAction action, int bindingIndex, VisualElement parent)
    {
        InputBinding binding = action.bindings[bindingIndex];

        VisualElement bundle = new();
        bundle.AddToClassList("binding-button-bundle");

        // Composite part label (Up, Down, Left, Right, etc.)
        Label partLabel = new();

        if (binding.isPartOfComposite)
        {
            partLabel.text = binding.name;
        }
        else
        {
            partLabel.text = string.Empty;
        }

        partLabel.AddToClassList("binding-part-label");

        Button bindingButton = new()
        {
            text = action.GetBindingDisplayString(bindingIndex)
        };

        bindingButton.AddToClassList("binding-button");

        Button resetButton = new()
        {
            text = "Reset"
        };

        resetButton.AddToClassList("reset-binding-button");

        resetButton.SetEnabled(InputRebindUtility.HasBindingOverride(action, bindingIndex));

        resetButton.clicked += () =>
        {
            action.RemoveBindingOverride(bindingIndex);

            bindingButton.text = action.GetBindingDisplayString(bindingIndex);

            hasUnsavedChanges = true;

            resetButton.SetEnabled(false);

            UpdateButtons();

            conflictLabel.text = "";
        };

        bindingButton.clicked += () =>
        {
            conflictLabel.text = "";

            bindingButton.text = "<...>";

            InputRebindUtility.StartRebind(action, bindingIndex,

                conflictingAction =>
                {
                    conflictLabel.text =
                        $"{displayDatabase.GetDisplayName(action.name)} " +
                        $"conflicts with " +
                        $"{displayDatabase.GetDisplayName(conflictingAction.name)}";
                },

                () =>
                {
                    bindingButton.text = action.GetBindingDisplayString(bindingIndex);

                    hasUnsavedChanges = true;

                    resetButton.SetEnabled(InputRebindUtility.HasBindingOverride(action, bindingIndex));

                    UpdateButtons();
                });
        };

        bundle.Add(partLabel);
        bundle.Add(bindingButton);
        bundle.Add(resetButton);
        
        parent.Add(bundle);
    }
}