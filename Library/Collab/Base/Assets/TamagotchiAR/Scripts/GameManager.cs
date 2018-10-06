using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using GoogleARCore.HelloAR;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Static instance of GameManager which allows it to be accessed by any other script.
    /// Singleton pattern
    /// </summary>
    public static GameManager instance = null;

    /// <summary>
    /// Build: enabled gameobject positioning in the ArCore scene
    /// Idle: Stand-by status
    /// Game: Minigame active status
    /// </summary>
    public enum GameStatus { Build, Idle, Game };
    public enum ScoreResult { Failed, Neutral, Success, NotPlay };
    public enum GameSelection { Bubble, Card, Ball };
    public enum EndGame { GameOver, GoodEnding };

    [NonSerialized]
    public int gameChoose;

    //Varie GUI utilizzate nella scena e richimate dal GameManager secondo necessità
    public GameObject buttonGui;
    public GameObject gameGui;
    public GameObject evolutionGui;
    public GameObject deathGui;
    public GameObject helpGui;
    public GameObject gameOverGui;
    public GameObject neutralEndingGui;
    public GameObject badEndingGui;
    public GameObject goodEndingGui;
    public GameObject lastChoiceGui;

    private int _endGameResult;
    /// <summary>
    /// Getter per l'ultimo risultato del gioco
    /// </summary>
    public int EndGameResult { get { return _endGameResult; } }
    public void setEndGameResult(int endGameResult) { _endGameResult = endGameResult; }

    private int _lastGameResult;
    /// <summary>
    /// Getter per l'ultimo risultato del gioco
    /// </summary>
    public int lastGameScoreResult { get { return _lastGameResult; } }
    
    private int _currentGameStatus;
    /// <summary>
    /// Getter dello stato del Game Manager
    /// </summary>
    public int CurrentGameStatus { get { return _currentGameStatus; } }
    [NonSerialized]
    public int needFinalValue = 0; // da 0-6 indicano qual'è il bisogno dell'alieno; 0 non ha richieste 

    [NonSerialized]
    public bool isNavMeshReady;
    
    //Coroutine per lo start dei minigiochi
    private Coroutine _LaunchBubbleGame;
    private Coroutine _LaunchCardGame;
    private Coroutine _LaunchBallGame;

    [NonSerialized]
    public bool pressedGameButton;

    [NonSerialized]
    public bool ClickOnEvolutionMessage;


    [NonSerialized]
    public bool ClickOnDeathMessage;

    [NonSerialized]
    public bool pressedGameEndButton;

    [NonSerialized]
    int valuePressedEndGameButton;

    [NonSerialized]
    public bool pressedScreenEndGame;

    
    /// <summary>
    /// The Unity Awake() method
    /// Implementing the singleton pattern for the GameManager
    /// </summary>
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {

            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this) {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
            }
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// The Unity Start() method
    /// Effettua la sottoscrizione agli eventi e setta alcune variabili di stato per il sistema
    /// </summary>
    void Start()
    {
       // Debug.Log("Start call");
        isNavMeshReady = false;
         _currentGameStatus = (int)GameStatus.Build;

        //Subscribing to events
        AlienPlacerController.OnBuildCompleted += BuildCompletedManager;
        BubbleGameController.OnBubbleGameFinished += BubbleGameFinishedManager;
        CardManager.OnCardGameFinished += CardGameFinishedManager;
        BallGameManager.OnBallGameFinished += BallGameFinishedManager;

        pressedGameButton = false;
        ClickOnEvolutionMessage = false;

        //Carica le gui
        /*buttonGui = GameObject.FindGameObjectWithTag("Bottoni");
        gameGui = GameObject.FindGameObjectWithTag("MiniGameGUI");
        evolutionGui = GameObject.FindGameObjectWithTag("EvolutionGUI");
        deathGui = GameObject.FindGameObjectWithTag("DeathGUI");
        helpGui = GameObject.FindGameObjectWithTag("HelpButton");
        gameOverGui = GameObject.FindGameObjectWithTag("GameOverGUI");
        neutralEndingGui = GameObject.FindGameObjectWithTag("NeutralEndingGUI");
        badEndingGui = GameObject.FindGameObjectWithTag("BadEndingGUI");
        goodEndingGui = GameObject.FindGameObjectWithTag("GoodEndingGUI");
        lastChoiceGui = GameObject.FindGameObjectWithTag("LastChoiceGUI");*/
    }

    /// <summary>
    /// Evento per la gestione della conclusione del gioco delle carte
    /// </summary>
    /// <param name="result">Risultato del gioco</param>
    private void CardGameFinishedManager(int result)
    {
        Debug.Log("Event OnBubbleGameFinished Received");
        _currentGameStatus = (int)GameStatus.Idle;
        CalculateNeed.isGameEnd = true;
        _lastGameResult = result;

    }

    /// <summary>
    /// Evento per la gestione della conclusione del gioco delle bolle
    /// </summary>
    /// <param name="score">Risultato del gioco</param>
    private void BubbleGameFinishedManager(int score)
    {
        Debug.Log("Event OnBubbleGameFinished Received");
        _currentGameStatus = (int)GameStatus.Idle;
        CalculateNeed.isGameEnd = true;
        _lastGameResult = score;

    }

    /// <summary>
    /// Evento per la gestione del gioco della palla
    /// </summary>
    /// <param name="score">Risultato del gioco</param>
    private void BallGameFinishedManager(int score)
    {
        Debug.Log("Event OnBallGameFinished Received");
        _currentGameStatus = (int)GameStatus.Idle;
        CalculateNeed.isGameEnd = true;
        _lastGameResult = score;
    }

    /// <summary>
    /// Evento per la gestione della conclusione della fase di build
    /// Nel caso in cui non trovi i componenti dell'interfaccia chiude l'applicazione
    /// </summary>
    private void BuildCompletedManager()
    {
        Debug.Log("Event OnBuildCompleted Received");
       
        try
        {
            foreach (GameObject ground in GameObject.FindGameObjectsWithTag("Ground")) {
                Debug.Log("Object Ground found");
                ground.GetComponent<MeshRenderer>().enabled = false;
                ground.GetComponent<TrackedPlaneVisualizer>().enabled = false;
            }

            buttonGui.SetActive(true);
            isNavMeshReady = true;
            if (AlienNeeds.instance.IsNewGame())
            {
                Debug.Log("isNewGame!");
                helpGui.GetComponent<DialogueTrigger>().TriggerDialogue();
            }
        }
        catch (NullReferenceException) {
            CloseApplication();
        }
        _currentGameStatus = (int)GameStatus.Idle;
    }


    /// <summary>
    /// Metodo che fa partire i minigiochi e cambia lo stato del GameManager
    /// </summary>
    public void StartMiniGame()
    {
        if (valuePressedGameButton == 0)
        {
            _lastGameResult = (int)ScoreResult.NotPlay;
            CalculateNeed.isGameEnd = true;
            pressedGameButton = false;
            gameGui.SetActive(false);
            return;
        }
        if (pressedGameButton == true)
        {
            pressedGameButton = false;
            gameGui.SetActive(false);
        }
        _currentGameStatus = (int)GameStatus.Game;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Move>().LookObjectNeed();
        GameObject.FindGameObjectWithTag("BubbleSpeach").GetComponent<ProvaScript>().LaunchFadeOut();

        if (gameChoose == (int)GameSelection.Bubble)
            _LaunchBubbleGame = StartCoroutine(LaunchBubbleGame());
        else if (gameChoose == (int)GameSelection.Card)
            _LaunchCardGame = StartCoroutine(LaunchCardGame());
        else if (gameChoose == (int)GameSelection.Ball)
            _LaunchBallGame = StartCoroutine(LaunchBallGame());
    }

    private IEnumerator LaunchCardGame() {
        yield return new WaitForSeconds(3);
        _currentGameStatus = (int)GameStatus.Game;
        CardManager.instance.StartCardGame();       
    }


    private IEnumerator LaunchBubbleGame() {
    
        yield return new WaitForSeconds(3);
        _currentGameStatus = (int)GameStatus.Game;
        BubbleGameController.instance.StartBubbleGame();
    }

    private IEnumerator LaunchBallGame()
    {
        yield return new WaitForSeconds(1);
        _currentGameStatus = (int)GameStatus.Game;
        BallGameManager.instance.StartBallGame();
    }

    // END GAME SOLUTION //

    // Last choise //
    public void clickKill()
    {
        valuePressedEndGameButton = 1;
        pressedGameEndButton = true;
    }

    public void clickNoKill()
    {
        valuePressedEndGameButton = 0;
        pressedGameEndButton = true;
    }

    public void startLastChoise()
    {
        evolutionGui.SetActive(false);
        gameGui.SetActive(false);
        StartCoroutine(WaitForPressedEndButton());
    }
    /// <summary>
    /// Coroutine che aspetta che l'utente prema il bottone che segnala la fine del gioco
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForPressedEndButton()
    {
        //Parte dove si chiede all'utente di uccidere o no l'alieno
        lastChoiceGui.SetActive(true);
        yield return new WaitUntil(() => pressedGameEndButton == true);

        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
        if (valuePressedEndGameButton == 1)
        {
            lastChoiceGui.SetActive(false);
            alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.die);
            neutralEnding();
        }
        else
        {
            lastChoiceGui.SetActive(false);
            alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.jump);            
            NavMeshAgent player = GameObject.FindGameObjectWithTag("Player").GetComponent<NavMeshAgent>();
            Camera firstcamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            Vector3 dest = new Vector3(firstcamera.transform.position.x, player.transform.position.y, firstcamera.transform.position.z);
            player.SetDestination(dest);
            badEnding();
        }
    }


    public void setDeathMessage() {
        StartCoroutine(WaitForInputOnDeathImage());
    }


    IEnumerator WaitForInputOnDeathImage()
    {
        //Parte dove si avvisa il giocatore della morte dell'alieno
        evolutionGui.SetActive(false);
        gameGui.SetActive(false);
        deathGui.SetActive(true);

        yield return new WaitUntil(() => ClickOnDeathMessage == true);
        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
        deathGui.SetActive(false);
        alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.die);

        if (_endGameResult == (int)EndGame.GameOver)
            gameOver();
        if (_endGameResult == (int)EndGame.GoodEnding)
            goodEnding();
    }

    ///  Neutral ending //
    public void neutralEnding()
    {
        buttonGui.SetActive(false);
        evolutionGui.SetActive(false);
        gameGui.SetActive(false);
        neutralEndingGui.SetActive(true);
        GameObject text1 = GameObject.Find("FirstMessage");
        StartCoroutine(FadeIn(3.5f, neutralEndingGui));
    }

    /// Bad ending //
    public void badEnding()
    {
        buttonGui.SetActive(false);
        evolutionGui.SetActive(false);
        gameGui.SetActive(false);
        badEndingGui.SetActive(true);
        GameObject text1 = GameObject.Find("FirstMessage");
        StartCoroutine(FadeIn(3.5f, badEndingGui));
    }

    /// Good ending //
    public void goodEnding()
    {
        buttonGui.SetActive(false);
        evolutionGui.SetActive(false);
        gameGui.SetActive(false);
        goodEndingGui.SetActive(true);
        GameObject text1 = GameObject.Find("FirstMessage");
        StartCoroutine(FadeIn(3.5f, goodEndingGui));
    }


    public void gameOver()
    {
        buttonGui.SetActive(false);
        evolutionGui.SetActive(false);
        gameGui.SetActive(false);
        gameOverGui.SetActive(true);
        GameObject text1 = GameObject.Find("FirstMessage");
        StartCoroutine(FadeIn(3.5f, gameOverGui));
    }

    /// <summary>
    /// General fadein for finalgame messages 
    /// </summary>
    IEnumerator FadeIn(float duration, GameObject go)
    {
        Debug.Log("Coroutine started");
        Image goImage = go.GetComponent<Image>();
        while (goImage.color.a < 1)
        {
            Color newColor = goImage.color;
            newColor.a += Time.deltaTime / duration;
            goImage.color = newColor;
            yield return null;
        }
        if (goImage.color.a >= 1)
        {
            //Start fade in testi
            Text text1 = GameObject.Find("FirstMessage").gameObject.GetComponent<Text>();
            Text text2 = GameObject.Find("SecondMessage").gameObject.GetComponent<Text>();
            while (text1.color.a < 1)
            {
                Color newColor = text1.color;
                newColor.a += Time.deltaTime / duration;
                text1.color = newColor;
                yield return null;
            }
            if (text1.color.a >= 1)
            {
                while (text2.color.a < 1)
                {
                    Color newColor = text2.color;
                    newColor.a += Time.deltaTime / duration;
                    text2.color = newColor;
                    yield return null;
                }
            }
        }
        AlienNeeds.instance.ClearData();

        yield return new WaitUntil(()=>pressedScreenEndGame == true);
        CloseApplication();
    }

    public void CloseApplication() {
        Application.Quit();
    }


    ////////////////////////////////////

    public void setEvolutionMessage()
    {
        StartCoroutine(WaitForInputOnImage());
    }


    IEnumerator WaitForInputOnImage()
    {
        //Parte dove si chiede all'alieno di giocare
        evolutionGui.SetActive(true);
        yield return new WaitUntil(() => ClickOnEvolutionMessage == true);
        startEvolution();
    }

    void startEvolution()
    {
        evolutionGui.SetActive(false);
        CalculateNeed.isImageEvolutionPressed = true;
        ClickOnEvolutionMessage = false;

    }

    public void clickYes()
    {
        valuePressedGameButton = 1;
        pressedGameButton = true;
    }

    public void clickNo()
    {
        valuePressedGameButton = 0;
        pressedGameButton = true;
    }

    public int valuePressedGameButton;

    public void startGui()
    {
        StartCoroutine(WaitForPressedButton());
    }

    IEnumerator WaitForPressedButton()
    {
        //Parte dove si chiede all'alieno di giocare
        gameGui.SetActive(true);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<NavMeshAgent>().transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.position);

        GameObject text = GameObject.Find("MessagePlay");
        if (gameChoose == (int)GameSelection.Bubble)
            text.GetComponent<Text>().text = "Vuoi giocare con le Bolle?\nFai scoppiare tutte le bolle!";

        if (gameChoose == (int)GameSelection.Card)
            text.GetComponent<Text>().text = "Vuoi giocare a carte?\nChi pensca la carta con valore più alto vince!";

        if (gameChoose == (int)GameSelection.Ball)
            text.GetComponent<Text>().text = "Vuoi giocare a palla?\nSposta la pallina e aspetta che l'alieno si avvicini!";
        Debug.Log("prima di WaitForPressedButton GAMEMANAGER");


        Debug.Log("dopo di WaitForPressedButton GAMEMANAGER");
        yield return new WaitUntil(() => pressedGameButton == true);
        StartMiniGame();
    }

}
