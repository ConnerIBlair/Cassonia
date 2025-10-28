using UnityEngine;
using System.Collections;

public class NPCActions : MonoBehaviour
{
    public SpriteRenderer spriteR;
    public Sprite original;
    public Sprite feetSniff;

    private PlayerMovement player;

    public Item heldItem;

    [SerializeField]
    private bool run;
    private float speed;

    [SerializeField]
    private NPCController npcController;
    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        if (run)
        {
            RunToPlayer();
        }
    }

    public void FeetSniff()
    {
        StartCoroutine(SniffCo());
    }
    private IEnumerator SniffCo()
    {
        spriteR.sprite = feetSniff;
        yield return new WaitForSeconds(2);
        spriteR.sprite = original;
    }

    public void Treat()
    {
        player.Inventory.SetActive(true);
        if (InventoryManager.Singleton.AddItem(heldItem) == true) // Used to be player.Inventory.GetComponentInParent<InventoryManager>().AddItem(heldItem) == true
        {
            Transform copyItem = Instantiate(heldItem.equipmentPrefab).transform;
            copyItem.transform.position = player.transform.position;
            copyItem.parent = GameObject.Find("/Player/Available_Items").transform;
        }
        player.Inventory.SetActive(false);
    }

    public void RunToPlayer()
    {
        npcController.acting = true;
        run = true;
        speed = npcController.speed * 1.5f;
    }
    private void FixedUpdate()
    {
        if (run)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            string walkDir = npcController.FacePlayer(player); 
            npcController.animator.Play("Npc_Walk_" + walkDir);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (run)
            {
                run = false;
                npcController.acting = false;
                speed = 0;
                player.Interactable = npcController;
                npcController.Interact(player);
            }
        }
    }
}