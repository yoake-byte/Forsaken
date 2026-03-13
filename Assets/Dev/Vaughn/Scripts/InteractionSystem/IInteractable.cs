public interface IInteractable
{
    void Interact(PlayerStateMachine player);
    bool CanInteract();
}
