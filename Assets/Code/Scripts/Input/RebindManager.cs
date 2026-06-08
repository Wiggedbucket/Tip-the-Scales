using UnityEngine;
using UnityEngine.InputSystem;

public class RebindManager : MonoBehaviour
{
    private PlayerInputActions actions;

    public void StartRebind(InputAction action, int bindingIndex)
    {
        action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse/position")
            .OnComplete(op =>
            {
                SaveBindings();

                op.Dispose();
            })
            .Start();
    }
}