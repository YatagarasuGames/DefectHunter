using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private IInteract _interactTarget;

    public void SetInteractTarget(IInteract newInteractTarget)
    {
        if (newInteractTarget == null) throw new UnityException("newInteractTarget is null");
        else _interactTarget = newInteractTarget;

    }
    public void Interact()
    {
        _interactTarget.Interact();
    }

    private void OnDisable()
    {
        _interactTarget = null;
    }
}
