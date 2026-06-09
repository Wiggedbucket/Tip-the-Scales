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

    private VisualElement container;

    private Button saveButton;

    private Label conflictLabel;

    private bool hasUnsavedChanges;

    private void Awake()
    {
        InputBindingSaveSystem.Load(inputActions);

        var root = document.rootVisualElement;

        container = root.Q<VisualElement>("BindingsContainer");
        saveButton = root.Q<Button>("SaveButton");
        Button resetButton = root.Q<Button>("ResetBindingsButton");
        conflictLabel = root.Q<Label>("ConflictLabel");

        saveButton.clicked += SaveChanges;
        resetButton.clicked += ResetBindings;

        BuildUI();

        UpdateSaveButton();
    }

    private void SaveChanges()
    {
        InputBindingSaveSystem.Save(inputActions);

        hasUnsavedChanges = false;

        UpdateSaveButton();
    }

    private void ResetBindings()
    {
        InputBindingSaveSystem.Reset(inputActions);

        BuildUI();

        hasUnsavedChanges = true;

        UpdateSaveButton();
    }

    private void UpdateSaveButton()
    {
        saveButton.SetEnabled(hasUnsavedChanges);
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

        Debug.Log($"Group name: {groupName}");
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

        // Spacer pushes reset button to the right
        VisualElement spacer = new();
        spacer.AddToClassList("binding-spacer");

        Button resetButton = new()
        {
            text = "Reset"
        };

        resetButton.AddToClassList("binding-button");

        resetButton.clicked += () =>
        {
            action.RemoveBindingOverride(bindingIndex);

            bindingButton.text = action.GetBindingDisplayString(bindingIndex);

            hasUnsavedChanges = true;

            UpdateSaveButton();

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

                    UpdateSaveButton();
                });
        };

        bundle.Add(partLabel);
        bundle.Add(bindingButton);
        bundle.Add(spacer);
        bundle.Add(resetButton);
        
        parent.Add(bundle);
    }
}