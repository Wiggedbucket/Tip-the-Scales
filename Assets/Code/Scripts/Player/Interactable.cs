using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected float interactionDistance = 3f;

    [SerializeField] protected string interactionText = "Interact";

    public float InteractionDistance => interactionDistance;
    public string InteractionText => interactionText;

    public virtual void Interact(GameObject interactor)
    {
    }
}