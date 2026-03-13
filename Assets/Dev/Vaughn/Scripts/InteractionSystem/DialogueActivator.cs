using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;

    private bool isDialogueActive;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerStateMachine player))
        {
            player.Interactable = this;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerStateMachine player))
        {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                player.Interactable = null;
            }
        }
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }
    public void Interact(PlayerStateMachine player)
    {
        player.DialogueUI.ShowDialogue(dialogueObject);
        isDialogueActive = true;
    }
}
