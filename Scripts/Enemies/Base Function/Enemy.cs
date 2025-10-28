  using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Enemy : BridgeInteraction
{
    //public enum EnemyState // Implement this
    //  {
    //      Burned,
    //      Shocked,
    //      Paused,
    //      Stunned,
    //      Moving,
    //  }

    [SerializeField]
    protected int speed;
    [SerializeField]
    protected int attackDamage;
    [SerializeField]
    protected int maxHealth = 2;
    public int health;
    public int defense;
    public float KBForce;

    public bool invincible;
    [SerializeField]
    private int InvincibilityFrames;
    public Animator animator;
    public VolumeScript volumeScript;
    public PlayerMovement player;
    protected Transform pTransform;
    public bool paused;

    public bool onFire = false;
    public bool isShocked = false;

    [SerializeField] private Shader whiteShader;
    private Shader defaultShader;
    public SpriteRenderer childObjectShaderChange;

    [SerializeField]
    private AudioClip shockedSound;
    [SerializeField]
    private AudioClip hurtSound;
    [SerializeField]
    private AudioClip deathSound;

    public bool beingHit;

    [SerializeField]
    private GameObject deathAnimObj;

    private new void Start()
    {
        startingLayer = gameObject.layer;
        health = maxHealth;
        //animator = GetComponent<Animator>();
        player = FindFirstObjectByType<PlayerMovement>();
        pTransform = player.transform;
        //sRenderer = gameObject.GetComponent<SpriteRenderer>();
        defaultShader = sRenderer.material.shader;
        volumeScript = FindFirstObjectByType<VolumeScript>();
    }

    public void Health(int damage)
    {
        if (invincible) return;
        StartCoroutine(BeingHitCo());
        if (damage - defense > 0)
        {
            health -= damage - defense;

            if (health <= 0)
            {
                StopAllCoroutines();
                StartCoroutine(DeathCo());
            }
            else
            {
                StartCoroutine(InvincibleCo());
                whiteSprite();
            }
        }
    }
    private IEnumerator BeingHitCo()
    {
        beingHit = true;
        yield return new WaitForSeconds(.3f);
        beingHit = false;
    }

    private void whiteSprite()
    {
        sRenderer.material.shader = whiteShader;
        sRenderer.color = Color.white;
        if (childObjectShaderChange != null)
        {
            childObjectShaderChange.material.shader = whiteShader;
            childObjectShaderChange.color = Color.white;
        }

        //Color pink = new Color(1, .66f, .66f);

        //myRenderer.color = pink;

    }

    public void normalSprite() // Triggered in Knockback script
    {
        sRenderer.material.shader = defaultShader;
        sRenderer.color = Color.white;
        if (childObjectShaderChange != null)
        {
            childObjectShaderChange.material.shader = defaultShader;
            childObjectShaderChange.color = Color.white;
        }
    }

    public IEnumerator ShockedCo(float speed)
    {
        isShocked = true;
        paused = true;
        yield return new WaitForSeconds(speed);
        isShocked = false;
        paused = false;
    }
    public IEnumerator FireCo(float speed)
    {
        onFire = true;
        paused = true;
        yield return new WaitForSeconds(speed);
        onFire = false;
        paused = false;
    }
    public IEnumerator WindCo(float speed)
    {
        onFire = true;
        paused = true;
        yield return new WaitForSeconds(speed);
        onFire = false;
        paused = false;
    }

    private IEnumerator InvincibleCo()
    {
        if (!isShocked)
        {
            volumeScript.PlayEffect(hurtSound, 1);
        } else 
        {
            volumeScript.PlayEffect(shockedSound, 1);
        }

        invincible = true;
        for (int i = 0; i < InvincibilityFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        invincible = false;
        normalSprite(); // Failsafe
    }

    private IEnumerator DeathCo()
    {
        speed = 0;
        attackDamage = 0;
        paused = true;
        GetComponent<Rigidbody2D>().simulated = false;
        deathAnimObj.SetActive(true);
        if (childObjectShaderChange != null) { childObjectShaderChange.enabled = false; } // is a sprite renderer for the spear usually
        sRenderer.enabled = false;
        yield return new WaitForSeconds(.1f);  // Originally .33f and two lines below non-existant
        normalSprite(); // Failsafe
        volumeScript.PlayEffect(deathSound, 1);
        //yield return new WaitForSeconds(.23f);

        yield return new WaitForSeconds(.65f); // Death anim takes .75f
        Destroy(gameObject);
    }
    public void Fall()
    {
        StartCoroutine(DeathCo());
    }
}
 