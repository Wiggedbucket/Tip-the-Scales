using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject playerObject;

    [Header("Sensitivity")]
    public float sensX = 0.1f;
    public float sensY = 0.1f;

    public Transform Orientation;

    [Header("Input binding")]
    [SerializeField] private InputActionReference lookActionReference;
    [SerializeField] private InputActionReference interactActionReference;

    private InputAction Look => lookActionReference.action;
    private InputAction InteractAction => interactActionReference.action;

    [Header("User Interface")]
    [SerializeField] private UIDocument document;

    private Label interactLabel;

    [Header("Camera Mover")]
    public Transform cameraPosition;

    float xRotation;
    float yRotation;

    private EventBinding<PauseGameStateChangedEvent> pauseGameStateChangedBinding;

    private EventBinding<TookDamageEvent> tookDamageBinding;

    private EventBinding<PlayerWonEvent> playerWonBinding;

    private void Awake()
    {
        interactLabel = document.rootVisualElement.Q<Label>("InteractLabel");
        interactLabel.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        pauseGameStateChangedBinding = new EventBinding<PauseGameStateChangedEvent>(OnPauseGameStateChanged);
        EventBus<PauseGameStateChangedEvent>.Register(pauseGameStateChangedBinding);

        tookDamageBinding = new EventBinding<TookDamageEvent>(OnTookDamage);
        EventBus<TookDamageEvent>.Register(tookDamageBinding);

        playerWonBinding = new EventBinding<PlayerWonEvent>(OnPlayerWon);
        EventBus<PlayerWonEvent>.Register(playerWonBinding);

        Look.Enable();

        InteractAction.Enable();
        InteractAction.performed += OnInteract;
    }

    private void OnDisable()
    {
        EventBus<PauseGameStateChangedEvent>.Deregister(pauseGameStateChangedBinding);
        EventBus<TookDamageEvent>.Deregister(tookDamageBinding);
        EventBus<PlayerWonEvent>.Deregister(playerWonBinding);

        Look.Disable();

        InteractAction.performed -= OnInteract;
        InteractAction.Disable();
    }

    private void OnPauseGameStateChanged(PauseGameStateChangedEvent e)
    {
        ToggleLookControl(!e.IsPaused);
    }

    private void OnTookDamage(TookDamageEvent e)
    {
        if (!e.IsEnemy && e.Died && GameState.Instance.InPermaDeathRange)
            ToggleLookControl(false);
    }

    private void OnPlayerWon(PlayerWonEvent e)
    {
        ToggleLookControl(false);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit))
        {
            var interactable = hit.collider.GetComponentInParent<Interactable>();

            if (interactable == null)
                return;

            float distance = Vector3.Distance(playerCamera.transform.position, interactable.transform.position);

            if (distance > interactable.InteractionDistance)
                return;

            interactable.Interact(playerObject);
        }
    }

    private void ToggleLookControl(bool enable)
    {
        if (enable)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;

            Look.Enable();
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            Look.Disable();
        }
    }

    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 lookInput = Look.ReadValue<Vector2>();

        float mouseX = lookInput.x * sensX;
        float mouseY = lookInput.y * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        Orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

        UpdateInteractionUI();
    }

    private void UpdateInteractionUI()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit))
        {
            var interactable = hit.collider.GetComponentInParent<Interactable>();

            if (interactable == null)
            {
                interactLabel.style.display = DisplayStyle.None;
                return;
            }

            float distance = Vector3.Distance(playerCamera.transform.position, interactable.transform.position);

            if (distance > interactable.InteractionDistance)
            {
                interactLabel.style.display = DisplayStyle.None;
                return;
            }

            interactLabel.style.display = DisplayStyle.Flex;
            interactLabel.text = $"[{GetInteractBinding()}] {interactable.InteractionText}";
        }
        else
        {
            interactLabel.style.display = DisplayStyle.None;
        }
    }

    private string GetInteractBinding()
    {
        string path = "";

        for (int i = 0; i < InteractAction.bindings.Count; i++)
        {
            path += InteractAction.GetBindingDisplayString(i);
            if (i != InteractAction.bindings.Count - 1)
                path += ", ";
        }

        return string.IsNullOrEmpty(path) ? "?" : path;
    }

    private void LateUpdate()
    {
        transform.position = cameraPosition.position;
    }
}
