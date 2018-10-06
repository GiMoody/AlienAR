using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script che permette di far l'effetto particellare "puf" 
/// </summary>
public class EvolutionEffectScript : MonoBehaviour {

    public GameObject EffectLight;
    public ParticleSystem PuffStars;

    /*void Start()
    {
        EffectLight = GameObject.FindGameObjectWithTag("EffectLight");
        PuffStars = GameObject.FindGameObjectWithTag("PuffStars").GetComponent<ParticleSystem>();
    }*/

    // Update is called once per frame
    void Update() {
        if (PuffStars.isStopped) {
            EffectLight.SetActive(false);
        }
    }

    /// <summary>
    /// Fa partire l'effetto particellare nella posizione dell'alieno
    /// </summary>
    public void StartAnimation(Vector3 alienPosition) {
        transform.position = alienPosition;
        EffectLight.SetActive(true);
        PuffStars.Play();
    }
}
