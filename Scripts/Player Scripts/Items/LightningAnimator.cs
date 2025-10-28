using UnityEngine;

public class LightningAnimator : MonoBehaviour
{
    public SpriteRenderer sRenderer;
    public Sprite[] sprites;

    private float time;

    public float switchTime;
    public float deathTime;

    private int prevSprite;
    private int nextSprite;
    private void Update()
    {
        time += Time.deltaTime;
        if (time > switchTime)
        {
            switchTime += time;
            nextSprite = Random.Range(0, sprites.Length);
            while (nextSprite == prevSprite)
            {
                nextSprite = Random.Range(0, sprites.Length);
            }
            prevSprite = nextSprite;
            sRenderer.sprite = sprites[nextSprite];
        }
        if (time > deathTime) { Destroy(gameObject); }
    }
}
