using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;


/// <summary>
/// Classe che controlla in esecuzione i bisogni dell'alieno, incrementandoli ogni tot secondi 
/// </summary>
public class AlienNeeds : MonoBehaviour
{
    // Singleton della classe
    public static AlienNeeds instance = null;

    // Valori statici
    public static float MAX_EXP = 20;//100;
    public static float MAX_LIFE = 50;//100;
    public static float MAX_HAPPY = 40;//100;
    public static float time_saving = 5;

    // Statistiche dell'alieno
    static int level;

    static float exp;
    static float life;
    static float happy;

    static float food;
    static float drink;
    static float health;
    static float sleep;
    static float clean;
    static float fun;

    // Controllo lo stato dell'alieno in termini di cure e bisogni
    static bool isIll;
    static bool isDead = false;
    static bool isEvolving = false;
    
    // Variabile di controllo per controllare se l'alieno è stato pulito o no
    static bool isScreenDirty;

    // Controlla se il gioco è stato avviato o no per la prima volta
    static bool isNewGame = false;

    // Tempi che devono passare per incrementare un bisogno
    // VARIABILI SETTATE PER UNA SESSIONE DI GIOCO MOLTO VELOCE
    float time_increment_need_food = 5f; 
    float time_increment_need_drink = 4f;
    float time_increment_need_sleep = 8f;
    float time_increment_need_clean = 7f;
    float time_increment_ill = 9f;
    float time_increment_play = 3f;

    float timer_need_drink = 0;
    float timer_need_food = 0;
    float timer_need_sleep = 0;
    float timer_need_clean = 0;
    float timer_ill = 0;
    float timer_play = 0;

    float timer_save = 0;

    // Struttura data usata per salvare/caricare i dati
    SaveData playerData;

    // Log usato per il debug
    static string myLog = "";


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

    // Usato all'avvio dell'applicazione
    void Start()
    {
        playerData = new SaveData();

        string saveGameFileName = "saves";
        string filePath = Path.Combine(Application.persistentDataPath, "data");
        filePath = Path.Combine(filePath, saveGameFileName + ".binary");

        // In caso non esista un file di salvataggio si parte con le statistiche base 
         if (!File.Exists(filePath))
        {
            level = 1;
            exp = 0;
            life = MAX_LIFE;
            happy = 50;

            food = 0.5f;
            drink = 0.5f;
            health = 0f;
            sleep = 0.5f;
            fun = 0.5f;
            clean = 0.5f;

            isIll = false;
            isNewGame = true;
        }
        else
        {
            // In caso esista un file di salvataggio si caricano i dati salvati
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream saveFile = File.Open(filePath, FileMode.Open);

            playerData = (SaveData)formatter.Deserialize(saveFile);

            level = playerData.level;
            exp = playerData.exp;
            life = playerData.life;
            happy = playerData.happy;

            food = playerData.food;
            drink = playerData.drink;
            health = playerData.health;
            sleep = playerData.sleep;
            fun = playerData.fun;
            clean = playerData.clean;

            isIll = playerData.isIll;

            isNewGame = false;
            saveFile.Close();
        }

        // Usato per debuggare
        myLog = "Food:" + food + "\n";
        myLog += "Drink:" + drink + "\n";
        myLog += "Health:" + health + "\n";
        myLog += "Sleep:" + sleep + "\n";
        myLog += "Clean:" + clean + "\n";
        myLog += "Fun:" + fun + "\n";
        myLog += "IS ILL:" + isIll + "\n";
        myLog += "STATS\n";
        myLog += "LVL:" + level + "\n";
        myLog += "Happyness:" + happy + "\n";
        myLog += "Life:" + life + "\n";
        myLog += "EXP:" + exp + "\n";
        myLog += "CurrentNeed:" + "none\n";
        myLog += "Time for finish need:" + CalculateNeed.timeForFinishRequest + "\n";
    }

    // Usato solo per debuggare Screen.width   Screen.height
    /*void OnGUI() {
        myLog = GUI.TextArea(new Rect(10, 500, 200, 200), myLog);
    }*/

    void Update()
    {
        // I vari bisogni vengono aggiornati solo in fase IDLE del gioco
        if (GameManager.instance.CurrentGameStatus == (int)GameManager.GameStatus.Idle)
        {
            // Non aggiornare se è morto o si sta evolvendo
            if (isEvolving) return; if (isDead) return;
            
            // Aspetto fino a quando il lasso di tempo è passato per ogni bisogno
            timer_need_food += Time.deltaTime;
            timer_need_drink += Time.deltaTime;
            timer_need_sleep += Time.deltaTime;
            timer_need_clean += Time.deltaTime;
            timer_play += Time.deltaTime;
            timer_ill += Time.deltaTime;
            timer_save += Time.deltaTime;

            //// Start incrementi dei vari bisogni ////

            // Incremento bisogno cibo
            if (timer_need_food >= time_increment_need_food)
            {
                if (food < 1.0f)
                    food += 0.1f;
                timer_need_food = 0;
            }

            // Incremento bisogno sete
            if (timer_need_drink >= time_increment_need_drink)
            {
                if (drink < 1.0f)
                    drink += 0.1f;

                timer_need_drink = 0;
            }

            // Incremento bisogno cure
            if (timer_ill >= time_increment_ill)
            {
                if (health < 1.0f)
                {
                    float ill_increment = UnityEngine.Random.Range(0, 0.2f);
                    health += ill_increment;
                }
                else
                {
                    isIll = true;
                }
                timer_ill = 0;
            }

            // Incremento bisogno dormire
            if (timer_need_sleep >= time_increment_need_sleep)
            {
                if (sleep < 1.0f)
                    sleep += 0.1f;

                timer_need_sleep = 0;
            }

            // Incremento bisogno gioco
            if (timer_play >= time_increment_play)
            {
                if (fun < 1.0f)
                {
                    float play_increment = UnityEngine.Random.Range(0, 0.4f);
                    fun += play_increment;
                }
                timer_play = 0;
            }

            // Incremento bisogno pulizia
            if (timer_need_clean >= time_increment_need_clean)
            {
                if (clean < 1.0f)
                    clean += 0.1f;
                else
                {
                    if (isScreenDirty == false)
                    {
                        NeedBehaviour.instance.setScreenDirty();
                        isScreenDirty = true;
                    }
                }

                timer_need_clean = 0;
            }

            //// Fine incrementi dei vari bisogni ////
            
            // Ogni minuto viene salvato il file
            if (timer_save >= time_saving)
            {
                SaveData();
                timer_save = 0;
            }

            // Usato per debuggare
            myLog = "Food:" + food + "\n";
            myLog += "Drink:" + drink + "\n";
            myLog += "Health:" + health + "\n";
            myLog += "Sleep:" + sleep + "\n";
            myLog += "Clean:" + clean + "\n";
            myLog += "Fun:" + fun + "\n";
            myLog += "IS ILL:" + isIll + "\n";
            myLog += "STATS\n";
            myLog += "LVL:" + level + "\n";
            myLog += "Happyness:" + happy + "\n";
            myLog += "Life:" + life + "\n";
            myLog += "EXP:" + exp + "\n";
            myLog += "CurrentNeed:";
            if (GameManager.instance.needFinalValue == 0)
            {
                myLog += "none";
            }
            if (GameManager.instance.needFinalValue == 1)
            {
                myLog += "hungry!";
            }
            if (GameManager.instance.needFinalValue == 2)
            {
                myLog += "drink!";
            }
            if (GameManager.instance.needFinalValue == 3)
            {
                myLog += "sleepy";
            }
            if (GameManager.instance.needFinalValue == 4)
            {
                myLog += "play";
            }
            if (GameManager.instance.needFinalValue == 5)
            {
                myLog += "don't feel good...";
            }
            if (GameManager.instance.needFinalValue == 6)
            {
                myLog += "dirty!";
            }
            myLog += "\nTime for finish need:" + CalculateNeed.timeForFinishRequest + "\n";
        }
    }
    
    /// <summary>
    /// Funzione richiamata per incrementare l'esperienza dell'alieno e, se raggiunge il valore massimo, fare level up
    /// </summary>
    public void IncrementExp(float exp_inc)
    {
        exp += exp_inc;
        if (exp >= MAX_EXP)
        {
            float rest_exp = exp - MAX_EXP;
            if (level < 3)
            {
                isEvolving = true;
                level++;
                exp = rest_exp;
            }
            else
            {
                isDead = true;
            }
        }
    }
    
    /// <summary>
    /// Funzione richiamata per incrementare/decrementare la felicità dell'alieno
    /// </summary>
    public void IncrementHappyness(float happy_inc)
    {
        happy += happy_inc;
        if (happy >= MAX_HAPPY)
        {
            happy = MAX_HAPPY;
        }
        else if (happy < 0)
        {
            happy = 0;
        }
    }

    /// <summary>
    /// Funzione richiamata per incrementare/decrementare la vita dell'alieno
    /// </summary>
    public void IncrementLife(float life_inc)
    {
        life += life_inc;
        if (life >= MAX_LIFE)
        {
            life = MAX_LIFE;
        }
        else if (happy < 0)
        {
            life = 0;
            isDead = true;
        }
    }

    /// <summary>
    /// A seconda di quale richeista l'alieno voleva, setto tale statistica a zero 
    /// </summary>
    public void SetNeedToZero()
    {
        // Setta a zero il bisogno cibo
        if (GameManager.instance.needFinalValue == 1)
        {
            food = 0;
            //Debug.Log("FOOD:" + food);
        }

        // Setta a zero il bisogno sete
        if (GameManager.instance.needFinalValue == 2)
        {
            drink = 0;
            //Debug.Log("DRINK:" + drink);
        }

        // Setta a zero il bisogno dormire
        if (GameManager.instance.needFinalValue == 3)
        {
            sleep = 0;
            //Debug.Log("SLEEP:" + sleep);
        }

        // Setta a zero il bisogno gioco
        if (GameManager.instance.needFinalValue == 4)
        {
            fun = 0;
            //Debug.Log("FUN:" + fun);
        }

        // Setta a zero il bisogno cure
        if (GameManager.instance.needFinalValue == 5 || (isIll && CalculateNeed.isReceivedMedicine))
        {
            health = 0;
            isIll = false;
            //Debug.Log("HEALT:" + health);
        }

        // Setta a zero il bisogno pulizia
        if (GameManager.instance.needFinalValue == 6 )
        {
            clean = 0;
            isScreenDirty = false;
        }
    }

    /// <summary>
    /// A seconda di quale richeista l'alieno voleva, riduco un poco tale statistica. 
    /// Questo viene fatto per evitare che l'alieno stia fisso su un unico bisogno
    /// </summary>
    public void ReduceNeed()
    {
        // Riduce il bisogno cibo
        if (GameManager.instance.needFinalValue == 1)
        {
            food = 0.8f;
            //Debug.Log("FOOD:" + food);
        }

        // Riduce a zero il bisogno sete
        if (GameManager.instance.needFinalValue == 2)
        {
            drink = 0.8f;
            Debug.Log("DRINK:" + drink);
        }

        // Riduce a zero il bisogno gioco
        if (GameManager.instance.needFinalValue == 4)
        {
            fun = 0.8f;
            Debug.Log("FUN:" + fun);
        }

        // Riduce a zero il bisogno cure
        if (GameManager.instance.needFinalValue == 5)
        {
            health = 0.8f;
            //Debug.Log("HEALT:" + health);
        }

        // Riduce a zero il bisogno pulizia
        if (GameManager.instance.needFinalValue == 6)
        {
            clean = 0.8f;
            Debug.Log("CLEAN:" + fun);
        }
    }
   
    /// Salvataggio dei dati
    void SaveData()
    {
        string saveGameFileName = "saves";
        string filePath = Path.Combine(Application.persistentDataPath, "data");
        filePath = Path.Combine(filePath, saveGameFileName + ".binary");

        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(filePath, FileMode.Create);
            playerData.level = level;
            playerData.exp = exp;
            playerData.life = life;
            playerData.happy = happy;

            playerData.food = food;
            playerData.drink = drink;
            playerData.health = health;
            playerData.sleep = sleep;
            playerData.clean = clean;
            playerData.fun = fun;
            playerData.isIll = isIll;


            bf.Serialize(file, playerData);
            file.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + filePath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
        

    }

    /// Cancellazione dei dati
    public void ClearData()
    {
        string saveGameFileName = "saves";
        string filePath = Path.Combine(Application.persistentDataPath, "data");
        filePath = Path.Combine(filePath, saveGameFileName + ".binary");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    // Getter per le statistiche dell'alieno
    public float getFood() { return food; }
    public float getDrink() { return drink; }
    public float getHealth() { return health; }
    public float getSleep() { return sleep; }
    public float getFun() { return fun; }
    public float getClean() { return clean; }
    public float getHappyness() { return happy; }
    public float getLvl() { return level; }

    // Getter per determinati stati dell'alieno o del gioco
    public bool IsNewGame() { return isNewGame; }
    public bool IsIll() { return isIll; }
    public bool IsScreenDirty() { return isScreenDirty; }
    public bool IsEvolving() { return isEvolving; }
    public bool IsItDead() { return isDead; }

    // Serve per segnalare ad AlienNeeds che l'alieno ha smesso di evolversi
    public void SetEvolving(bool value) { isEvolving = value; }
}
