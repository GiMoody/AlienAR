using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using GoogleARCore;
using GoogleARCore.HelloAR;


public class AlienPlacerController : MonoBehaviour
{

    public delegate void BuildComplete();
    public static event BuildComplete OnBuildCompleted;
    public static bool buildColliding = false;
    public GameObject scoreGUI;
    /// <summary>
    /// The first-person camera being used to render the passthrough camera.
    /// </summary>
    public Camera FirstPersonCamera;

  //  public GameObject BasePlane;

    private bool planeSet = false;

    /// <summary>
    /// List of all the prefabs we need to put into our scene
    /// </summary>
    public GameObject[] ScenePrefab;
    GameObject current = null;

    /// <summary>
    /// Counter of the already instantiated prefabs
    /// </summary>
    private static int instantiatedPrefab = 0;
    private bool isBuildFinished = false;

    /// <summary>
    /// A prefab for tracking and visualizing detected planes.
    /// </summary>
    public GameObject TrackedPlanePrefab;

    /// <summary>
    /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
    /// </summary>
    public GameObject SearchingForPlaneUI;


    /// <summary>
    /// A list to hold new planes ARCore began tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private List<TrackedPlane> m_NewPlanes = new List<TrackedPlane>();

    /// <summary>
    /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;


    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        _QuitOnConnectionErrors();
        
        // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
        if (Frame.TrackingState != TrackingState.Tracking)
        {
            const int LOST_TRACKING_SLEEP_TIMEOUT = 15;
            Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;
            return;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        Frame.GetPlanes(m_NewPlanes, TrackableQueryFilter.New);

        //Se abbiamo il piano di gioco non mostrare i piani del tracking di ArCore
        if (!planeSet)
        {
            // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
                // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                // coordinates.
                GameObject planeObject = Instantiate(TrackedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                planeObject.GetComponent<TrackedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
                //   Debug.Log("La posizione del piano è: "+planeObject.transform.position);
            }
        }

        // Disable the snackbar UI when no planes are valid.
        bool showSearchingUI = true;

        Frame.GetPlanes(m_AllPlanes);
        for (int i = 0; i < m_AllPlanes.Count; i++)
        {
            if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
            {
                showSearchingUI = false;

                break;
            }
        }

        SearchingForPlaneUI.SetActive(showSearchingUI);

        //If we are not in the Build Game Status, we are done with this update.
        if (GameManager.instance.CurrentGameStatus != (int)GameManager.GameStatus.Build)
        {
            return;
        }


        // If the player has not touched the screen, we are done with this update.
      //  Touch touch;

        if (Input.touchCount < 1) {
            return;
          //  touch = Input.GetTouch(0);
        }
            
        /*    if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }*/

        //Se non abbiamo il piano di appoggio, ne creiamo uno tramite la anchor di ArCore
      /*  if (planeSet == false)
        {
            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

            Touch touch = Input.GetTouch(0);
            //Se il raycast tocca un piano e non abbiamo finito di posizionare gli oggetti...
            if (Session.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && !(isBuildFinished))
            {
                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                //Instantiate our alien as a child of the anchor; it's transform will be relative from the anchor's tracking
                // var curObject = Instantiate(ScenePrefab[instantiatedPrefab], hit.Pose.position, hit.Pose.rotation);
                var curObject = Instantiate(BasePlane, hit.Pose.position, hit.Pose.rotation);

                //Our Alien should look at the camera, but still be flush with the plane
                // alienObject.transform.LookAt(FirstPersonCamera.transform);
                Vector3 dist = new Vector3(0f, 0f, 2.5f);
                curObject.transform.Translate(dist);
                // alienObject.transform.rotation = Quaternion.Euler(-90.0f, alienObject.transform.rotation.y, alienObject.transform.rotation.z);


                // Andy should look at the camera but still be flush with the plane.
                //   curObject.transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f); //NON FA NULLA
                //      curObject.transform.LookAt(FirstPersonCamera.transform); //Nb: viene considerata la posizione della camera appena fa il tracking di un piano
                //  curObject.transform.rotation = Quaternion.Euler(-90.0f,0.0f,180.0f); //SI E' OFFESO E NON MI GUARDA


                // Make the object a child of the anchor.
                curObject.transform.parent = anchor.transform;
                /*
                //Pass to the next prefab
                instantiatedPrefab++;
                //If we have already put into the scene every object, send an event to the GameManager
                if (instantiatedPrefab == ScenePrefab.Length)
                {
                    isBuildFinished = true;
                    if (OnBuildCompleted != null) {
                        Debug.Log("Event OnBuildCompleted called");
                        OnBuildCompleted();
                    }
                    else Debug.LogError("No Listener subscribed to OnBuildCompleted");
                }*/
                /*
                planeSet = true;
                scoreGUI.GetComponent<Text>().text = ("Il ground è stato settato con valore " + planeSet.ToString());
                scoreGUI.GetComponent<CanvasGroup>().alpha = 1.0f;
                return;
            }
        }

        //Posizionamento degli oggetti di scena
        else*/
        //{
            if (!isBuildFinished)
            {
                if (instantiatedPrefab < ScenePrefab.Length)
                {
                    scoreGUI.GetComponent<CanvasGroup>().alpha = 1.0f;
                    scoreGUI.GetComponent<Text>().text = ("Fase build");

                    // CODE FOR TOUCHPAD
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                    {
                        scoreGUI.GetComponent<Text>().text = ("Posizionamento in corso");
                       /* if (current == null)
                        {
                            current = Instantiate(ScenePrefab[instantiatedPrefab] as GameObject);
                            Debug.Log("Current object name:" + current.name);

                        }*/

                        TrackableHit hit;
                        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
                        if (Session.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
                        {
                            if (current == null)
                            {
                                 current = Instantiate(ScenePrefab[instantiatedPrefab] as GameObject, hit.Pose.position,hit.Pose.rotation);
                                 Debug.Log("Current object name:" + current.name);
                            }
                        current.transform.position = hit.Pose.position;

                        }
                    }

                    if (Input.touchCount < 1 && touch.phase == TouchPhase.Moved)
                    {
                        TrackableHit hit;
                        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
                        if (Session.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
                        {
                            current.transform.position = hit.Pose.position;

                        }
                    }
                 /*   else if (current == null)
                    {
                        current = Instantiate(ScenePrefab[instantiatedPrefab] as GameObject);
                        Debug.Log("Current object name:" + current.name);

                    }*/


                    if (touch.phase == TouchPhase.Ended)
                    {

                        TrackableHit hit;
                        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
                        if (Session.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && !(isBuildFinished))
                        {
                            // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                            // world evolves.
                            var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                            Vector3 curPosition = new Vector3(hit.Pose.position.x, current.transform.position.y, hit.Pose.position.z);
                            current.transform.position = curPosition;
                           // current.transform.position = hit.Pose.position;
                            current.transform.rotation = hit.Pose.rotation;
                            current.transform.parent = anchor.transform;

                            if (!planeSet) planeSet = true;

                            if (!buildColliding)
                            {
                                scoreGUI.GetComponent<Text>().text = ("Fine posizionamento per " + current.name);
                                instantiatedPrefab++;
                                current.GetComponent<BoxCollider>().enabled = true;
                                current = null;
                            }
                        }
                    }

                    //Tutti i prefab sono stati posizionati. Il gameController viene avvertito che il gioco può iniziare
                    if (instantiatedPrefab == ScenePrefab.Length)
                    {
                        isBuildFinished = true;
                        if (OnBuildCompleted != null)
                        {
                            Debug.Log("Event OnBuildCompleted called");
                            scoreGUI.GetComponent<Text>().text = ("Build Complete");
                            OnBuildCompleted();
                        }
                        else Debug.LogError("No Listener subscribed to OnBuildCompleted");
                    }
                }

            //}


        }
    }


    /// <summary>
    /// Change the texture of the alien for each screen tap
    /// </summary>
    private void ChangeTexture()
    {
        Debug.Log("Change Called");
        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
        int stato = alien.GetComponent<Alien_Stage1_Behaviour>().CurrentStatus;
        stato = (stato + 1) % 6;
        alien.GetComponent<Alien_Stage1_Behaviour>().CambiaStato(stato);

        //Position debug check
        Vector3 pos = alien.transform.position;
        string msg = "La posizione dell'alieno è: " + pos.ToString();
        Debug.Log(msg);

    }

    /// <summary>
    /// Quit the application if there was a connection error for the ARCore session.
    /// </summary>
    private void _QuitOnConnectionErrors()
    {
        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("DoQuit", 0.5f);
        }
        else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed)
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
