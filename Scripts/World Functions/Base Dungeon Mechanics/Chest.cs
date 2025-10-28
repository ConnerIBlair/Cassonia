using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private bool playerInteracting;

    public float floatSpeed = 1;

    public Sprite openedSprite;
    public Sprite closedSprite;
    private SpriteRenderer sRenderer;

    public Item heldItem;

    private Transform player;

    public SpriteRenderer floatyObject;
    private bool opened;

    [SerializeField]
    private DialogueActivator dialogueActivator;

    [SerializeField]
    private DialogueObject obtainedDialogue;
    [SerializeField]
    private DialogueObject fullDialogue;

    private void Start()
    {
        sRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (opened)
        {
            sRenderer.sprite = openedSprite;
        }
    }

    private void Update()
    {
        if (opened)
        {
            if (player.position.y > transform.position.y)
            {
                sRenderer.sortingOrder = 2;
            }
            else
            {
                sRenderer.sortingOrder = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !opened && other.GetComponent<PlayerMovement>().Interactable == null)
        {
            other.GetComponent<PlayerMovement>().Interactable = this;
            player = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PlayerMovement>().Interactable == this)
        {
            other.GetComponent<PlayerMovement>().Interactable = null;
        }
    }

    public void Interact(PlayerMovement player)
    {
        if (playerInteracting == true)
        {
            player.currentState = PlayerState.walk;
            playerInteracting = false;
            floatyObject.gameObject.SetActive(false);
            return;
        }

        if (opened == true)
        {
            return;
        }

        playerInteracting = true;
        floatyObject.gameObject.SetActive(true);

        StartCoroutine(FloatCo());
        sRenderer.sprite = openedSprite;
        player.Inventory.SetActive(true);
        if (InventoryManager.Singleton.AddItem(heldItem) == true) // Used to be player.Inventory.GetComponentInParent<InventoryManager>().AddItem(heldItem) == true
        {
            obtainedDialogue.Dialogue[0] = $"You obtained the {heldItem.name}!";
            dialogueActivator.UpdateDialogueObject(obtainedDialogue);
            if (heldItem.itemTag == SlotTag.Item)
            {
                Transform copyItem = Instantiate(heldItem.equipmentPrefab).transform;
                copyItem.parent = GameObject.Find("/Player/Available_Items").transform;
                copyItem.localPosition = Vector3.zero;
            }
            heldItem = null;
            opened = true;
        }
        else
        {
            dialogueActivator.UpdateDialogueObject(fullDialogue);
            sRenderer.sprite = closedSprite;
            floatyObject.gameObject.SetActive(false);
            //animator.SetBool("Open", false);
        }
        dialogueActivator.Interact(player.GetComponent<PlayerMovement>());
        player.Interactable = this;
        player.Inventory.SetActive(false);
    }

    private IEnumerator FloatCo()
    {
        floatyObject.sprite = heldItem.sprite;
        for (int i = 0; i < 50 / floatSpeed; i++)
        {
            floatyObject.transform.Translate(Vector2.up * Time.deltaTime * floatSpeed, Space.Self);
            yield return new WaitForFixedUpdate();
        }
    }
}