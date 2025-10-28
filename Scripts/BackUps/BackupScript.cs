

#region Title Camera Fade

//private void Start()
//{
//    this._asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad); // Begin to load the scene you want to
//    this._asyncOperation.allowSceneActivation = false; // Prevent it from loading

//    image.gameObject.SetActive(true);
//    StartCoroutine(FadeTransitionCo(1, 0, timeToFade));

//    //image.color = new Color(1.0f, 1.0f, 1.0f, 0);
//}


//public void StartGame()
//{
//    image.gameObject.SetActive(true);
//    volumeS.PlaySong(null, false);
//    StartCoroutine(FadeTransitionCo(image.color.a, 1, fadeInTime));
//}

//private IEnumerator FadeTransitionCo(float start, float end, float fadeTime)
//{
//    for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
//    {
//        //change color as you want
//        var newColor = new Color(0, 0, 0, Mathf.Lerp(start, end, t)); // alpha, 1 (StartGame())     1, 0 (FadeInCo)
//        image.color = newColor;

//        yield return null;
//    }
//    if (end != 0)
//    {
//        this._asyncOperation.allowSceneActivation = true;
//    }
//    else
//    {
//        image.gameObject.SetActive(false);
//    }

//}

#endregion

#region Camera Follow
//private void FixedUpdate()
//{
//    if (transform.position != target.position)
//    {
//        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

//        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
//        targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

//        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);

//    }
//}
#endregion

#region Load Scene Function
//public void LoadScene(float timeToFade, int sceneToLoad)
//{
//    StartCoroutine(FadeTransitionCo(0, 1, timeToFade, sceneToLoad));
//}
//private IEnumerator FadeTransitionCo(float start, float end, float fadeTime, int sceneToLoad)
//{
//    for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
//    {
//        var newColor = new Color(0, 0, 0, Mathf.Lerp(start, end, t));
//        image.color = newColor;

//        yield return null;
//    }
//    if (end == 1)
//    {
//        SceneManager.LoadScene(sceneToLoad);
//    }
//}
#endregion

#region Knockback
//public Enemy enemyS;
//    public float force;

//    public Rigidbody2D rb;

//    public Transform obj2;

//    private float differenceX;
//    private float differenceY;
//    private float radians;
//    private float angle;


//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        obj2 = other.transform;]
//        KnockbackObject(obj2);
//    }

//    public void KnockbackObject(Transform obj, Vector3 optDir = new Vector3())
//    {
//        rb = obj.GetComponent<Rigidbody2D>();
//        enemyS = obj.gameObject.GetComponent<Enemy>();

//        if (optDir != new Vector3())
//        {
//            optDir += transform.position;
//        }
//        else
//        {
//            getAngle(obj.transform);
//        }


//        if (enemyS)
//            enemyS.paused = true;

//        transform.eulerAngles = new Vector3(0, 0, angle); // transform used to be collider turner
//        rb.velocity = -this.gameObject.transform.right * force; //rb.velocity.magnitude * force; // transform used to be collider turner
//        StartCoroutine(WaitCo());
//    }

//    public IEnumerator WaitCo()
//    {
//        yield return new WaitForSeconds(0.5f);
//        rb.velocity = Vector2.zero;
//        if (enemyS)
//        {

//            if (enemyS.health > 0)
//            {
//                enemyS.paused = false;
//            }
//            enemyS = null;
//        }
//        obj2 = null;
//        rb = null;
//    }

//    private void getAngle(Transform obj2)
//    {
//        differenceX = transform.position.x - obj2.position.x; // transform used to be obj1
//        differenceY = transform.position.y - obj2.position.y; // transform used to be obj1
//        radians = Mathf.Atan2(differenceY, differenceX);
//        angle = radians * 57.296f;
//    }
#endregion