using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX = 0.1f;
    public float sensY = 0.1f;

    public Transform Orientation;

    [Header("Input binding")]
    [SerializeField] private InputAction look;

    [Header("Camera Mover")]
    public Transform cameraPosition;

    float xRotation;
    float yRotation;

    private EventBinding<PauseGameStateChangedEvent> pauseGameStateChangedBinding;

    private void OnEnable()
    {
        pauseGameStateChangedBinding = new EventBinding<PauseGameStateChangedEvent>(OnPauseGameStateChanged);
        EventBus<PauseGameStateChangedEvent>.Register(pauseGameStateChangedBinding);

        look.Enable();
    }

    private void OnDisable()
    {
        EventBus<PauseGameStateChangedEvent>.Deregister(pauseGameStateChangedBinding);

        look.Disable();
    }

    private void OnPauseGameStateChanged(PauseGameStateChangedEvent e)
    {
        if (e.IsPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            look.Disable();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            look.Enable();
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 lookInput = look.ReadValue<Vector2>();

        float mouseX = lookInput.x * sensX;
        float mouseY = lookInput.y * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        Orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void LateUpdate()
    {
        transform.position = cameraPosition.position;
    }
}
