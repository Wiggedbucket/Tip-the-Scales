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

    private Button resetButton;

    private Label conflictLabel;

    private bool hasUnsavedChanges;

    private void Awake()
    {
        InputBindingSaveSystem.Load(inputActions);

        var root = document.rootVisualElement;

        container = root.Q<VisualElement>("BindingsContainer");
        saveButton = root.Q<Button>("SaveButton");
        resetButton = root.Q<Button>("ResetBindingsButton");
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

        row.style.flexDirection = FlexDirection.Row;
        row.style.marginBottom = 4;

        Label actionLabel = new(displayDatabase.GetDisplayName(action.name));
        actionLabel.AddToClassList("heading-two");

        actionLabel.style.flexGrow = 1;

        row.Add(actionLabel);
        row.AddToClassList("action-row");

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].isComposite)
                continue;

            int bindingIndex = i;

            Button button = new()
            {
                text = action.GetBindingDisplayString(bindingIndex)
            };

            button.AddToClassList("binding-button");

            button.clicked += () =>
            {
                conflictLabel.text = "";

                button.text = "Press a key...";

                InputRebindUtility
                    .StartRebind(action, bindingIndex,

                        conflictingAction =>
                        {
                            conflictLabel.text =
                                $"{displayDatabase.GetDisplayName(action.name)} " +
                                $"conflicts with " +
                                $"{displayDatabase.GetDisplayName(conflictingAction.name)}";
                        },

                        () =>
                        {
                            button.text = action.GetBindingDisplayString(bindingIndex);

                            hasUnsavedChanges = true;

                            UpdateSaveButton();
                        });
            };

            row.Add(button);
        }

        container.Add(row);
    }
}