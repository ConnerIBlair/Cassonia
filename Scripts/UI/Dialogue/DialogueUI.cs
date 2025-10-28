using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    // Main component responsible for showing text  Attatched to the canvas

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;

    public bool isOpen { get; private set; }

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;

    [HideInInspector]
    public DialogueActivator currentActivator;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        CloseDialogue();
    }

    public void ShowDialogue(DialogueObject dialogueObject) //Allows for a waiting period between each dialogue that shows up.
    {
        isOpen = true;
        dialogueBox.SetActive(true);

        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];
            yield return RunTypingEffectCo(dialogue);

            textLabel.text = dialogue;

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses)
            {
                break;
            }

            yield return null; // This is here because 20 lines down also waits for Interact key to be pressed, don't want to be skipping dialogue on accident
            yield return new WaitUntil(() => Input.GetButtonDown("Interact"));
        }

        if (dialogueObject.HasResponses)
        {
            foreach (DialogueResponseEvents responseEvents in currentActivator.GetComponents<DialogueResponseEvents>())
            { // This allows for 2 deep events to happen!!!!!!!
                if (responseEvents.DialogueObject == dialogueObject)
                {
                    AddResponseEvents(responseEvents.Events);
                    break;
                }
            }
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogue();
        }
    }

    private IEnumerator RunTypingEffectCo(string dialogue)
    {
        typewriterEffect.Run(dialogue, textLabel);


        while (typewriterEffect.IsRunning)
        {
            yield return null;
            if (Input.GetButtonDown("Interact"))
            {
                typewriterEffect.Stop();
                textLabel.maxVisibleCharacters = dialogue.Length;
            }
        }
    }

    public void CloseDialogue()
    {
        isOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
        // Make player able to move
    } 
} 
