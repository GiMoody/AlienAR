using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

/// <summary>
/// Classe che contiene i task dell'albero di comportamento.
/// Coltrolla ad ogni update se i bisogni dell'utente sono soddisfatti e calcola l'azione successiva tramite dei pesi assegnati ad ogni bisogno.
/// </summary>
public class CalculateNeed : MonoBehaviour {
    // Timer vari
    public static float timeForFinishRequest = 0;
    float timer = 0;

    /// <summary>
    ///  Tempi massimi prima che si passi al prossimo bisogno
    /// </summary>
    float maxWaitingTime = 10.0f;
    float waitTimeFood = 0;
    float waitTimeDrink = 0;
    float waitTimeHealth = 0;
    float waitTimeClean = 0;
    float noHealthCycle = 0;
    float noCleanCycle = 0;

    // Pesi assegnati ad ogni bisogno
    static int[] needWeight = { 2, 3, 2, 5, 6, 3 };

    // Sorgenti sonore
    public AudioSource mangia;
    public AudioSource dorme;
    public AudioSource evolve;
    public AudioSource finalevolve;

    bool soundStart = false;

    // Interazioni con GUI o altre classi non contenenti i task dell'albero
    public static bool isNeedEnd = false;
    public static bool isGameEnd = false;
    public static bool isReceivedMedicine = false;
    public static bool isScreenCleaned = false;
    public static bool isImageEvolutionPressed = false;
    public static bool isGameOver = false;

    // Eventi per comunicare che le ciotole sono piene o se l'alieno ha finito di interagirci
    public static bool IsFoodBowlFull = false;
    public static bool IsDrinkBowlFull = false;

    // Pulisce la ciotola del cibo
    public delegate void ClearBowl(int needSatisfied);
    public static event ClearBowl OnClearBowl;

    // Pulisce la ciotola dell'acqua
    public delegate void ClearWaterBowl(int needSatisfied);
    public static event ClearWaterBowl OnClearWaterBowl;

    // Variabili usate durante l'evoluzione dell'alieno
    GameObject nextEvolution, oldEvolution;
    float deltaTime, maxEvolutionTime = 5;
    float evoTimer, changeFrequencyTimer;
    GameObject ParticleSystem;

    // Variabile usata per gestire la nuvoletta dei bisogni dell'alieno
    GameObject sprite;

    // Variabili usate all'interno dell'albero, servono per far procedere l'albero o bloccarlo momentaneamente
    [Task]
    public static bool IsBowlFull = false;

    [Task]
    bool IsNeedSatisfied = false;

    [Task]
    public static bool need = false;

    [Task]
    bool GameNeed = false;

    [Task]
    bool IsEvolving { get { return AlienNeeds.instance.IsEvolving(); } }

    [Task]
    bool IsDead { get { return AlienNeeds.instance.IsItDead(); } }

    [Task]
    bool IsFinallyGameEnd = false;

    [Task]
    bool GameStarted = false;

    
    void Start()
    {
        // Prende le sorgenti audio dalla scena
        dorme = GameObject.Find("Audiodorme").GetComponent<AudioSource>();
        mangia = GameObject.Find("Audiomangia").GetComponent<AudioSource>();
        evolve = GameObject.Find("Audioevolve").GetComponent<AudioSource>();
        finalevolve = GameObject.Find("Audiofinalevolve").GetComponent<AudioSource>();
    }

    /// <summary>
    /// Task che viene avviato solo a fine gioco
    /// </summary>
    [Task]
    void FinallyEndGame()
    {
        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");

        // Nel caso l'alieno abbia la vita a 0 prima che abbia raggiunto il terzo stadio, si avrà un GAME OVER
        if (AlienNeeds.instance.getLvl() < 3)
        {
            GameManager.instance.setEndGameResult((int)GameManager.EndGame.GameOver);
            GameManager.instance.setDeathMessage();
            //Start end game routine for no last choise

            // Viene fatta partire l'animazione die e chiamato il comando che gestisce il game over
            //alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.die);
            //GameManager.instance.gameOver();
        }
        // Se l'alieno muore in stadio adulto (terzo stadio)
        else
        {
            // Se la felicità è minore dell'80% viene fatta partire la scena finale
            if (AlienNeeds.instance.getHappyness() < AlienNeeds.MAX_HAPPY * 0.8f) 
            {
                // L'alieno guarda verso la telecamera e viene settato lo stato arrabbiato
                NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
                navMeshAgent.transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.position);
                alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.angry);

                // Comando che gestisce l'ultima scelta del gioco
                GameManager.instance.startLastChoise();
            }
            // Se la felicità è maggiore dell'80% si avrà la fine Happy del gioco
            else
            {
                GameManager.instance.setEndGameResult((int)GameManager.EndGame.GoodEnding);
                GameManager.instance.setDeathMessage();
                //Start end game routine for no last choise
                //alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.die);
                //GameManager.instance.goodEnding();
            }

        }
        // Viene settata la variabile per evitare di far ripartire l'albero di comportamento
        IsFinallyGameEnd = true;
        Task.current.Succeed();
    }


    /// <summary>
    /// Task che fa partire la GUI per avvisare l'utente dell'evoluzione dell'alieno
    /// </summary>
    [Task]
    void AdvisePlayerEvo()
    {
        GameManager.instance.setEvolutionMessage();
        Task.current.Succeed();
    }

    /// <summary>
    /// Task che gestisce l'evoluzione effettiva dell'alieno
    /// </summary>
    [Task]
    void WowAlienIsEvolving()
    {
        // Se il task è stato appena avviato, la funzione prende tutte le varaibili necessarie per eseguire l'operazione
        if (Task.current.isStarting)
        {
            ParticleSystem = GameObject.Find("EvolutionEffect");
            string nameEvolutionStage = "";
            oldEvolution = GameObject.FindGameObjectWithTag("Alien_Baby");
            oldEvolution.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.standard);

            if (AlienNeeds.instance.getLvl() == 2)
            {
                nameEvolutionStage = "Fase_Cucciolo";
            }
            if (AlienNeeds.instance.getLvl() == 3)
            {
                nameEvolutionStage = "Fase_Intermedia";
            }

            foreach (Transform trans in GetComponentsInChildren<Transform>(true))
            {
                Debug.Log(trans.name);
                if (trans.name == nameEvolutionStage)
                {
                    nextEvolution = trans.gameObject;
                }
            }

            deltaTime = 2f;
            evoTimer = 0;
            changeFrequencyTimer = 0;
        }

        // Appena è stata cliccato sopra la GUI dell'evoluzione
        if (isImageEvolutionPressed)
        {
            // Start suono : alieno evoluzione
            if (!soundStart)
            {
                evolve.Play();
                soundStart = true;
            }

            // Gestione dell'animazione dell'alieno tramite codice
            // Viene fatta alternate la mesh dell'alieno tra i due stati evolutivi fino a che non
            // rimane quella del nuovo stadio
            evoTimer += Time.deltaTime;
            changeFrequencyTimer += Time.deltaTime;
            
            // A fine animazione vengono settati tutti i valori per far contibuare l'esecuzione dell'albero
            if (evoTimer >= maxEvolutionTime)
            {
                evolve.Stop();

                AlienNeeds.instance.SetEvolving(false);
                oldEvolution.SetActive(false);
                nextEvolution.SetActive(true);

                finalevolve.Play();

                ParticleSystem.GetComponent<EvolutionEffectScript>().StartAnimation(transform.position);

                isImageEvolutionPressed = false;
                soundStart = false;
                Task.current.Succeed();
            }

            // Funzione che alterna le due mesh ad intervalli di tempo decrescenti
            if (changeFrequencyTimer >= deltaTime)
            {
                if ((maxEvolutionTime - evoTimer) > 0.2f)
                {
                    if (oldEvolution.activeInHierarchy == true)
                        oldEvolution.SetActive(false);
                    else
                        oldEvolution.SetActive(true);

                    if (nextEvolution.activeInHierarchy == true)
                        nextEvolution.SetActive(false);
                    else
                        nextEvolution.SetActive(true);
                    changeFrequencyTimer = 0;
                    if (deltaTime > 0.1)
                    {
                        evoTimer = 0;
                        deltaTime = deltaTime * 0.8f;
                    }
                }
            }

        }
        // Nel caso non ci sia stata ancora interazione con la GUI, l'alieno guarda l'utente
        else transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.position);
    }

    /// <summary>
    /// Calcola la prossima azione da assegnare all'alieno
    /// </summary>
    [Task]
    void CalculateAction()
    {
        // Ferma l'alieno se questo sta saltando
        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
        if (alien.GetComponent<AlienBehaviour>().CurrentAnimation == (int)AlienBehaviour.AnimationStatus.jump)
            alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.idle);

        int[] needs = new int[6];
        needs[0] = NeedFood();
        needs[1] = NeedDrink();
        needs[2] = NeedSleep();
        needs[3] = NeedPlay();
        needs[4] = NeedHealth();
        needs[5] = NeedClean();

        Debug.Log(needs[0] + "-" + needs[1] + "-" + needs[2] + "-" + needs[3] + "-" + needs[4] + "-" + needs[5]);

        // Calcolo il bisogno dell'alieno a questo ciclo (per ora i pesi delle azioni sono statici)
        int currentMax = 0;
        int currentNeed = 0;
        for (int i = 0; i < 6; i++) {
            if (needs[i] >= 1 && needWeight[i] > currentMax) {
                currentMax = needWeight[i];
                currentNeed = i;
            }
        }

        // A seconda del valore del peso definisco se l'alieno ha una richiesta o no
        if (currentMax != 0)
        {
            GameManager.instance.needFinalValue = currentNeed + 1;
            
            // Nel caso del gioco, scelto anche a quale gioco giocare.
            // I vari minigiochi dipendono dallo stato evolutivo dell'alieno
            if (GameManager.instance.needFinalValue == 4)
            {
                float currentLvl = AlienNeeds.instance.getLvl();
                int gameChoose = (int)Random.Range(1, currentLvl + 1);
                if (gameChoose >= (int)currentLvl)
                    gameChoose = (int)currentLvl;
                GameManager.instance.gameChoose = gameChoose - 1;
                GameNeed = true;
            }
            else
                GameNeed = false;

            need = true;
            IsNeedSatisfied = false;
        }
        else
        {
            GameManager.instance.needFinalValue = 0;
            need = false;
        }
        Task.current.Succeed();
    }


    // Metodi per controllare le statistiche dell'alieno
    int NeedFood() { return AlienNeeds.instance.getFood() >= 1 ? 1 : 0; }
    int NeedDrink() { return AlienNeeds.instance.getDrink() >= 1 ? 1 : 0; }
    int NeedHealth() { return AlienNeeds.instance.getHealth() >= 1 ? 1 : 0; }
    int NeedSleep() { return AlienNeeds.instance.getSleep() >= 1 ? 1 : 0; }
    int NeedClean() { return AlienNeeds.instance.getClean() >= 1 ? 1 : 0; }
    int NeedPlay() { return AlienNeeds.instance.getFun() >= 1 ? 1 : 0; }

    /// <summary>
    /// Righe di codice della morte, controlla se il minigioco è finito
    /// </summary>
    [Task]
    void EndGame()
    {
        if (isGameEnd == true)
            Task.current.Succeed();
    }

    /// <summary>
    /// Avvia la GUI per far partire il minigioco
    /// </summary>
    [Task]
    void StartGame()
    {
        GameManager.instance.startGui();
        GameStarted = true;
        Task.current.Succeed();

    }

    /// <summary>
    /// Controlla i punteggi dei minigiochi e da li vengono incrementati o decrementate esperienza e felicità dell'alieno
    /// </summary>
    [Task]
    void SetScores()
    {
        int scoreResult = GameManager.instance.lastGameScoreResult;

        // Nel caso l'utente decida di non giocare, l'alieno si arrabbierà
        if (scoreResult == (int)GameManager.ScoreResult.NotPlay)
        {
            AlienNeeds.instance.IncrementExp(1);
            AlienNeeds.instance.IncrementHappyness(-8);

            if (ShowEmotionScript.isAngry == false)
                ShowEmotionScript.isAngry = true;
        }

        // Giocare con l'alieno incrementa comunque l'esperienza e la felicità
        if (scoreResult == (int)GameManager.ScoreResult.Success)
        {
            AlienNeeds.instance.IncrementExp(8);
            AlienNeeds.instance.IncrementHappyness(10);
        }
        if (scoreResult == (int)GameManager.ScoreResult.Neutral)
        {
            AlienNeeds.instance.IncrementExp(5);
            AlienNeeds.instance.IncrementHappyness(5);
        }
        if (scoreResult == (int)GameManager.ScoreResult.Failed)
        {
            AlienNeeds.instance.IncrementExp(3);
            AlienNeeds.instance.IncrementHappyness(-1);
        }

        // Settaggio di variabili per far continuare l'esecuzione dell'albero
        isGameEnd = false;
        GameStarted = false;
        need = false;
        GameNeed = false;

        AlienNeeds.instance.SetNeedToZero();

        sprite = GameObject.FindGameObjectWithTag("BubbleSpeach");
        sprite.GetComponent<ProvaScript>().LaunchFadeOut();

        Task.current.Succeed();
    }

    /// <summary>
    /// Controlla se l'azione è giunta al termine o no.
    /// In caso positivo verrà valutato il tempo di risposta dell'utente in termini di esperienza,vita e felicità dell'alieno
    /// </summary>
    [Task]
    void IsNeedEnd()
    {
        // Viene presa la nuvoletta dell'alieno
        if (Task.current.isStarting)
            sprite = GameObject.FindGameObjectWithTag("BubbleSpeach");

        // Switch case per gestire tutti i bisogni tranne che il gioco, gestito separatamente
        switch (GameManager.instance.needFinalValue)
        {
            case 1: // L'alieno ha fame
                {
                    timer += Time.deltaTime;

                    // Dopo un determinato lasso di tempo, il bisono verrà ridotto per passare al bisogno successivo
                    // al costo di felicità e vita dell'alieno
                    if (timer > maxWaitingTime)
                    {
                        AlienNeeds.instance.IncrementHappyness(-5);
                        AlienNeeds.instance.IncrementLife(-5);

                        if (ShowEmotionScript.isAngry == false)
                            ShowEmotionScript.isAngry = true;

                        waitTimeFood += timer;
                        need = false;
                        IsNeedSatisfied = true;
                        timer = 0;

                        AlienNeeds.instance.ReduceNeed();
                        sprite.GetComponent<ProvaScript>().LaunchFadeOut();
                        Task.current.Succeed();
                    }
                    // In caso non si risponda per quel lasso di tempo, l'alieno guarderà il giocatore
                    else
                        GetComponent<NavMeshAgent>().transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.position);

                    // Se si riempe la ciotola del cibo
                    if (BowlCollisionBehaviour.isFull)
                    {
                        // Parte animazione, suono e nuvoletta 
                        GetComponent<Move>().LookObjectNeed();
                        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");

                        alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.eating);
                        alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.eating);
                        sprite.GetComponent<ProvaScript>().LaunchFadeOut();
                        
                        // Start suono : alieno mangia
                        mangia.Play();

                        // Vengono settati i parametri dell'alieno
                        waitTimeFood += timer;
                        if (waitTimeFood <= 6)
                        {
                            //full exp/happyness
                            AlienNeeds.instance.IncrementExp(4);
                            AlienNeeds.instance.IncrementHappyness(4);
                            AlienNeeds.instance.IncrementLife(8);

                        }
                        else if (waitTimeFood > 6 && waitTimeFood < 8)
                        {
                            //less exp and happyness
                            AlienNeeds.instance.IncrementExp(2);
                            AlienNeeds.instance.IncrementHappyness(1);
                            AlienNeeds.instance.IncrementLife(8);

                        }
                        else
                        {
                            // no exp and it become angry and decrement happyness and life
                            AlienNeeds.instance.IncrementHappyness(-3);
                            AlienNeeds.instance.IncrementLife(8);

                        }

                        // Finito il task si resettano tutti i valori
                        need = false;
                        IsNeedSatisfied = true;

                        AlienNeeds.instance.SetNeedToZero();

                        timer = 0;
                        IsBowlFull = true;
                        waitTimeFood = 0;

                        Task.current.Succeed();
                    }
                }
                break;
            case 2: // L'alieno ha sete
                if (GameManager.instance.needFinalValue == 2)
                {
                    timer += Time.deltaTime;
                    
                    // Dopo un determinato lasso di tempo, il bisono verrà ridotto per passare al bisogno successivo
                    // al costo di felicità e vita dell'alieno
                    if (timer > maxWaitingTime)
                    {
                        AlienNeeds.instance.IncrementHappyness(-6);
                        AlienNeeds.instance.IncrementLife(-5);

                        if (ShowEmotionScript.isAngry == false)
                            ShowEmotionScript.isAngry = true;

                        waitTimeDrink += timer;
                        need = false;
                        IsNeedSatisfied = true;
                        timer = 0;

                        AlienNeeds.instance.ReduceNeed();
                        sprite.GetComponent<ProvaScript>().LaunchFadeOut();
                        Task.current.Succeed();
                    }
                    // In caso non si risponda per quel lasso di tempo, l'alieno guarderà il giocatore
                    else
                        GetComponent<NavMeshAgent>().transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.position);

                    // Se si riempe la ciotola dell'acqua
                    if (DrinkBowlCollisionBehaviour.isFull)
                    {
                        // Parte animazione e nuvoletta 
                        GetComponent<Move>().LookObjectNeed();

                        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");

                        alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.eating);
                        alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.eating);
                        sprite.GetComponent<ProvaScript>().LaunchFadeOut();

                        // Vengono settati i parametri dell'alieno
                        waitTimeDrink += timer;
                        if (waitTimeDrink <= 6)
                        {
                            //full exp/happyness
                            AlienNeeds.instance.IncrementExp(4);
                            AlienNeeds.instance.IncrementHappyness(4);
                            AlienNeeds.instance.IncrementLife(7);
                        }
                        else if (waitTimeDrink > 6 && waitTimeDrink < 8)
                        {
                            //less exp and happyness
                            AlienNeeds.instance.IncrementExp(2);
                            AlienNeeds.instance.IncrementHappyness(1);
                            AlienNeeds.instance.IncrementLife(7);
                        }
                        else
                        {
                            // no exp and it become angry and decrement happyness
                            AlienNeeds.instance.IncrementHappyness(-3);
                            AlienNeeds.instance.IncrementLife(7);
                        }
                        // Finito il task si resettano tutti i valori
                        need = false;
                        IsNeedSatisfied = true;
                        AlienNeeds.instance.SetNeedToZero();
                        timer = 0;
                        IsBowlFull = true;
                        waitTimeDrink = 0;

                        Task.current.Succeed();
                    }
                }
                break;
            case 3: // L'alieno va a dormire
                if (GameManager.instance.needFinalValue == 3)
                {
                    // Unico bisogno dove l'alieno lo risolve autonomamente, andando a dormire
                    GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
                    alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.sleeping);
                    if (!soundStart)
                    {
                        // Start suono : alieno dorme
                        dorme.Play();  //inizio suono dove dorme
                        soundStart = true;
                    }

                    timer += Time.deltaTime;
                    if (timer > maxWaitingTime)
                    {
                        // End suono : alieno dorme
                        
                        dorme.Stop();   //fine suono dove dorme
                        alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.standard);

                        AlienNeeds.instance.IncrementHappyness(1);
                        AlienNeeds.instance.IncrementExp(1);
                        AlienNeeds.instance.IncrementLife(1);

                        soundStart = false;
                        need = false;
                        IsNeedSatisfied = true;
                        timer = 0;
                        AlienNeeds.instance.SetNeedToZero();
                        sprite.GetComponent<ProvaScript>().LaunchFadeOut();
                        Task.current.Succeed();
                    }
                }
                break;
            case 5: // L'alieno è malato
                if (GameManager.instance.needFinalValue == 5)
                {
                    noHealthCycle++;
                    // Dopo un determinato lasso di tempo, il bisono verrà ridotto per passare al bisogno successivo
                    // al costo di felicità e vita dell'alieno
                    if (noHealthCycle > maxWaitingTime / 2)
                    {
                        AlienNeeds.instance.IncrementHappyness(-6);
                        AlienNeeds.instance.IncrementLife(-7);

                        waitTimeHealth += noHealthCycle;
                        need = false;
                        IsNeedSatisfied = true;
                        timeForFinishRequest = 0;
                        timer = 0;
                        noHealthCycle = 0;
                        AlienNeeds.instance.ReduceNeed();
                        sprite.GetComponent<ProvaScript>().LaunchFadeOut();
                        Task.current.Succeed();
                    }
                    
                    //Se il bisogno è soddisfatto, l'alieno viene curato
                    if (AlienNeeds.instance.IsIll() && isReceivedMedicine)
                    {
                        // A seconda di quanti cicli dell'albero è stato ammalato,
                        // questo riacquista vita
                        waitTimeHealth += noHealthCycle;
                        if (waitTimeHealth <= 6)
                        {
                            //full exp/happyness
                            AlienNeeds.instance.IncrementExp(4);
                            AlienNeeds.instance.IncrementHappyness(4);
                            AlienNeeds.instance.IncrementLife(waitTimeHealth);

                        }
                        else if (waitTimeHealth > 6 && waitTimeHealth < 8)
                        {
                            //less exp and happyness
                            AlienNeeds.instance.IncrementExp(2);
                            AlienNeeds.instance.IncrementHappyness(1);
                            AlienNeeds.instance.IncrementLife(waitTimeHealth);
                        }
                        else
                        {
                            // no exp and it become angry and decrement happyness
                            AlienNeeds.instance.IncrementHappyness(-7);
                            AlienNeeds.instance.IncrementLife(waitTimeHealth);
                        }

                        // Finito il task si resettano tutti i valori
                        need = false;
                        IsNeedSatisfied = true;
                        AlienNeeds.instance.SetNeedToZero();
                        timer = 0;
                        isReceivedMedicine = false;
                        waitTimeHealth = 0;
                        noHealthCycle = 0;
                        sprite.GetComponent<ProvaScript>().LaunchFadeOut();

                        Task.current.Succeed();
                    }
                    // In caso invece non sia stato curato, nel prossimo ciclo si muoverà in un'altra direzione
                    else
                    {
                        IsNeedSatisfied = true;

                        Task.current.Succeed();
                    }
                }
                break;
            case 6: // L'alieno è sporco
                if (GameManager.instance.needFinalValue == 6)
                {
                    noCleanCycle++;
                    // Dopo un determinato lasso di tempo, il bisono verrà ridotto per passare al bisogno successivo
                    // al costo di felicità e vita dell'alieno
                    if (noCleanCycle > maxWaitingTime / 2)
                    {
                        AlienNeeds.instance.IncrementHappyness(-1);
                        AlienNeeds.instance.IncrementLife(-1);

                        waitTimeClean += noCleanCycle;
                        need = false;
                        IsNeedSatisfied = true;
                        timeForFinishRequest = 0;
                        timer = 0;
                        noCleanCycle = 0;
                        AlienNeeds.instance.ReduceNeed();
                        sprite.GetComponent<ProvaScript>().LaunchFadeOut();
                        Task.current.Succeed();
                    }

                    //Se il bisogno è soddisfatto, l'alieno viene pulito
                    if (isScreenCleaned)
                    {
                        waitTimeClean += noCleanCycle;
                        if (waitTimeClean <= 6)
                        {
                            //full exp/happyness
                            AlienNeeds.instance.IncrementExp(3);
                            AlienNeeds.instance.IncrementHappyness(3);
                            AlienNeeds.instance.IncrementLife(2);
                        }
                        else if (waitTimeClean > 6 && waitTimeClean < 8)
                        {
                            //less exp and happyness
                            AlienNeeds.instance.IncrementExp(2);
                            AlienNeeds.instance.IncrementHappyness(1);
                            AlienNeeds.instance.IncrementLife(2);
                        }
                        else
                        {
                            // no exp and it become angry and decrement happyness                   
                            AlienNeeds.instance.IncrementHappyness(-3);
                            AlienNeeds.instance.IncrementLife(2);
                        }
                        // Finito il task si resettano tutti i valori
                        need = false;
                        IsNeedSatisfied = true;
                        AlienNeeds.instance.SetNeedToZero();
                        timer = 0;

                        isScreenCleaned = false;
                        waitTimeClean = 0;
                        noCleanCycle = 0;
                        sprite.GetComponent<ProvaScript>().LaunchFadeOut();
                        Task.current.Succeed();
                    }
                    // In caso invece non sia stato pulito, nel prossimo ciclo si muoverà in un'altra direzione
                    else
                    {
                        IsNeedSatisfied = true;

                        Task.current.Succeed();
                    }
                }
                break;
            default:
                break;
        }

    }


    /// <summary>
    /// Pulisce la ciotola con cui l'alino ha interagito
    /// </summary>
    [Task]
    void CleanBowl()
    {
        // Se ha interagito con la ciotola del cibo...
        if (GameManager.instance.needFinalValue == 1)
        {
            if (OnClearBowl != null)
                OnClearBowl(GameManager.instance.needFinalValue);
        }
        // Se ha interagito con la ciotola dell'acqua...
        else if (GameManager.instance.needFinalValue == 2)
        {
            if (OnClearBowl != null)
                OnClearBowl(GameManager.instance.needFinalValue);
        }
        IsBowlFull = false;

        // Setta l'animazione e lo stato di default
        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
        alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.standard);
        alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.idle);

        mangia.Stop();
        Task.current.Succeed();
    }

}
