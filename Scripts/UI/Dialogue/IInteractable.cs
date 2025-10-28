using UnityEngine;

public interface IInteractable
{
    void Interact(PlayerMovement player);

    GameObject gameObject { get; }
}
