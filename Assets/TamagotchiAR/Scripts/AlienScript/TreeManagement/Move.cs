using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

/// <summary>
/// Classe che gestisce i task dell'albero per quanto riguarda i movimenti dell'alieno
/// </summary>
public class Move : MonoBehaviour {

    // Dati per la NavMesh e la definizione del path
    NavMeshAgent navMeshAgent;
    NavMeshPath path;
    public float m_Range;

    // Nuova posizione
    Vector3 target;

    // Vettori per salvare posizione degli oggetti
    GameObject[] objectsPosition = new GameObject[6];

    /// <summary>
    /// Task che in fase di build permette all'alieno di guarda dove l'utente sta posizionando gli oggetti nella scena
    /// </summary>
    [Task]
    void LookObject()
    {
        if (Input.touchCount > 0)
        {
            RaycastHit hit;
            Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.GetTouch(0).position);//.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

                Vector3 dest = new Vector3(hit.point.x, navMeshAgent.transform.position.y, hit.point.z);
                Vector3 targetDir = dest - navMeshAgent.transform.position;
                float speed = 10.0f;
                float step = speed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(navMeshAgent.transform.forward, targetDir, step, 0.0F);
                navMeshAgent.transform.rotation = Quaternion.LookRotation(newDir);
            }
        }
        Task.current.Succeed();
    }

    /// <summary>
    /// Funzione che permette all'alieno di guardare in direzione del bisogno che vuole soddisfare
    /// </summary>
    public void LookObjectNeed()
    {
        GameObject ob;
        if (GameManager.instance.needFinalValue == 4)
        {
            ob = objectsPosition[GameManager.instance.needFinalValue + GameManager.instance.gameChoose - 1];
        }
        else
            ob = objectsPosition[GameManager.instance.needFinalValue - 1];

        Vector3 dest = new Vector3(ob.transform.position.x, navMeshAgent.transform.position.y, ob.transform.position.z);
        navMeshAgent.transform.LookAt(dest);
    }

    ///Task che controlla se il sistema è in IDLE
    [Task]
    bool IsIdle()
    {
        if (GameManager.instance.CurrentGameStatus == (int)GameManager.GameStatus.Idle)
        {
            return true;
        }
        else
            return false;
    }

    ///Task che controlla se il sistema è in BUILD
    [Task]
    bool IsBuild()
    {
        if (GameManager.instance.CurrentGameStatus == (int)GameManager.GameStatus.Build)
        {
            return true;
        }
        else
            return false;
    }
    
    ///Task che controlla se il sistema è in GAME
    [Task]
    bool IsGame()
    {
        if (GameManager.instance.CurrentGameStatus == (int)GameManager.GameStatus.Game)
        {
            return true;
        }
        else
            return false;
    }

    ///Task che controlla se la navmesh è stata buildata
    [Task]
    bool IsNavMeshBuild()
    {
        if (GameManager.instance.isNavMeshReady == true)
            return true;
        else
            return false;
    }

    ///Task che controlla se il gioco è in stato di idle e la navmesh è stata buildatata
    [Task]
    bool IsIdleAndNavMeshBuild()
    {
        if (GameManager.instance.isNavMeshReady == true && (GameManager.instance.CurrentGameStatus == (int)GameManager.GameStatus.Idle))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Task che permette di muovere in una nuova posizione l'alieno
    /// </summary>
    [Task]
    void MoveTo()
    {
        // Se il task sta iniziando...
        if (Task.current.isStarting)
        {
            objectsPosition[0] = GameObject.Find("ciotolacibo(Clone)"); //cibo
            objectsPosition[1] = GameObject.Find("ciotola_acqua_piena(Clone)"); //drink
            objectsPosition[2] = GameObject.Find("cuscino_gonfio(Clone)"); //cuscino
            objectsPosition[3] = GameObject.Find("SparaBolleGO(Clone)"); //bolle
            objectsPosition[4] = GameObject.Find("mazzocarte(Clone)"); //carta
            objectsPosition[5] = GameObject.Find("PallaBella(Clone)"); //palla

            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

            path = new NavMeshPath();
        }
        else
        {
            // Se no cerca una nuova posizione per l'alieno, avvio animazione camminata
            GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
            alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.jump);

            // Cerco una nuova posizione
            //NewPosizion();
            getNewPath();
            Task.current.Succeed();
        }
    }


    /// <summary>
    /// Controlla se l'alieno ha raggiunto la destinazione, fatto solo in caso di bisogno da parte dell'alieno
    /// </summary>
    [Task]
    bool ReachDestination()
    {

        if (navMeshAgent.pathPending || navMeshAgent.remainingDistance > 0.1f)
            return false;
        else
        {
            // Se si è raggiunta la destinazione, l'animazione della camminata si ferma
            GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
            alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.idle);

            return true;
        }
    }
    
    /// <summary>
    /// Stampa sul log il bisogno dell'alieno (DEBUG)
    /// </summary>
    [Task]
    bool AskNeed(){
        string text = "I";
        if (GameManager.instance.needFinalValue == 1) {
            text += "'m hungry!";
        }
        if (GameManager.instance.needFinalValue == 2)
        {
            text += " want to drink!";
        }
        if (GameManager.instance.needFinalValue == 3)
        {
            text += "'m sleepy...";
        }
        if (GameManager.instance.needFinalValue == 4)
        {
            text += " want to play";
        }
        if (GameManager.instance.needFinalValue == 5)
        {
            text += " don't feel good...";
        }
        if (GameManager.instance.needFinalValue == 6)
        {
            text += "'m dirty!";
        }
        Debug.Log(text);
        return true;

    }

    /// <summary>
    /// Cerca un punto nella navmesh vicino a quello dell'oggetto da raggingere
    /// </summary>
    Vector3 getRandomNearPosition()
    {
        GameObject ob;
        if (GameManager.instance.needFinalValue == 4)
        {
            ob = objectsPosition[GameManager.instance.needFinalValue + GameManager.instance.gameChoose - 1];
        }
        else
            ob = objectsPosition[GameManager.instance.needFinalValue - 1];

        Vector3 from = navMeshAgent.transform.position;
        Vector3 to = ob.transform.position;
        float value = 0.80f;
        float dist = Vector3.Distance(from, to);
        if (dist <= 0.5)
            value = 0.25f;
        Debug.Log("Distace to destination" + dist);

        Vector3 result;
        if (GameManager.instance.needFinalValue == 3)
            result = to;
        else result = Vector3.Lerp(from, to, value);

        return result;
    }

    /// <summary>
    /// Funzione che scelie l'immagine da mettere nella nuvoletta, viene presa in base al bisogno dell'alieno
    /// </summary>
    void ChooseSprite() {
        GameObject sprite = GameObject.FindGameObjectWithTag("BubbleSpeach");
        Debug.Log("OnChooseSprite");
        switch (GameManager.instance.needFinalValue) {
            case 1: // cibo
                sprite.GetComponent<ProvaScript>().LaunchFadeIn((int)ProvaScript.BubbleType.hunger);
                break;
            case 2: // acqua
                sprite.GetComponent<ProvaScript>().LaunchFadeIn((int)ProvaScript.BubbleType.thirst  );
                break;
            case 3: // sonno
                sprite.GetComponent<ProvaScript>().LaunchFadeIn((int)ProvaScript.BubbleType.sleep);
                break;
            case 4: // gioco
                if (GameManager.instance.gameChoose == 0) // bolle
                    sprite.GetComponent<ProvaScript>().LaunchFadeIn((int)ProvaScript.BubbleType.bubble);

                if (GameManager.instance.gameChoose == 1) // carte                
                    sprite.GetComponent<ProvaScript>().LaunchFadeIn((int)ProvaScript.BubbleType.cards);

                if (GameManager.instance.gameChoose == 2) // palla                
                    sprite.GetComponent<ProvaScript>().LaunchFadeIn((int)ProvaScript.BubbleType.ball);
                break;
            case 5: // medicina
                sprite.GetComponent<ProvaScript>().LaunchFadeIn((int)ProvaScript.BubbleType.health);
                break;
            case 6: // pulizia
                sprite.GetComponent<ProvaScript>().LaunchFadeIn((int)ProvaScript.BubbleType.clean);
                break;
            default:
                break;
        }

    }
    
    /// <summary>
    /// A seconda che l'alieno abbia un bisogno o no questo si muove randomicamente o no
    /// </summary>
    void getNewPath()
    {
        // Setta la sprite corretta
        if (GameManager.instance.needFinalValue != 0)
        {
            ChooseSprite();
        }
        
        // Se l'alieno non ha dei bisogni o è malato o sporco... si muove randomicamente nella scena
        if (CalculateNeed.need == false || GameManager.instance.needFinalValue > 4)
        {
            Vector3 target_new = m_Range * Random.insideUnitSphere;
            target = new Vector3(target_new.x + navMeshAgent.transform.position.x, navMeshAgent.transform.position.y, target_new.z + navMeshAgent.transform.position.z);
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(target, out hit, Mathf.Infinity, NavMesh.AllAreas))
            {
                finalPosition = hit.position;
            }
            target = finalPosition;
        }
        // Se no, si dirige verso l'oggetto desiderato
        else
        {
           target = getRandomNearPosition();
        }
        navMeshAgent.SetDestination(target);
    }

    /* Test */
    [Task]
    void PrintTo(string text)
    {
        Debug.Log(text);
        Task.current.Succeed();
    }

}
