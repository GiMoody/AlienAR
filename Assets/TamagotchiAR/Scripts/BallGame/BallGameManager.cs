using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleARCore;


public class BallGameManager : MonoBehaviour
{

    /// <summary>
    /// Total duration of the mini-game in terms of fetchs
    /// </summary>
    public float MaxNumberFetch;

    public Camera FirstPersonCamera;
    public GameObject scoreGUI;

    /// <summary>
    /// Max delay between the choosing of the next destination
    /// </summary>
    public float MaxDelayFetch;

    /// <summary>
    /// Text to modify
    /// </summary>
   // public GameObject scoreGUI;

    private GameObject player;
    //private GameObject alien;
    private NavMeshAgent navMeshAgent;
    private GameObject ballToMove;
    private static int TotalScore;

    /// <summary>
    /// Coroutine that stop the game after a fixed time
    /// </summary>
    private Coroutine _stopGame;

    public delegate void BallGameComplete(int score);
    public static event BallGameComplete OnBallGameFinished;

    private bool isBallGameActive = false;

    public static BallGameManager instance = null;

    /// <summary>
    /// Various variables used
    /// </summary>
    private bool reachDestination;
    private bool setDestination;
    private float timer;


    /// <summary>
    /// The Unity Awake() method
    /// Implementing the singleton pattern for the MiniGameController
    /// </summary>
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }


    /// <summary>
    /// Metodo che inizializza le variabili per il BT e fa partire il minigioco
    /// </summary>
    /// <returns></returns>
    public int StartBallGame()
    {
        if (isBallGameActive) return -1;
        isBallGameActive = true;
        TotalScore = 0;

        player = GameObject.FindGameObjectWithTag("Alien_Baby");//Find("AlienGO_Final_Test");
        navMeshAgent = GameObject.FindGameObjectWithTag("Player").GetComponent<NavMeshAgent>();
        ballToMove = GameObject.Find("PallaBella(Clone)");
        //ballToMove = GameObject.Find("palla");
        reachDestination = true;
        setDestination = false;
        timer = 0;

        scoreGUI.GetComponent<CanvasGroup>().alpha = 1.0f;
        scoreGUI.GetComponent<Text>().text = ("Current Fetch Score: " + TotalScore + "/" + MaxNumberFetch);
        //scoreGUI.GetComponent<CanvasGroup>().alpha = 1.0f;
        // calculate directional vector to target

        return 0;
    }


    // Update is called once per frame
    void Update()
    {

        if ((GameManager.instance.CurrentGameStatus != (int)GameManager.GameStatus.Game)) return;
        if (!isBallGameActive) return;
        if (TotalScore >= MaxNumberFetch)
        {
            //Se passa la palla per il numero prefissato vince, massimo punteggio
            scoreGUI.GetComponent<Text>().text = ("Ottimo lavoro!");

            TotalScore = 2;
            StartCoroutine(EndGame());
        }

        if (reachDestination)
        {
            timer += Time.deltaTime;
            if (timer >= MaxDelayFetch)
            {
                int currentScore = TotalScore;
                if (TotalScore >= (int)MaxNumberFetch * 0.5f)
                {
                    scoreGUI.GetComponent<Text>().text = ("Non male");
                    TotalScore = 1;
                }
                else
                {
                    scoreGUI.GetComponent<Text>().text = "Ops...";
                    TotalScore = 0;
                }
                StartCoroutine(EndGame());



            }
            else
            {
                if (Input.touchCount > 0)
                {

                    int layerMask = 1 << 8;

                    // This would cast rays only against colliders in layer 8.
                    // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
                    layerMask = ~layerMask;
                    TrackableHit rayhit;
                    TrackableHitFlags raycastFilter;
                    Touch touch = Input.GetTouch(0);
                    Vector3 touchPos3D = FirstPersonCamera.ScreenToWorldPoint(touch.position);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                        case TouchPhase.Stationary:
                        case TouchPhase.Moved:
                            raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
                            if (Session.Raycast(touch.position.x, touch.position.y, raycastFilter, out rayhit))
                            {
                                Vector3 dest = new Vector3(rayhit.Pose.position.x, ballToMove.transform.position.y, rayhit.Pose.position.z);
                                ballToMove.transform.position = dest;

                                Vector3 targetDir = dest - player.transform.position;
                                float speed = 40.0f;
                                float step = speed * Time.deltaTime;
                                Vector3 newDir = Vector3.RotateTowards(player.transform.forward, targetDir, step, 0.0F);
                                navMeshAgent.transform.rotation = Quaternion.LookRotation(newDir);
                            }
                            break;

                        case TouchPhase.Ended:

                            RaycastHit hit;
                            Ray ray = FirstPersonCamera.ScreenPointToRay(touch.position);
                            raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
                            if (Session.Raycast(touch.position.x, touch.position.y, raycastFilter, out rayhit) && (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)))
                            {
                                Vector3 dest = new Vector3(rayhit.Pose.position.x, ballToMove.transform.position.y, rayhit.Pose.position.z);
                                ballToMove.transform.position = dest;

                                Vector3 targetDir = dest - player.transform.position;
                                float speed = 40.0f;
                                float step = speed * Time.deltaTime;
                                Vector3 newDir = Vector3.RotateTowards(player.transform.forward, targetDir, step, 0.0F);
                                navMeshAgent.transform.rotation = Quaternion.LookRotation(newDir);

                                reachDestination = false;
                                timer = 0;
                            }
                            break;
                    }
                }
            }
        }
        else
        {
            if (setDestination)
            {
                Debug.Log("Destination is setted");
                if (navMeshAgent.pathPending || navMeshAgent.remainingDistance > 0.1f)
                    return;
                player.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.idle);
                //AnimationController.ChangeAnimator(0);
                reachDestination = true;
                setDestination = false;
                TotalScore++;
                scoreGUI.GetComponent<Text>().text = ("Current Fetch Score: " + TotalScore + "/" + MaxNumberFetch);

            }
            else
            {
                Debug.Log("Destination to be setted");

                Vector3 from = navMeshAgent.transform.position;
                Vector3 to = ballToMove.transform.position;
                float value = 0.80f;

                // Here result = (4, 5, 6)
                Vector3 result = Vector3.Lerp(from, to, value);
                navMeshAgent.SetDestination(result);
                setDestination = true;
                //AnimationController.ChangeAnimator(1);

                player.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.jump);

            }
        }


    }


    //Stop the emission after GameMaxFetch
    private IEnumerator EndGame()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        scoreGUI.GetComponent<CanvasGroup>().alpha = 0;
        if (OnBallGameFinished != null)
        {
            Debug.Log("Event OnBubbleGameFinished called");
            OnBallGameFinished(TotalScore);
        }
        else Debug.Log("No listener subscribed to OnBubbleGameFinished");

        //scoreGUI.GetComponent<CanvasGroup>().alpha = 0.0f;
        isBallGameActive = false;
        //TODO: Aggiungere schermata hai vinto/hai perso

    }

}