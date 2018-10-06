using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleGameController : MonoBehaviour
{
    /// <summary>
    /// Delay between each generated Bubble in second
    /// </summary>
    public float BubbleSpawnDelay;
    /// <summary>
    /// Total duration of the mini-game in seconds
    /// </summary>
    public float GameDuration;
    /// <summary>
    /// Initial velocity of the bubble
    /// </summary>
    public float BubbleSpeed;

    //Object from wich the bubbles are generated
    [HideInInspector] public GameObject BubbleGenerator;

    public GameObject BubblePrefab;
    public GameObject scoreGUI;

    /// <summary>
    /// The first-person camera being used to render the passthrough camera.
    /// </summary>
    public Camera FirstPersonCamera;

    private static int TotalScore;
    /// <summary>
    /// Coroutine that manage the bubble emission
    /// </summary>
    private Coroutine _EmitBubbleRoutine;
    /// <summary>
    /// Coroutine that stop the game after a fixed time
    /// </summary>
    private Coroutine _stopEmission;

    public delegate void BubbleGameComplete(int score);
    public static event BubbleGameComplete OnBubbleGameFinished;

    private bool isGameActive = false;

    public static BubbleGameController instance = null;

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
    /// Metodo che fa partire il minigioco, settando delle variabili per il BT
    /// </summary>
    /// <returns></returns>
    public int StartBubbleGame()
    {
        if (isGameActive) return -1;
        isGameActive = true;
        TotalScore = 0;
        _EmitBubbleRoutine = StartCoroutine(EmitBubbleRoutine());
        _stopEmission = StartCoroutine(StopEmission());
        scoreGUI.GetComponent<Text>().text = ("Current Score: " + TotalScore);
        scoreGUI.GetComponent<CanvasGroup>().alpha = 1.0f;
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        if ((GameManager.instance.CurrentGameStatus != (int)GameManager.GameStatus.Game)&&!isGameActive) return;

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        RaycastHit hit;        
        Ray ray = FirstPersonCamera.ScreenPointToRay(touch.position);

        if (Physics.Raycast(ray, out hit))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();

            if (interactable != null)
                interactable.Interact(gameObject);
        }
    }
    
    public void AddScore(int scoreValue)
    {
        if (scoreValue >= 0)
        {
            TotalScore += scoreValue;
            scoreGUI.GetComponent<Text>().text = ("Current Score: " + TotalScore);
        }
    }

    public void SubScore(int scoreValue)
    {
        if ((TotalScore - scoreValue) >= 0)
        {
            TotalScore -= scoreValue;
            scoreGUI.GetComponent<Text>().text = ("Current Score: " + TotalScore);
        }

    }

    /// <summary>
    /// Calcola il punteggio del mingioco a seconda di quante bolle sono state scoppiate rispetto al totale
    /// </summary>
    /// <returns></returns>
    private int ComputeTotalScore() {
        int bubbleSpawned = (int) (GameDuration/BubbleSpawnDelay);
        float touchedBubbleRatio = TotalScore / bubbleSpawned;
        if (touchedBubbleRatio <= 0.3) return 0;
        else if (touchedBubbleRatio > 0.3 && touchedBubbleRatio <= 0.6) return 1;
        else return 2;
    }

    /// <summary>
    /// Stop the emission after GameDuration s
    /// </summary>
    /// <returns></returns>
    private IEnumerator StopEmission()
    {
        Debug.Log("Starting Coroutine generation");
        yield return new WaitForSeconds(GameDuration);


        Debug.Log("Stopping Coroutine generation");
        StopCoroutine(_EmitBubbleRoutine);
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Bubble") == null);

        int finalScore = ComputeTotalScore();
        if(finalScore == 2)
            scoreGUI.GetComponent<Text>().text = ("Ottimo Lavoro! TotalScore: " + TotalScore);
        if (finalScore == 1)
            scoreGUI.GetComponent<Text>().text = ("Non male, TotalScore:" + TotalScore);
        if (finalScore == 0)
            scoreGUI.GetComponent<Text>().text = ("Ops... TotalScore:" + TotalScore);


        yield return new WaitForSecondsRealtime(3.0f);


        if (OnBubbleGameFinished != null)
        {
            Debug.Log("Event OnBubbleGameFinished called");
            OnBubbleGameFinished(ComputeTotalScore());
        }
        else Debug.Log("No listener subscribed to OnBubbleGameFinished");

        scoreGUI.GetComponent<CanvasGroup>().alpha = 0.0f;
        isGameActive = false;
        //TODO: Aggiungere schermata hai vinto/hai perso

    }

    /// <summary>
    /// Instantiate a new Bubble every BubbleSpawnDelay s
    /// </summary>
    /// <returns></returns>
    private IEnumerator EmitBubbleRoutine()
    {
        float offset = 0.25f;
        GameObject generator = GameObject.FindGameObjectWithTag("BubbleGenerator");
        while (true)
        {
            GameObject curBubble;
            // curBubble = Instantiate(BubblePrefab, BubbleGenerator.transform.position, Quaternion.identity);
            curBubble = Instantiate(BubblePrefab, (generator.transform.position+new Vector3(0, offset, 0)), Quaternion.identity);
            Debug.Log(generator.transform.position + new Vector3(0, offset, 0));
            curBubble.GetComponent<BubbleBehaviour>().Float();
           // curBubble.GetComponent<Rigidbody>().velocity = BubbleInitialVelocity * transform.localScale.x * curBubble.transform.up;
            yield return new WaitForSeconds(BubbleSpawnDelay);
        }
    }

}
