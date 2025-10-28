using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SoccerGoal : MonoBehaviour
{
    [SerializeField]
    private Transform ball;

    private enum Team { Home, Guest }

    [SerializeField]
    private Team team;

    public TMP_Text winText;
    public TMP_Text scoreText;
    public GameObject scoreboard;

    [SerializeField]
    private Animator animator;

    private int score;

    [SerializeField]
    private SoccerPlayer player;

    [SerializeField]
    private SoccerPlayer player2;

    public AISoccerPlayer[] players;

    private bool canScore = true;

    [SerializeField]
    private Vector2 ballPosition;
    [SerializeField]
    private Vector2[] homePositions;
    [SerializeField]
    private Vector2[] guestPositions;

    private bool winner = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == ball && canScore)
        {
            scoreboard.SetActive(true);
            canScore = false;
            scoreText.text = $"{team} {score += 1}";
            if (score >= 5)
            {
                StartCoroutine(WinCo());
                return;
            }
            StartCoroutine(DisplayCo());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && winner == true)
        {
            winner = false;
            score = 0;
            winText.text = "Score";
            scoreboard.SetActive(false);
            ResetPos();
        }
    }

    private IEnumerator DisplayCo()
    {
        winText.text = "Score";
        player.paused = true;
        player.pAnimator.currentAction = PlayerAction.Idle;
        player.pAnimator.MakePlayerMove();
        //player2.paused = true;
        //player2.pAnimator.MakePlayerMove();


        foreach (AISoccerPlayer AI in players)
        {
            AI.paused = true;
            AI.animator.currentAction = PlayerAction.Idle;
            AI.animator.MakePlayerMove();
        }

        animator.Play("FadeInScore");
        yield return new WaitForSeconds(5);
        player.paused = false;
        player2.paused = false;
        scoreboard.SetActive(false);

        foreach (AISoccerPlayer AI in players)
        {
            AI.paused = false;
        }
        canScore = true;

        ResetPos();
    }

    private void ResetPos()
    {
        ball.position = ballPosition;

        player.transform.position = guestPositions[0];
        //player2.transform.position = homePositions[0];
        players[0].transform.position = homePositions[0];

        // Reset Positions
        // Player can choose to be Defender, Midfield, or Forward.
        // Positions based on that
    }



    private IEnumerator WinCo()
    {
        winner = true;
        foreach (AISoccerPlayer AI in players)
        {
            AI.paused = true;
        }
        if (team == Team.Home)
        {
            winText.text = "Home Team Wins!";
        }
        else
        {
            winText.text = "Guest Team Wins!";
        }
        animator.Play("FadeInScore");
        yield return new WaitForSeconds(2);
        animator.Play("FadedInScore");
    } 
}
