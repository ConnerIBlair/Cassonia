using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    public Animator animator;

    public float speed;

    public Transform[] points;
    public int currentPoint;
    private bool changePoint;
    public bool circlePatrol;

    public Vector2 currentDir;

    private bool interacting = false;

    [SerializeField]
    private DialogueActivator dialogueActivator;

    [SerializeField]
    private DialogueObject dialogue;

    private PlayerMovement player;

    public bool acting;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().Interactable = this;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PlayerMovement>().Interactable == this)
        {
            collision.GetComponent<PlayerMovement>().Interactable = null;
        }
    }
    private void Patrol()
    {
        Vector3 lastPos = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, points[currentPoint].position, speed * Time.deltaTime);
        currentDir = transform.position - lastPos;
        string walkDir = MovingDir();
        animator.Play("Npc_Walk_" + walkDir);

        Vector3 pointsDifference = transform.position - points[currentPoint].position;
        if (Mathf.Abs(pointsDifference.sqrMagnitude) < .0625)
        {
            if (changePoint == true)
            {
                changePoint = false;

                if (currentPoint + 1 == points.Length)
                {
                    if (circlePatrol)
                    {
                        currentPoint = 0;
                    }
                    else
                    {
                        currentPoint--;
                    }
                }
                else
                {
                    currentPoint++;
                }
            }
        }
        else
        {
            changePoint = true;
        }
    }

    private void Update()
    {
        if (acting) { return; }
        if (!interacting)
        {
            Patrol();
        }
        else
        {
            string idleDir = FacePlayer(player);
            animator.Play("Npc_Idle_" + idleDir);
            if (player.currentState != PlayerState.interact)
            {
                interacting = false;
            }
        }
    }
    public string FacePlayer(PlayerMovement player)
    {
        if (Mathf.Abs(transform.position.x - player.transform.position.x) > Mathf.Abs(transform.position.y - player.transform.position.y))
        {
            if (transform.position.x < player.transform.position.x)
            {
                return "Right";
            } else { return "Left"; }
        }
        else
        {
            if (transform.position.y < player.transform.position.y)
            {
                return "Up";
            }
            else { return "Down"; }
        }
    }
    public string MovingDir()
    {
        if (Mathf.Abs(currentDir.x) > Mathf.Abs(currentDir.y)) // Animate x
        {
            if (currentDir.x < 0)
            {
                return "Left";
            }
            else
            {
                return "Right";
            }
        }
        else // Animate y
        {
            if (currentDir.y < 0)
            {
                return "Down";
            }
            else
            {
                return "Up";
            }
        }
    }

    public void Interact(PlayerMovement player)
    {
        this.player = player;
        interacting = true;
        player.currentState = PlayerState.interact;
        dialogueActivator.UpdateDialogueObject(dialogue);
        dialogueActivator.Interact(player);
    }
    public void NotInteracting(){
        interacting = false;
    }
}
