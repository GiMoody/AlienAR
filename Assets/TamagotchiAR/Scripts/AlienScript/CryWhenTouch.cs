using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestisce il tocco da parte dell'utente sull'alieno, questo risponde col suo verso
/// </summary>
public class CryWhenTouch : MonoBehaviour {
	void Update () {
        // Solo se l'alieno non è morto, non si sta evolvendo e il gioco non è in fase di idle...
        // fa il verso di Bulbasaur se toccato
        if (AlienNeeds.instance.IsItDead()) return;
        if (AlienNeeds.instance.IsEvolving()) return;

        if (Input.touchCount > 0 && GameManager.instance.CurrentGameStatus == (int)GameManager.GameStatus.Idle)
        {

            Touch touch = Input.GetTouch(0);
            Camera FirstPersonCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Vector3 touchPos3D = FirstPersonCamera.ScreenToWorldPoint(touch.position);
            // float distance = 0.15f;
            if (touch.phase == TouchPhase.Ended)
            {
                RaycastHit hit;
                Ray ray = FirstPersonCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        GetComponent<AudioSource>().Play();
                    }
                }
            }
        }
    }
    
}
