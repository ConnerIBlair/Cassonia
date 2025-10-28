using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SoccerPlayer : MonoBehaviour
{   
    public Transform ball;

    public bool paused;

    [SerializeField]
    private bool playerOne;

    [SerializeField]
    private float speed;
    [SerializeField]
    private SoccerKick kick;

    private Rigidbody2D rb;
    private Vector3 change;
    public Vector2 lastDir;
    public PlayerState currentState;
    [HideInInspector]
    public PlayerAnimator pAnimator;

    [SerializeField]
    private GameObject Menu;

    [SerializeField] private DialogueUI dialogueUI;

    public DialogueUI DialogueUI => dialogueUI;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pAnimator = GetComponent<PlayerAnimator>();
        currentState = PlayerState.walk;
    }

    private void Update()
    {
        if (paused)
        {
            return;
        }

        if (dialogueUI.isOpen)
        {
            change = Vector3.zero;
            pAnimator.currentAction = PlayerAction.Idle;
            pAnimator.MakePlayerMove();
            return; // This prevents any code later on from executing if the dialogue is open
        }

        if (currentState != PlayerState.interact && currentState != PlayerState.roll)
        {
            change = Vector3.zero;
            if (playerOne)
            {
                change.x = Input.GetAxisRaw("Horizontal");
                change.y = Input.GetAxisRaw("Vertical");

                if (Input.GetButtonDown("Attack") && currentState != PlayerState.attack)
                {
                    currentState = PlayerState.attack;
                    pAnimator.currentAction = PlayerAction.Action;

                    pAnimator.MakePlayerMove();
                    StartCoroutine(kick.KickCo());
                }
            }
            else
            {
                change.x = Input.GetAxisRaw("Horizontal 2");
                change.y = Input.GetAxisRaw("Vertical 2");

                if (Input.GetButtonDown("attack") && currentState != PlayerState.attack)
                {
                    currentState = PlayerState.attack;
                    pAnimator.currentAction = PlayerAction.Action;

                    pAnimator.MakePlayerMove();
                    StartCoroutine(kick.KickCo());
                }
            }
            if (Input.GetButtonDown("Menu"))
            {
                Menu.SetActive(true);
                currentState = PlayerState.interact;
            }
        }
        else
        {
            if (Input.GetButtonDown("Menu"))
            {
                Menu.SetActive(false);
                currentState = PlayerState.walk;
            }
        }
    }

    private void FixedUpdate()
    {
        if (paused)
        {
            return;
        }
        if (currentState == PlayerState.walk)
        {
            UpdateAnimationAndMove();
        }
    }

    public void UpdateAnimationAndMove() // Wasn't public before
    {
        if (change != Vector3.zero)
        {
            pAnimator.currentAction = PlayerAction.Run;

            MoveCharacter();
            pAnimator.Horizontal = change.x;
            pAnimator.Vertical = change.y;
            lastDir = new Vector2(change.x, change.y);
        }
        else
        {
            currentState = PlayerState.walk;
            pAnimator.currentAction = PlayerAction.Idle;
        }

        pAnimator.MakePlayerMove();
    }

    void MoveCharacter()
    {
        rb.MovePosition(transform.position + change.normalized * speed * Time.deltaTime);
    }
}