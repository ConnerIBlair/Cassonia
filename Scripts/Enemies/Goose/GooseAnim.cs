using UnityEngine;

public class GooseAnim : MonoBehaviour
{
    public string enemyName = "Goose_Soldier";
    public int enemyVarient = 1;

    public Animator animator;
    public SpriteRenderer sRenderer;

    private Transform pos;

    public int x, y;
    private float lastX, lastY;
    private string dir;
    public enum GooseState // Change these names
    {
        Moving, 
        Looking
    }
    public GooseState currentState;
    private GooseState lastState;

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
            animator.Play($"{enemyName}{enemyVarient}_{currentState}_{dir}");
        }
        //print($"{enemyName}{enemyVarient}_{currentState}_{dir}");
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
        print("Start");
        lastState = currentState;
        lastX = x; lastY = y;
        sRenderer.flipX = false;
        if (x == 1)
        {
            print("Right");
            return "Right";
        }
        if (x == -1)
        {
            //sRenderer.flipX = true;
            print("Left");
            return "Left"; // LEFT
        }
        if (y == 1)
        {
            print("Up");
            return "Up";
        }
        if (y == -1)
        {
            print("Down");
            return "Down";
        }
        print("FAILURE");
        Debug.Break();
        return "Right";
    }
}
