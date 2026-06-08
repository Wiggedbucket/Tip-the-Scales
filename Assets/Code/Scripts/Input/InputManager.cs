using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions actions;

    public Vector2 MoveInput => actions.Gameplay.Move.ReadValue<Vector2>();

    public Vector2 LookInput => actions.Gameplay.Look.ReadValue<Vector2>();

    public bool JumpPressed => actions.Gameplay.Jump.WasPressedThisFrame();

    public bool FireHeld => actions.Gameplay.Fire.IsPressed();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        actions = new PlayerInputActions();
        actions.Enable();
    }
}
