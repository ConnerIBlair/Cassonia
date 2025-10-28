using UnityEngine;
using UnityEngine.UIElements;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;

    private SpriteRenderer sRenderer;
    private Transform player;

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }

    private void Start()
    {
        sRenderer = GetComponentInParent<SpriteRenderer>();
        player = FindFirstObjectByType<PlayerMovement>().transform;
    }

    private void Update()
    {
        if (sRenderer == null) { return; }
        if (player.position.y > transform.position.y)
        {
            sRenderer.sortingOrder = 2;
        }
        else
        {
            sRenderer.sortingOrder = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement player) && player.Interactable == null)
        {
            player.Interactable = this;
            player.tooltip.Activate();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement player))
        {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
                // This makes sure that if there are multiple interactbles 
                // in a small space that this is the activator being used before closing the interactable.
            {
                player.Interactable = null;
                player.tooltip.DeActivate();
            }
        }
    }

    public void Interact(PlayerMovement player) // Look to IInteractable and the first part of update in PlayerMovement
    {
        player.Interactable = this;
        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        { // This used to be an if statement, only allowed for one event per object with dialogue.
          // Now it checks through all the events with that line of dialogue to see all the
          // possible events and once it finds the right one, it breaks out of the for loop.
            Debug.Log(dialogueObject + " What");
            if (responseEvents.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }
        player.DialogueUI.currentActivator = this;
        player.DialogueUI.ShowDialogue(dialogueObject); // This function (Interact) Is activated by the player
                                                        // This part is the code that the Object is executing. Put different code here for different objects
        player.Interactable = null;
    }
}
