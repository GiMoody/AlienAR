using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedBehaviour : MonoBehaviour {

    public static NeedBehaviour instance = null;
    public Camera FirstPersonCamera;
    GameObject current;
    public GameObject scoreGUI;
    //public static bool onCube;
    //public static GameObject colliderObject;
    //public static bool changeColor;

    //public Image image;



    public delegate void FullBowlComplete(GameObject food);
    public static event FullBowlComplete OnFullBowlComplete;

    //public delegate void FullDrinkBowlComplete(GameObject drink);
    //public static event FullDrinkBowlComplete OnFullDrinkBowlComplete;

    /// <summary>
    /// The Unity Awake() method
    /// Implementing the singleton pattern for the GameManager
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

    // Use this for initialization
    void Start()
    {
        //onCube = false;
        //colliderObject = null;
        //changeColor = false;
    }

    // Update is called once per frame
    void Update()
    {
       /* if (AlienNeeds.instance.IsScreenDirty())
        {
            image.enabled = true;
            var tempColor = image.color;
            tempColor.a = 0.5f;
            image.color = tempColor;
        }*/
        if (current != null)
        {
            if (current.tag == "Food")
            {
                scoreGUI.GetComponent<Text>().text = ("Before FoodRoutine");
                FoodRoutine();
            }
            /*else if (current.tag == "Drink")
            {
                DrinkRoutine();
            }
            else if (current.tag == "Medicine")
            {
                MedicineRoutine();
            }*/

        }
    }

    public void CreateCarrot()
    {
        if (current == null)
        {

            GameObject resource = Resources.Load("Food/Carota") as GameObject;

            current = Instantiate(resource as GameObject);
            /*Renderer curR = current.GetComponent<Renderer>();
            Color curC = curR.material.color;
            curC.a = 0.5f;
            curR.material.color = curC;*/
            scoreGUI.GetComponent<Text>().text = ("Carrot Created");

        }

    }

    public void CreateCerry()
    {
        if (current == null)
        {

            GameObject resource = Resources.Load("Ciliegia") as GameObject;

            current = Instantiate(resource as GameObject);
            /*Renderer curR = current.GetComponent<Renderer>();
            Color curC = curR.material.color;
            curC.a = 0.5f;
            curR.material.color = curC;*/

        }
    }

    public void CreateGlassWater()
    {
        if (current == null)
        {

            GameObject resource = Resources.Load("Acqua") as GameObject;

            current = Instantiate(resource as GameObject);
            /*Renderer curR = current.GetComponent<Renderer>();
            Color curC = curR.material.color;
            curC.a = 0.5f;
            curR.material.color = curC;*/

        }
    }

    public void CreateBandAid()
    {
        if (current == null)
        {

            GameObject resource = Resources.Load("Cerotto") as GameObject;

            current = Instantiate(resource as GameObject);
            /*Renderer curR = current.GetComponent<Renderer>();
            Color curC = curR.material.color;
            curC.a = 0.5f;
            curR.material.color = curC;*/

        }
    }

    public void CreatePill()
    {
        if (current == null)
        {

            GameObject resource = Resources.Load("Pillola") as GameObject;

            current = Instantiate(resource as GameObject);
            /*Renderer curR = current.GetComponent<Renderer>();
            Color curC = curR.material.color;
            curC.a = 0.5f;
            curR.material.color = curC;*/

        }
    }
    /*public void CleanScreen()
    {
        if (AlienNeeds.instance.IsScreenDirty())
        {
            image.enabled = false;
        }
    }*/

    void FoodRoutine()
    {
        scoreGUI.GetComponent<Text>().text = ("on FoodRoutine");

        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        Touch touch = Input.GetTouch(0);
        if (Input.touchCount < 1 && (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved))
        {
            scoreGUI.GetComponent<Text>().text = ("On touch stationary/moved");

            //RaycastHit hit;
            //Ray ray = FirstPersonCamera.ScreenPointToRay(touch.position);//mousePosition);

            //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            //{
            float distance = 1;
                Vector3 touchPosition = touch.position;
                touchPosition.z = distance;
                Vector3 touchLocation = FirstPersonCamera.ScreenToWorldPoint(touchPosition);
                current.transform.position = touchLocation;
                //current.transform.LookAt(Camera.main.transform);
                

            //}
           
        }
        /*if (Input.touchCount < 1 && touch.phase == TouchPhase.Moved)
        {
            
            //RaycastHit hit;
            //Ray ray = FirstPersonCamera.ScreenPointToRay(touch.position);

            //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            //{
                float distance = 1;
                Vector3 touchPosition = touch.position;
                touchPosition.z = distance;
                Vector3 touchLocation = FirstPersonCamera.ScreenToWorldPoint(touchPosition);
                current.transform.position = touchLocation;

            //}
           
        }*/
        if (touch.phase == TouchPhase.Ended)
        {
            scoreGUI.GetComponent<Text>().text = ("On touch end");

            RaycastHit hit;
             Ray ray = FirstPersonCamera.ScreenPointToRay(touch.position);

             if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
             {                            
                     //Transform objectHit = hit.transform;
                    // Vector3 dest = new Vector3(hit.point.x, current.transform.position.y, hit.point.z);
                    // current.transform.position = hit.point;//dest;
                                                            // Do something with the object that was hit by the raycast.
                if (hit.collider.gameObject.name == "ciotolacibo(Clone)")
                {
                    scoreGUI.GetComponent<Text>().text = ("Sono qui! Addio adesso :(");
                    /*if (OnFullBowlComplete != null)
                    {
                        //CalculateNeed.isNeedEnd = true;
                        //OnFullBowlComplete(current);//oodCollision(current);
                        DestroyObject(current);
                        current = null;
                        //onCube = false;
                        //colliderObject = null;
                    }
                    else
                    {
                        DestroyObject(current);
                        current = null;
                    }*/
                    DestroyObject(current);
                    current = null;
                }
                else {
                    DestroyObject(current);
                    current = null;
                }
             }

          
        }


      /*  if (Input.GetMouseButtonDown(0))
        {
            // if (onCube == true && colliderObject != null)
            //{
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//GetTouch(0).position);//mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
               
                if (hit.collider.gameObject.name == "ciotolacibo(Clone)")
                {

                    Debug.Log("Sono qui! Addio adesso :(");
                    if (OnFullBowlComplete != null)
                    {
                        //CalculateNeed.isNeedEnd = true;
                        OnFullBowlComplete(current);//oodCollision(current);
                        DestroyObject(current);
                        current = null;
                        //onCube = false;
                        //colliderObject = null;
                    }

                }
                else
                {
                    DestroyObject(current);
                    current = null;
                }
            }

        }*/
        /*else {
            DestroyObject(current);
            current = null;
        }*/
        //}
        //else
        //{
            //float distance = (current.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).magnitude;
            /*float distance = 10;
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition).magnitude);
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.y += 5;
            mousePosition.z = distance;
            Debug.DrawLine(Input.mousePosition, mousePosition);
            //current.transform.position = mousePosition;
            Vector3 mouselocation = Camera.main.ScreenToWorldPoint(mousePosition);
            current.transform.position = mouselocation;
            current.transform.LookAt(Camera.main.transform);*/
            /*int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//GetTouch(0).position);//mousePosition);
            //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            //{
            //Transform objectHit = hit.transform;
            Vector3 dest = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);//(hit.point.x, hit.point.y, 10f);

                current.transform.position = dest;
                */
            //}
        //}
    }

    void DrinkRoutine()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // if (onCube == true && colliderObject != null)
            //{
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//GetTouch(0).position);//mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.DrawRay(ray.origin, hit.point);
                //if (onCube == true && colliderObject != null)
                if (hit.collider.gameObject.name == "ciotola_acqua_piena_2(Clone)")
                {
                    Debug.Log("on object");

                    //Transform objectHit = hit.transform;
                    //Vector3 dest = new Vector3(hit.point.x, hit.point.x, hit.point.z);
                    //current.transform.position = dest;
                    //current = null;
                    //changeColor = true;

                    Debug.Log("Sono qui! Addio adesso :(");
                    if (OnFullBowlComplete != null)
                    {
                        //CalculateNeed.isNeedEnd = true;
                        //OnFullDrinkBowlComplete(current);//oodCollision(current);
                        DestroyObject(current);
                        current = null;
                        //onCube = false;
                        //colliderObject = null;
                    }

                }
                else
                {
                    DestroyObject(current);
                    current = null;
                }
            }

        }
        /*else {
            DestroyObject(current);
            current = null;
        }*/
        //}
        else
        {
            //float distance = (current.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).magnitude;
            float distance = 10;
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition).magnitude);
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.y += 5;
            mousePosition.z = distance;
            Debug.DrawLine(Input.mousePosition, mousePosition);
            //current.transform.position = mousePosition;*/
            Vector3 mouselocation = Camera.main.ScreenToWorldPoint(mousePosition);
            current.transform.position = mouselocation;
            current.transform.LookAt(Camera.main.transform);
            /*int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//GetTouch(0).position);//mousePosition);
            //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            //{
            //Transform objectHit = hit.transform;
            Vector3 dest = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);//(hit.point.x, hit.point.y, 10f);

                current.transform.position = dest;
                */
            //}
        }
    }


    void MedicineRoutine()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // if (onCube == true && colliderObject != null)
            //{
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//GetTouch(0).position);//mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.DrawRay(ray.origin, hit.point);
                //if (onCube == true && colliderObject != null)
                /*if (hit.collider.gameObject.name == "Player(Clone)" && CalculateNeed.isReceivedMedicine == false)
                {
                    Debug.Log("Ho preso la medicina");

                    //Transform objectHit = hit.transform;
                    //Vector3 dest = new Vector3(hit.point.x, hit.point.x, hit.point.z);
                    //current.transform.position = dest;
                    //current = null;
                    // changeColor = true;

                    Debug.Log("Sono qui! Addio adesso :(");
                    //if (OnFullBowlComplete != null)
                    //{
                    //CalculateNeed.isNeedEnd = true;
                    //OnFullDrinkBowlComplete(current);//oodCollision(current);

                    CalculateNeed.isReceivedMedicine = true;
                    DestroyObject(current);
                    current = null;
                    //onCube = false;
                    //colliderObject = null;
                    //}

                }
                else
                {
                    DestroyObject(current);
                    current = null;
                }*/
            }

        }
        /*else {
            DestroyObject(current);
            current = null;
        }*/
        //}
        else
        {
            //float distance = (current.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).magnitude;
            float distance = 10;
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition).magnitude);
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.y += 5;
            mousePosition.z = distance;
            Debug.DrawLine(Input.mousePosition, mousePosition);
            //current.transform.position = mousePosition;*/
            Vector3 mouselocation = Camera.main.ScreenToWorldPoint(mousePosition);
            current.transform.position = mouselocation;
            //current.transform.LookAt(Camera.main.transform);
            /*int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//GetTouch(0).position);//mousePosition);
            //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            //{
            //Transform objectHit = hit.transform;
            Vector3 dest = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);//(hit.point.x, hit.point.y, 10f);

                current.transform.position = dest;
                */
            //}
        }
    }
}
