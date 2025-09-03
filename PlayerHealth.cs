using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public AudioClip deathSound;

    public int maxHealth;
    public int health;

    public int IFrames;
    public bool invincible = false;

    public FloatValue currentHealth;
    public Signal playerHealthSiganl;

    [SerializeField] private SpriteRenderer myRenderer;
    [SerializeField] private Shader whiteShader;
    private Shader defaultShader;

    private void Start()
    {
        health = maxHealth;
        defaultShader = myRenderer.material.shader;
    }

    public void Health(int Damage)
    {
        if (!invincible)
        {
            currentHealth.RuntimeValue -= Damage;
            playerHealthSiganl.Raise();
            if (currentHealth.RuntimeValue > 0)
            {
                StartCoroutine(InvincibleCo(IFrames));
            }

            health -= Damage;
            if (health <= 0)
            {
                StartCoroutine(DeathCo());
            }
            else
            {
                StartCoroutine(InvincibleCo(IFrames));
                WhiteSprite();
            }
        }
    }

    private void WhiteSprite()
    {
        myRenderer.material.shader = whiteShader;
        myRenderer.color = Color.white;
    }

    public void NormalSprite()
    {
        myRenderer.material.shader = defaultShader;
        myRenderer.color = Color.white;
    }

    private IEnumerator InvincibleCo(int frames)
    {
        invincible = true;
        for (int i = 0; i < frames; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        invincible = false;
        NormalSprite();
    }

    public IEnumerator DeathCo()
    {
        FindFirstObjectByType<VolumeScript>().PlayEffect(deathSound, 1);
        Debug.Log("You just died... How does that make you feel?");
        yield return null;
    }
}
