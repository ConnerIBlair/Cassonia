using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{ // Somehow the animator speed was 2 while the goose was in the looking anim... and the looking state
    public string enemyName = "Goose_Soldier";
    public int enemyVarient = 1;

    public Animator animator;
    public SpriteRenderer sRenderer;

    private Transform pos;

    public int x, y;
    private int lastX, lastY;
    private string dir = "Left";
    public enum EnemyState // Change these names
    {
        Idle,
        Pace,
        Move, // Goose
        Attack,
        Look, // Goose
        Transition
    }
    public EnemyState currentState = EnemyState.Move;
    private EnemyState lastState;

    private void Start()
    {
        pos = GetComponent<Transform>();
    }

    public void FixedUpdate()
    {
        if (lastX != x || lastY != y && XYNot0())
        {
            dir = WhichDir();
            animator.Play($"{enemyName}{enemyVarient}_{currentState}_{dir}"); 
            return;
        }
        if (currentState != lastState)
        {
            lastState = currentState;
            animator.Play($"{enemyName}{enemyVarient}_{currentState}_{dir}");
        }
    }
    private bool XYNot0()
    {
        if (x != 0 || y != 0)
        {
            return true;
        }
        return false;
    }
    private string WhichDir()
    {
        lastState = currentState;
        lastX = x; lastY = y;
        sRenderer.flipX = false;
        if (x == 1)
        {
            return "Right";
        }
        if (x == -1)
        {
            return "Left";
        }
        if (y == 1)
        {
            return "Up";
        }
        if (y == -1)
        {
            return "Down";
        }
        return "Right";
    }
}
