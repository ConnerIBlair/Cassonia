using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// Video Spots

//
//

// IDEAS
// When saving, add an underscore then + savenumber for every script that saves something for different profiles/game saves
//
// Teleporting enemies


// To Do
//      Proper sound effects, (change which # used for JumpCo())
//      ScaleCanvasCustom
//          Make it change only when changing screen size instead of update
//      Sound Effects

// Camera transition between squares (32x32 units)
// Change Global lighting based on if in cave or marelia
// magic use for fire/wind seeds + visual in inventory and slowly regenerate magic bar
//      WIND SEEDS - When they detect if there is any object in the place where the player will land and it is inside a wall, it will not count it as a barrier
public enum PlayerState
{
    walk,
    run,
    attack,
    interact,
    roll,
    jump,
    paused
}

public class PlayerMovement : ChangeSortingOrder
{
    public AudioClip[] soundEffects; // 0: Inventory open 1: inv close 2: sword swing 3: bush pickup  4: Walking

    public float setTime;

    [SerializeField]
    private CircleCollider2D collision;

    public bool paused;
    [HideInInspector]
    public bool hasStick;

    public float speed;

    [SerializeField]
    private Rigidbody2D rb;
    private Vector3 oldPos;
    private Vector3 change;
    //[HideInInspector]
    public bool moving; // Only used when walking/running (To make wall collision not stay running) and dog
    public Vector2 lastDir;
    public PlayerState currentState;
    public PlayerAnimator pAnimator;
    [HideInInspector]
    public VolumeScript sounds;

    public PlayerDirection CurrentDir => pAnimator.currentDirection;

    public TMP_Text CoinCounter;
    private int coins;

    [SerializeField]
    private PlayerAttack pAttack;
    [SerializeField]
    private GameObject Sword;
    [SerializeField]
    private GameObject Menu;

    public GameObject Inventory;

    [SerializeField] private DialogueUI dialogueUI;

    public DialogueUI DialogueUI => dialogueUI; // This is a getter. It makes a public variable that other classes can get from, but not set.
                                                // The => tells the code it is a getter, that's why other things can't change it.
    public IInteractable Interactable { get; set; }

    private int layerMask = 1 << 7;

    private Coroutine lastCo;

    [SerializeField]
    private Tilemap wallTilemap;

    private bool transitioning = false;

    private Vector2 jumpSpot;
    [HideInInspector]
    public InGameTooltip tooltip;

    private static PlayerMovement instance = null;

    public static PlayerMovement Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        currentState = PlayerState.walk;
        sounds = FindFirstObjectByType<VolumeScript>();
        //Application.targetFrameRate = 60;
        GetComponentInChildren<UIFunctions>().LoadAll();
        tooltip=GetComponent<InGameTooltip>();
    }

    private void Update()
    {
        Vector3 direction = lastDir;
        if (paused)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            //sounds.ChangeVolume(0, 0); // Mute

            //sounds.ChangeVolume(1, 0);
            //sounds.ChangeVolume(.6f, 1);
            //sounds.ChangeVolume(1, 2);

            StartCoroutine(WaveCo());
        }

        if (dialogueUI.isOpen)
        {
            change = Vector3.zero;
            pAnimator.currentAction = PlayerAction.Idle; // This needs to be changed, Player shouldn't stop an attack because they opened up the UI
            currentState = PlayerState.interact;         // This would also break the weapon animation...
            pAnimator.MakePlayerMove();
            return; // This prevents any code later on from executing if the dialogue is open
        }

        if (Input.GetButtonUp("Interact") && currentState != PlayerState.attack) //checked on up is important! No idea why...
        {
            if (Interactable != null) // Same thing as above code
            {
                tooltip.DeActivate();
                // currentState = PlayerState.interact;  // Implement this to every interactable that needs the player to be still
                Interactable.Interact(this);
                return;
            }
            else
            {
                if (NearbyBush() == true) { return; }
                else
                {
                    currentState = PlayerState.walk;
                }
            }
        }

        if (currentState != PlayerState.interact && currentState != PlayerState.roll && 
            currentState != PlayerState.jump && currentState != PlayerState.attack) // attack state is used for sword/ item
        {
            change = Vector3.zero;
            change.x = Input.GetAxisRaw("Horizontal");
            change.y = Input.GetAxisRaw("Vertical");
            if (Interactable == null)
            {
                if (Input.GetButtonDown("Attack"))
                {
                    Sword.SetActive(true);
                    ChangeOrder(pAttack.sRenderer);
                    ChangeLayerOnly(pAttack.gameObject, true);
                    currentState = PlayerState.attack;
                    pAttack.StopCoroutine("AttackCo");
                    pAttack.StartCoroutine("AttackCo");
                    //sounds.PlayEffect(soundEffects[2]);
                }

                if (Input.GetButtonDown("Roll") && moving) // ROLL ROLL ROLL ROLL ROLL ROLL ROLL ROLL ROLL ROLL
                {
                    lastCo = StartCoroutine(RollCo());
                }
            }
            if (Input.GetButtonDown("Menu"))
            {
                Menu.SetActive(true);
                currentState = PlayerState.interact;
                pAnimator.currentAction = PlayerAction.Idle;
                pAnimator.MakePlayerMove();
            }
            if (Input.GetButtonDown("Inventory"))
            {
                Inventory.SetActive(true);
                currentState = PlayerState.interact;
                pAnimator.currentAction = PlayerAction.Idle;
                pAnimator.MakePlayerMove();
                sounds.PlayEffect(soundEffects[0]);
            }
        }
        else
        {
            if (Input.GetButtonDown("Menu"))
            {
                Menu.SetActive(false);
                currentState = PlayerState.walk;
            }
            if (Input.GetButtonDown("Inventory"))
            {
                Inventory.SetActive(false);
                currentState = PlayerState.walk;
                sounds.PlayEffect(soundEffects[1]);
            }
        }
    }

    private void FixedUpdate()
    {
        if (paused)
        {
            return;
        }
        if (MapManager.Instance.GetType(new Vector2(transform.position.x, transform.position.y - .5f)) == true)
        {
            pAnimator.hay.SetActive(true);
            if (moving)
            {
                pAnimator.hay.GetComponent<Animator>().Play("Move_Hay");
            }
            else
            {
                pAnimator.hay.GetComponent<Animator>().Play("Idle_Hay");
            }
        }
        else
        {
            pAnimator.hay.SetActive(false);
        }
        if (currentState == PlayerState.walk)
        {
            UpdateAnimationAndMove();
        }
        if (currentState == PlayerState.jump)
        {
            transform.position = Vector2.MoveTowards(transform.position, jumpSpot, Time.deltaTime * speed);
        }
        if (currentState == PlayerState.roll)
        {
            StartCoroutine(MoveCharacterCo());
        }
    }

    private void IdleAnims()
    {
        //currentState = PlayerState.walk;
        if (pAnimator.currentAction == PlayerAction.Carry || pAnimator.currentAction == PlayerAction.CarryIdle)
        {
            pAnimator.currentAction = PlayerAction.CarryIdle;
        }
        else if (pAnimator.currentAction == PlayerAction.CarryDog || pAnimator.currentAction == PlayerAction.CarryIdleDog)
        {
            pAnimator.currentAction = PlayerAction.CarryIdleDog;
        }
        else { pAnimator.currentAction = PlayerAction.Idle; }
    }

    private void MovingAnims()
    {
        if (pAnimator.currentAction == PlayerAction.Carry || pAnimator.currentAction == PlayerAction.CarryIdle)
        {
            pAnimator.currentAction = PlayerAction.Carry;
        }
        else if (pAnimator.currentAction == PlayerAction.CarryDog || pAnimator.currentAction == PlayerAction.CarryIdleDog)
        {
            pAnimator.currentAction = PlayerAction.CarryDog;
        }
        else { pAnimator.currentAction = PlayerAction.Run; }
    }

    public void ChangeState(int state)
    {
        currentState = (PlayerState)state;
    }
    public void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero) // change is actually just input
        {
            StartCoroutine(MoveCharacterCo());
            pAnimator.Horizontal = change.x;
            pAnimator.Vertical = change.y;
            lastDir = new Vector2(change.x, change.y);
            if (moving)
            {
                MovingAnims();
                WalkSound();
            }
            else
            {
                IdleAnims();
                time = 20;
            }
        }
        else
        {
            if (currentState != PlayerState.roll)
                moving = false;

            IdleAnims();
            time = 20;
        }

        pAnimator.MakePlayerMove();
    }

    private int time;
    private void WalkSound()
    {
        if (time == 20)
        {
            time = 0;
            sounds.PlayEffect(soundEffects[4]);
        }
        time++;
    }
    private IEnumerator MoveCharacterCo()
    {
        oldPos = transform.position;
        rb.MovePosition(transform.position + change.normalized * speed * Time.deltaTime);
        yield return null;
        Vector3 newPos = transform.position;
        if (oldPos == newPos)
        {
            if (CanJump() != Vector2Int.zero)
            {
                time = 0;
                StartCoroutine(JumpCo());
            }

            moving = false;
            time = 0;
        }
        else
        {
            moving = true;
        }
    }

    private IEnumerator RollCo()
    {
        if (currentState == PlayerState.roll) { yield break; }
        currentState = PlayerState.roll;
        pAnimator.currentAction = PlayerAction.Roll;
        pAnimator.MakePlayerMove();
        speed += 2;
        yield return new WaitForSeconds(.4166667f);
        speed -= 2;
        currentState = PlayerState.walk;
        pAnimator.currentAction = PlayerAction.Idle;
        pAnimator.MakePlayerMove();
    }

    private IEnumerator JumpCo()
    {
        sounds.PlayEffect(soundEffects[4]); // Replace with appropriate sound
        jumpSpot = new Vector2(transform.position.x, transform.position.y) + CanJump();
        currentState = PlayerState.jump;
        collision.enabled = false;
        yield return new WaitForSeconds(1);
        collision.enabled = true;
        currentState = PlayerState.walk;
    }

    private Vector2Int CanJump()
    {
        float y = 0;
        if (lastDir.y != 1)
        {
            y = lastDir.y;
        }
        if (currentState != PlayerState.jump && MapManager.Instance.GetJumpableState // Actually want .walk and .run here
            (new Vector2(transform.position.x + lastDir.x, transform.position.y + y)) == true)
        {
            if (MapManager.Instance.GetHeight
            (new Vector2(transform.position.x + lastDir.x, transform.position.y + y)) == 2) { return new Vector2Int(0, -3); } // Always down

            if (wallTilemap.GetTransformMatrix(new Vector3Int((int)(transform.position.x + lastDir.x),
            (int)(transform.position.y + y))).rotation.eulerAngles.z == 0 && lastDir.y == 1)
            {
                // Jump up!
                return new Vector2Int(0, 2);
            }
            if (wallTilemap.GetTransformMatrix(new Vector3Int((int)(transform.position.x + lastDir.x),
            (int)(transform.position.y + y))).rotation.eulerAngles.z == 90 && lastDir.x == -1)
            {
                // Jump left!
                return new Vector2Int(-2, 0);
            }
            if (wallTilemap.GetTransformMatrix(new Vector3Int((int)(transform.position.x + lastDir.x),
            (int)(transform.position.y + y))).rotation.eulerAngles.z == 270 && lastDir.x == 1)
            {
                // Jump right
                return new Vector2Int(2, 0);
            }
        }
        return Vector2Int.zero;
    }

    #region Old Scene Transition
    public void TeleportCharacter(Vector3 spawnCords, Scene scene)
    {
        StartCoroutine(TeleportPlayerCo(spawnCords, scene));
        pAnimator.MakePlayerMove();
    }
    private IEnumerator TeleportPlayerCo(Vector3 spawnCords, Scene currentScene)
    {
        pAnimator.Vertical = lastDir.y;
        StartCoroutine(WalkAnimCo());
        yield return new WaitUntil(() => SceneManager.GetActiveScene() != currentScene);
        gameObject.transform.position = spawnCords;
        StartCoroutine(WalkAnimCo(2));
        dialogueUI.gameObject.GetComponent<UIFunctions>().LoadScene(-1, 1, 0); // SceneToLoad, start(Alpha color), end(Alpha color)
        yield return new WaitForSeconds(setTime);
        currentState = PlayerState.walk;
        collision.enabled = true;
    }
    private IEnumerator WalkAnimCo(int modify = 1)
    {
        collision.enabled = false;
        pAnimator.MakePlayerMove();
        Vector3 lastDirD = new(0, pAnimator.Vertical, 0);

        for (float i = 0; i < setTime / modify; i += Time.deltaTime)
        {
            rb.MovePosition(transform.position + lastDirD * speed / 3 * Time.deltaTime); // put a shadow at the cave entrance
            yield return new WaitForFixedUpdate();
        }
        if (modify > 1)
        {
            pAnimator.currentAction = PlayerAction.Walk;
            currentState = PlayerState.walk;
            pAnimator.MakePlayerMove();
        }
        GetComponentInChildren<UIFunctions>().playerSpawn = transform.position;
        GetComponentInChildren<UIFunctions>().SaveAll();
    }
    #endregion

    public void TransitionScene(string ID, int sceneToLoad, PixelationTransition effect, AudioClip soundEffect)
    {
        if (transitioning) { return; }
        transitioning = true;
        if (soundEffect != null)
        {
            sounds.PlayEffect(soundEffect);
        }
        StartCoroutine(TransitionCo(ID, sceneToLoad, effect));
    }
    private IEnumerator TransitionCo(string ID, int sceneToLoad, PixelationTransition effect) // Looks at the player position, not at his feet
    {
        Scene currentScene = SceneManager.GetActiveScene();
        //currentState = PlayerState.interact;
        paused = true;

        StartCoroutine(PlayerSceneMovingCo(lastDir));

        UnityEngine.AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad); //start async loading
        asyncOperation.allowSceneActivation = false;
        // While moving start effect
        yield return new WaitForSeconds(.45f);
        effect.FadeOut(); // .55 seconds
        yield return new WaitForSeconds(.55f);
        // Once effect is done, try to load scene

        asyncOperation.allowSceneActivation = true;
        yield return new WaitUntil(() => SceneManager.GetActiveScene() != currentScene);

        MapManager.Instance.tilemaps = FindFirstObjectByType<Grid>();
        MapManager.Instance.map = GameObject.Find("Tilemaps/Walls").GetComponent<Tilemap>();
        // Once scene is loaded, find SceneTransition object with same ID
        SceneTransitions[] transitions = FindObjectsByType<SceneTransitions>(FindObjectsSortMode.InstanceID);
        SceneTransitions transition = transitions[0]; // Just so it teleports somewhere and doesn't break...
        foreach (SceneTransitions obj in transitions)
        {
            if (obj.ID == ID)
            {
                transition = obj;
                break;
            }
        }
        // Change player dir + pos
        Vector3 direction = lastDir;
        lastDir = Vector2.zero;
        change = Vector3.zero;
        pAnimator.PlayAnimation(PlayerAction.Run, transition.exitMovement);

        // REPLACE THIS IMMEDIATELYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY because it should be behind it and you walk to just in front of transition object (Will be changed once MovePlayerCo is implemented)
        transform.position = new Vector2(transition.transform.position.x, transition.transform.position.y - .25f) + LastDirCalc(transition.exitMovement.ToString());

        StartCoroutine(PlayerSceneMovingCo(direction));

        transition.player = this.transform; // Starting effect code-block
        transition.CameraPositionChange(); // Set camera pos and borders
        transition.cameraS.GetComponent<Camera>().backgroundColor = transition.cameraS.colours.ColourSwatches[sceneToLoad];
       // effect.thisCamera.GetComponent<Camera>().backgroundColor = transition.cameraS.colours.ColourSwatches[sceneToLoad];
        yield return new WaitForSeconds(.5f);
        effect.FadeIn();
        yield return new WaitForSeconds(.5f);

        pAnimator.currentAction = PlayerAction.Idle; // This will be replaced by MovePlayer/WalkAnimCo
        currentState = PlayerState.walk; // This will be replaced by MovePlayer/WalkAnimCo
        transitioning = false;
        paused = false;
        pAnimator.PlayAnimation(PlayerAction.Idle, transition.exitMovement);
        yield return new WaitForFixedUpdate();
        // All Done!
    }

    private IEnumerator PlayerSceneMovingCo(Vector3 direction)
    {
        for (int i = 0; i < 52; i++)
        {
            rb.MovePosition(transform.position + direction.normalized * Time.deltaTime);
            yield return null;
        }
    }

    private Vector2 LastDirCalc(string dir)
    {
        switch (dir)
        {
            case "Down":
                return new Vector2(0, -1);
            case "Up":
                return new Vector2(0, 1);
            case "Right":
                return new Vector2(1, 0);
            case "Left":
                return new Vector2(-1, 0);
        }
        return Vector2.zero;
    }

    public void CoinCount(int coins)
    {
        this.coins += coins;
        CoinCounter.text = $"{this.coins}";
    }

    private bool NearbyBush() // Or any throwable (Object with IInteractable and Throwable Tag)
    {
        int x = 0;
        int y = 0;

        switch (pAnimator.currentDirection)
        {
            case PlayerDirection.Down:
                y = -1;
                break;

            case PlayerDirection.Up:
                y = 2;
                break;

            case PlayerDirection.Left:
                x = -1;
                break;

            case PlayerDirection.Right:
                x = 1;
                break;
        }

        Vector2 frontPos = new(x, y);
        int layerMask = 1 << 0;
        Vector2 startPos = new(transform.position.x, transform.position.y - .25f);
        RaycastHit2D hit = Physics2D.Raycast(startPos, frontPos, 1, layerMask);
        if (hit.collider != null && hit.collider.CompareTag("Throwable"))
        {
            if (hit.collider.GetComponent<Bush>().dead == true)
            {
                return false;
            }
            if (Interactable == null)
            {                // Is something you can pick up and you aren't already interacting with something
                Interactable = hit.collider.GetComponent<IInteractable>();
                currentState = PlayerState.interact;
                pAnimator.currentAction = PlayerAction.Lift;
                sounds.PlayEffect(soundEffects[3]);
                Interactable.Interact(this);
                pAnimator.MakePlayerMove();
                return true;
            }
            else
            {
                Interactable.Interact(this);
                return false;
            }
        }
        return false;
    }

    private IEnumerator WaveCo()
    {
        currentState = PlayerState.interact;
        pAnimator.currentAction = PlayerAction.Wave;
        pAnimator.MakePlayerMove();
        yield return new WaitForSeconds(.5f);
        currentState = PlayerState.walk;
        pAnimator.currentAction = PlayerAction.Idle;
    }

    public IEnumerator WindJump()
    {
        jumpSpot.x = transform.position.x + speed * .5f * lastDir.x;
        jumpSpot.y = transform.position.y + speed * .5f * lastDir.y;
        Collider2D hit = Physics2D.OverlapCircle(jumpSpot, .5f);
        
        int origSpd = 0; // this block was beneath hit != null and had collision.isTrigger = true
        if (currentState == PlayerState.roll) { origSpd = 2; StopCoroutine(lastCo); } //Stop the RollCo();
        if (currentState == PlayerState.jump) { yield break; }
        currentState = PlayerState.jump;
        pAnimator.currentAction = PlayerAction.Jump;
        pAnimator.MakePlayerMove();
        GetComponent<PlayerHealth>().invincible = true;

        if (hit != null) // used to be no else
        {
            if (hit.isTrigger != true) // used to be no else and had yield break;
            {
                collision.isTrigger = false;
            }
            else
            {
                collision.isTrigger = true;
            }
        }
        else
        {
            collision.isTrigger = true;
        }

        yield return new WaitForSeconds(.5f);
        speed -= origSpd;
        collision.isTrigger = false;

        currentState = PlayerState.walk;
        pAnimator.currentAction = PlayerAction.Idle;
        pAnimator.MakePlayerMove();
        GetComponent<PlayerHealth>().invincible = false;
    }
}
