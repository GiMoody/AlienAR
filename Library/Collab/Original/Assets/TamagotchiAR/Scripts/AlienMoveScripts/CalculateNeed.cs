using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class CalculateNeed : MonoBehaviour {
    public static float timeForFinishRequest = 0;
    float timer = 0;

    // Pesi assegnati ad ogni bisogno
    static int[] needWeight = { 2, 3, 2, 5, 6, 3 };

    public static bool isImageEvolutionPressed = false;

    public static bool isNeedEnd = false;
    public static bool isGameEnd = false;
    public static bool isReceivedMedicine = false;
    public static bool isScreenCleaned = false;
    public delegate void ClearBowl(int needSatisfied);
    public static event ClearBowl OnClearBowl;

    public delegate void ClearWaterBowl(int needSatisfied);
    public static event ClearWaterBowl OnClearWaterBowl;

    [Task]
    public static bool IsBowlFull = false;

    public static bool IsFoodBowlFull = false;
    public static bool IsDrinkBowlFull = false;

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

    [Task]
    bool GameStarted = false;

    [Task]
    void FinallyEndGame()
    {
        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
        alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.die);
        if (AlienNeeds.instance.getLvl() < 3)
        {
            //Simpledead GAME OVER
            GameManager.instance.gameOver();
        }
        else
        {
            // Se la felicità è minore dell'80% BAD ENDING
            if (AlienNeeds.instance.getHappyness() < 20 * 0.8f) //AlienNeeds.MAX_HAPPY
            {
                GameManager.instance.startLastChoise();
            }
            else
            {
                //HAPPY ENDING (morte normale)
                GameManager.instance.goodEnding();
            }

        }
        IsFinallyGameEnd = true;
        Task.current.Succeed();
    }


    GameObject nextEvolution, oldEvolution;
    float deltaTime, maxEvolutionTime = 4;
    float evoTimer, changeFrequencyTimer;
    GameObject particleSystem;

    [Task]
    void AdvisePlayerEvo()
    {
        GameManager.instance.setEvolutionMessage();
        Task.current.Succeed();
    }


    [Task]
    void WowAlienIsEvolving()
    {
        if (Task.current.isStarting)
        {

            particleSystem = GameObject.Find("EvolutionEffect");
            //Code here
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

        if (isImageEvolutionPressed)
        {
            evoTimer += Time.deltaTime;
            changeFrequencyTimer += Time.deltaTime;
            Debug.Log(deltaTime);
            if (evoTimer >= maxEvolutionTime)
            {
                oldEvolution.SetActive(false);
                nextEvolution.SetActive(true);
                particleSystem.GetComponent<EvolutionEffectScript>().StartAnimation(transform.position);
                AlienNeeds.instance.SetEvolving(false);
                isImageEvolutionPressed = false;
                Task.current.Succeed();
            }


            if (changeFrequencyTimer >= deltaTime)
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
        else transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.position);
    }


    [Task]
    void CalculateAction()
    {
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
            if (GameManager.instance.needFinalValue == 4)
            {
                float currentLvl = AlienNeeds.instance.getLvl();
                int gameChoose = (int)Random.Range(1, currentLvl + 2);
                if (gameChoose >= (int)currentLvl)
                    gameChoose = (int)currentLvl;
                GameManager.instance.gameChoose = gameChoose - 1;
                Debug.Log("Valore random preso tra 1 e 3:" + gameChoose);
                Debug.Log("GAME BUBBLE " + (int)GameManager.GameSelection.Bubble);
                Debug.Log("GAME CARD " + (int)GameManager.GameSelection.Card);
                Debug.Log("GAME BALL " + (int)GameManager.GameSelection.Ball);
                Debug.Log("GAME CHOOSE " + GameManager.instance.gameChoose);
                GameNeed = true;
            }
            else
                //GameNeed = (GameManager.instance.needFinalValue == 4) ? true : false;
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
    /// Righe di codice della morte
    /// </summary>
    [Task]
    void EndGame()
    {
        Debug.Log("Is game end?" + isGameEnd);
        if (isGameEnd == true)
            Task.current.Succeed();
    }

    [Task]
    void StartGame()
    {
        // Aggiungere logica per il tipo di gioco (per ora faccio solo uno)
        /*if (GameManager.instance.gameChoose == (int)GameManager.GameSelection.Bubble)
        {
            Debug.Log("START BUBBLE GAME");
            GameManager.instance.StartMiniGame();
        }
        if (GameManager.instance.gameChoose == (int)GameManager.GameSelection.Card)
        {
            Debug.Log("START CARD GAME");
            GameManager.instance.StartMiniGame();
        }
        if (GameManager.instance.gameChoose == (int)GameManager.GameSelection.Ball)
        {
            Debug.Log("START BALL GAME");
            GameManager.instance.StartMiniGame();
        }*/
        GameManager.instance.startGui();
        GameStarted = true;
        Task.current.Succeed();
    }

    [Task]
    void SetScores()
    {

        Debug.Log("Possibile fine gioco");
        int scoreResult = GameManager.instance.lastGameScoreResult;
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
        isGameEnd = false;
        GameStarted = false;
        need = false;
        GameNeed = false;
        AlienNeeds.instance.SetNeedToZero();
        Task.current.Succeed();
    }

    /**
   * Controlla se l'azione è giunta al termine o no, in questa versione si aspetta un tot prima di segnarla come completata.
   * A seconda del tempo l'alieno reagirà in modo diverso ottenendo esperienza e felicità in caso positivo
   */
    bool soundStart = false;
    [Task]
    void IsNeedEnd()
    {
        // Calcolo il tempo di risposta all'avvio del task
        if (Task.current.isStarting && timeForFinishRequest == 0)
            timeForFinishRequest = Random.Range(0, 1);

        switch (GameManager.instance.needFinalValue)
        {
            case 1: // L'alieno ha fame
                {
                    if (timer > maxWaitingTime)
                    {
                        AlienNeeds.instance.IncrementHappyness(-5);
                        AlienNeeds.instance.IncrementLife(-5);

                        if (ShowEmotionScript.isAngry == false)
                            ShowEmotionScript.isAngry = true;

                        waitTimeFood += timer;
                        need = false;
                        IsNeedSatisfied = true;
                        timeForFinishRequest = 0;
                        timer = 0;
                        AlienNeeds.instance.ReduceNeed();
                        Task.current.Succeed();
                    }
                    
                    if (BowlCollisionBehaviour.isFull)
                    {
                        
                        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");

                        alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.eating);
                        alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.eating);
                        
                        // Start suono : alieno mangia

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
                        timeForFinishRequest = 0;
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
                    if (timer > maxWaitingTime)
                    {
                        AlienNeeds.instance.IncrementHappyness(-6);
                        AlienNeeds.instance.IncrementLife(-5);

                        if (ShowEmotionScript.isAngry == false)
                            ShowEmotionScript.isAngry = true;

                        waitTimeDrink += timer;
                        need = false;
                        IsNeedSatisfied = true;
                        timeForFinishRequest = 0;
                        timer = 0;
                        AlienNeeds.instance.ReduceNeed();
                        Task.current.Succeed();
                    }

                    if (DrinkBowlCollisionBehaviour.isFull)
                    {

                        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");

                        alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.eating);
                        alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.eating);

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
                        timeForFinishRequest = 0;
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
                    GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
                    alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.sleeping);
                    if (!soundStart) {
                        // Start suono : alieno dorme

                        soundStart = true;
                    }

                    timer += Time.deltaTime;
                    if (timer > maxWaitingTime)
                    {
                        // End suono : alieno dorme


                        AlienNeeds.instance.IncrementHappyness(1);
                        AlienNeeds.instance.IncrementExp(1);
                        AlienNeeds.instance.IncrementLife(1);

                        soundStart = false;
                        need = false;
                        IsNeedSatisfied = true;
                        timeForFinishRequest = 0;
                        timer = 0;
                        AlienNeeds.instance.SetNeedToZero();
                        Task.current.Succeed();
                    }
                }
                break;
            case 5: // L'alieno è malato
                if (GameManager.instance.needFinalValue == 5)
                {
                    noHealthCycle++;

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
                        Task.current.Succeed();
                    }

                    if (AlienNeeds.instance.IsIll() && isReceivedMedicine)
                    {
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
                        timeForFinishRequest = 0;
                        timer = 0;
                        isReceivedMedicine = false;
                        waitTimeHealth = 0;
                        noHealthCycle = 0;
                        // }
                        Task.current.Succeed();
                    }
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
                        Task.current.Succeed();
                    }

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
                        timeForFinishRequest = 0;
                        timer = 0;
                        isScreenCleaned = false;
                        waitTimeClean = 0;
                        noCleanCycle = 0;
                        // }
                        Task.current.Succeed();
                    }
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

    [Task]
    void CleanBowl()
    {
        if (GameManager.instance.needFinalValue == 1)
        {
            if (OnClearBowl != null)
                OnClearBowl(GameManager.instance.needFinalValue);
        }
        else if (GameManager.instance.needFinalValue == 2)
        {
            if (OnClearBowl != null)
                OnClearBowl(GameManager.instance.needFinalValue);
        }
        IsBowlFull = false;

        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
        alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.standard);
        alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.idle);

        // End suono : alieno mangia
        Task.current.Succeed();
    }

}
