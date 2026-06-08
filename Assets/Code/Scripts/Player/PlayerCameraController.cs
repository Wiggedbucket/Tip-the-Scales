using UnityEditor.Build.Content;
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

    float xRotation;
    float yRotation;

    private void OnEnable()
    {
        look.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
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

    [Header("Camera Mover")]
    public Transform cameraPosition;

    private void LateUpdate()
    {
        transform.position = cameraPosition.position;
    }
}
