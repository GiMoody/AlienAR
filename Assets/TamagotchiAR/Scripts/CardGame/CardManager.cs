using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour {

    //Gameobject del mazzo di carte e della UI
    public GameObject[] mazzo = new GameObject[10];
    public GameObject scoreGUI;

    /// <summary>
    /// The first-person camera being used to render the passthrough camera.
    /// </summary>
    public Camera FirstPersonCamera;


    GameObject carta1;
    GameObject carta2;
    int valore1 = 0;
    int valore2 = 0;
    int result = 0;
    public delegate void CardGameComplete(int result);
    public static event CardGameComplete OnCardGameFinished;

    private bool isGameActive = false;

    public static CardManager instance = null;
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
    /// Metodo che fa partire il minigioco
    /// </summary>
    public void StartCardGame()
    {
        //Variabili per il BT
        if (isGameActive) return;
        isGameActive = true;

        GameObject mazzocarte = GameObject.FindGameObjectWithTag("MazzoCarte");              //CERCO DOVE é STATO POSIZIONATO L'ALIENO NELLA SCENA
        scoreGUI.GetComponent<CanvasGroup>().alpha = 1.0f;
        //scoreGUI.GetComponent<Text>().text = ("Ti va una partita a carte?");

        valore1 = (int)Random.Range(0, mazzo.Length);
        carta1 = Instantiate(mazzo[valore1], mazzocarte.transform.position + new Vector3(-0.3f,0.2f,0), Quaternion.identity);             // ISTANZIO LA PRIMA CARTA
        carta1.transform.LookAt(FirstPersonCamera.transform);

        valore2 = (int)Random.Range(0, mazzo.Length);
        carta2 = Instantiate(mazzo[valore2], mazzocarte.transform.position + new Vector3(+0.3f,0.2f,0), Quaternion.identity);             // ISTANZIO LA PRIMA CARTA
        carta2.transform.LookAt(FirstPersonCamera.transform);

     }

    // Update is called once per frame
    void Update () {
        if ((GameManager.instance.CurrentGameStatus != (int)GameManager.GameStatus.Game)) return;
        if (!isGameActive) return;

        if (carta1) carta1.transform.LookAt(FirstPersonCamera.transform);
        if (carta2) carta2.transform.LookAt(FirstPersonCamera.transform);

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;                                                                                     //CONTROLLO SE IL GIOCATORE HA TOCCATO LO SCHERMO
        }
        //Debug.Log("Screen touched!");


        RaycastHit hit;
        Ray ray = FirstPersonCamera.ScreenPointToRay(touch.position);

        if (Physics.Raycast(ray, out hit))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();

            if (interactable != null)
            {
                Debug.Log("Ho interagito con qualcosa!");

                Animator anim1 = carta1.GetComponent<Animator>();
                anim1.SetInteger("Count", 1);

                Animator anim2 = carta2.GetComponent<Animator>();                            //QUANDO IL GIOCATORE TOCCA LO SCHERMO ANIMO LE CARTE E LE GIRO
                anim2.SetInteger("Count", 1);

                if (hit.collider == carta1.GetComponent<Collider>())
                {
                    Debug.Log("Hai scelto la prima carta!");

                    if (valore2 < valore1)       //SE IL GIOCATORE HA SCELTO LA PRIMA CARTA CONTROLLO SE IL VALORE DELLA PRIMA CARTA è MAGGIORE DI QUELLO DELLA SECONDA E IN BASE A QUESTO DECIDO SE HA VINTO
                    {

                        scoreGUI.GetComponent<Text>().text = ("HAI VINTO");
                        //testo.text = "    HAI VINTO";
                        result = 2;

                    }
                    if (valore1 == valore2)
                    {
                        scoreGUI.GetComponent<Text>().text = ("HAI PAREGGIATO");
                        //testo.text = "HAI PAREGGIATO";
                        result = 1;

                    }
                    if (valore2 > valore1)
                    {
                        scoreGUI.GetComponent<Text>().text = ("HAI PERSO");
                        //testo.text = "    HAI PERSO";
                        result = 0;

                    }
                    if (OnCardGameFinished != null)
                    {                                                                                //RITORNO IL RISULTATO DEL GIOCO ALL'EVENTO:
                        Debug.Log("Event OnCardGameFinished called");                                //       1   GIOCATORE HA VINTO
                        OnCardGameFinished(result);                                                  //       0   PAREGGIO
                    }                                                                                //      -1   GIOCATORE HA PERSO

                }

                else {                                                                               //SE IL GIOCATORE HA SCELTO LA SECONDA CARTA SCELGO IN BASE AL CONFRONTO CON LA PRIMA SE HA VINTO O PERSO
                    scoreGUI.GetComponent<Text>().text = (valore1.ToString() + " " + valore2.ToString());

                    if (valore2 > valore1)
                    {
                        scoreGUI.GetComponent<Text>().text = ("HAI VINTO");
                        //testo.text = "    HAI VINTO";
                        result = 2;

                    }
                    if (valore1 == valore2) {

                        scoreGUI.GetComponent<Text>().text = ("HAI PAREGGIATO");
                        //testo.text = "HAI PAREGGIATO";
                        result = 1;

                    }
                    if (valore2 < valore1) {

                        scoreGUI.GetComponent<Text>().text = ("HAI PERSO");
                        //testo.text = "    HAI PERSO";
                        result = 0;

                    }


                }

                StartCoroutine(EndGame());

            }
              
        }

    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1.0f);
        scoreGUI.GetComponent<CanvasGroup>().alpha = 0f;

        isGameActive = false;
        if (OnCardGameFinished != null)                                                //RITORNO IL RISULTATO DEL GIOCO ALL'EVENTO
        {                                                                              //        2   GIOCATORE HA VINTO
            Debug.Log("Event OnCardGameFinished called");                              //        1   PAREGGIO
            OnCardGameFinished(result);                                                //        0   GIOCATORE HA PERSO
        }

        yield return new WaitForSecondsRealtime(3.0f);
        if(carta1) DestroyObject(carta1);
        if(carta2) DestroyObject(carta2);


    }
}
