using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;


/// <summary>
/// Classe che gestisce i bottoni di cibo, acqua, medicine e pulizia
/// </summary>
public class NeedBehaviour : MonoBehaviour
{
    // Path per raggiungere i prefab
    private string resourcePath = "Food/";

    public Camera FirstPersonCamera;

    // Variabili per gestire un oggetto alla volta
    private GameObject current;
    private GameObject loadedResource;
    
    // Riferimento a diverse gui o immagini nella canvas
    public GameObject scoreGUI;
    public GameObject helpGUI;
    public GameObject Dirt;

    // Gestione eventi per le ciotole
    public delegate void FullBowlComplete(GameObject food);
    public static event FullBowlComplete OnFullBowlComplete;

    public delegate void FullDrinkBowlComplete(GameObject drink);
    public static event FullDrinkBowlComplete OnFullDrinkBowlComplete;

    /// <summary>
    ///  Singleton della classe
    /// </summary>
    public static NeedBehaviour instance = null;

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
    /// Funzione che "sporca" lo schermo
    /// </summary>
    public void setScreenDirty()
    {
        Dirt.GetComponent<Image>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Se un oggetto è stato caricato dalle risorsem si avviano diverse routine in base al tag del prefab
        if (loadedResource != null)
        {
            if (loadedResource.tag == "Food")
            {
                FoodRoutine();
            }
            else if (loadedResource.tag == "Drink")
            {
                DrinkRoutine();
            }
            else if (loadedResource.tag == "Medicine")
            {
                MedicineRoutine();
            }
        }
    }

    /// <summary>
    /// Funzione che carica il prefab Carota
    /// </summary>
    public void CreateCarrot()
    {
        if (loadedResource == null && current == null)
        {
            loadedResource = Resources.Load(resourcePath + "Carota") as GameObject;

        }
        else
        {
            Debug.Log("A resource is already selected!");
        }

    }

    /// <summary>
    /// Funzione che carica il prefab Ciliegia
    /// </summary>
    public void CreateCerry()
    {
        if (loadedResource == null && current == null)
        {
            loadedResource = Resources.Load(resourcePath + "Ciliegia") as GameObject;
        }
        else
        {
            Debug.Log("A resource is already selected!");
        }
    }

    /// <summary>
    /// Funzione che carica il prefab Acqua
    /// </summary>
    public void CreateGlassWater()
    {
        if (loadedResource == null && current == null)
        {
            loadedResource = Resources.Load(resourcePath + "Acqua") as GameObject;
        }
        else
        {
            Debug.Log("A resource is already selected!");
        }
    }

    /// <summary>
    /// Funzione che carica il prefab Cerotto
    /// </summary>
    public void CreateBandAid()
    {
        if (loadedResource == null && current == null)
        {
            loadedResource = Resources.Load(resourcePath + "Cerotto") as GameObject;
        }
        else
        {
            Debug.Log("A resource is already selected!");
        }
    }

    /// <summary>
    /// Funzione che carica il prefab Pillola
    /// </summary>
    public void CreatePill()
    {
        if (loadedResource == null && current == null)
        {
            loadedResource = Resources.Load(resourcePath + "Pillola") as GameObject;
        }
        else
        {
            Debug.Log("A resource is already selected!");
        }
    }

    /// <summary>
    /// Funzione che pulisce lo schermo, se sporco
    /// Non è attivo se la GUI help è attiva
    /// </summary>
    public void CleanScreen()
    {
        if (!helpGUI.activeInHierarchy)
        {
            Dirt.GetComponent<Image>().enabled = false;
            CalculateNeed.isScreenCleaned = true;
        }
    }


    /// <summary>
    /// Posiziona l'oggetto nella scena e aspetta l'interazione con il giusto oggetto definito dal nome objectname
    /// </summary>
    private void ObjectPositioning(string objectname, int layermask)
    {
        if (loadedResource == null) return;

        if (Input.touchCount > 0 && GameManager.instance.CurrentGameStatus == (int)GameManager.GameStatus.Idle)
        {

            Touch touch = Input.GetTouch(0);
            Vector3 touchPos3D = FirstPersonCamera.ScreenToWorldPoint(touch.position);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (current == null)
                    {
                        TrackableHit rayhit;
                        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
                        if (Session.Raycast(touch.position.x, touch.position.y, raycastFilter, out rayhit))
                        {
                            current = Instantiate(loadedResource, rayhit.Pose.position, rayhit.Pose.rotation);
                            // current.transform.position = rayhit.Pose.position;
                        }


                    }
                    break;

                case TouchPhase.Stationary:
                case TouchPhase.Moved:
                    if (current != null)
                    {
                        TrackableHit rayhit;
                        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
                        if (Session.Raycast(touch.position.x, touch.position.y, raycastFilter, out rayhit))
                        {
                            current.transform.position = rayhit.Pose.position;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    Debug.Log("Touchphase ended");

                    RaycastHit hit;
                    Ray ray = FirstPersonCamera.ScreenPointToRay(touch.position);
                    Debug.Log("Raycast");

                    //Faccio un raycast per vedere se ho colpito la ciotola
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layermask))
                    {
                        if (hit.collider.gameObject.name == objectname)
                        {
                            // Se l'oggetto è la ciotola del cibo, viene fatta partire l'evento che riempie la ciotola
                            if (objectname == "ciotolacibo(Clone)")
                            {
                                if (OnFullBowlComplete != null)
                                {
                                    OnFullBowlComplete(current);
                                    DestroyObject(current);
                                    current = null;
                                    loadedResource = null;
                                }
                            }

                            // Se l'oggetto è la ciotola dell'acqua, viene fatta partire l'evento che riempie la ciotola
                            if (objectname == "ciotola_acqua_piena(Clone)")
                            {
                                if (OnFullDrinkBowlComplete != null)
                                {
                                    OnFullDrinkBowlComplete(current);
                                    DestroyObject(current);
                                    current = null;
                                    loadedResource = null;
                                }
                            }
                            Debug.Log(hit.collider.gameObject.name);

                            // Se l'oggetto è l'alieno e non ha ancora ricevuto une medicina, questo la prende
                            if (objectname == "AlienGO_Final_Test_1(Clone)")//&& CalculateNeed.isReceivedMedicine == false)
                            {
                                CalculateNeed.isReceivedMedicine = true;
                                DestroyObject(current);
                                current = null;
                                loadedResource = null;
                            }
                        }
                    }
                    break;
            }
        }

    }

    /// <summary>
    /// Funzione che fa partire l'interazione dei cibi nella scena
    /// </summary>
    void FoodRoutine()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        ObjectPositioning("ciotolacibo(Clone)", layerMask);

    }

    /// <summary>
    /// Funzione che fa partire l'interazione delle bevande nella scena
    /// </summary>
    void DrinkRoutine()
    {
        int layerMask = 1 << 8;

        layerMask = ~layerMask;
        ObjectPositioning("ciotola_acqua_piena(Clone)", layerMask);

    }

    /// <summary>
    /// Funzione che fa partire l'interazione delle medicine nella scena
    /// </summary>
    void MedicineRoutine()
    {
        int layerMask = 1 << 8;
        ObjectPositioning("AlienGO_Final_Test_1(Clone)", layerMask);
    }
}
