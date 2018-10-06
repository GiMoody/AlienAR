using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BubbleBehaviour : Interactable {

    /// <summary>
    /// Coroutine che gestiscela durata vitale della bolla
    /// </summary>
    private Coroutine _BubbleLifetime;
    /// <summary>
    /// Suono dell'esplosione della bolla
    /// </summary>
    public AudioSource PopSound;

    void Start() {
        PopSound = GameObject.Find("AudioBubble").GetComponent<AudioSource>();
        _BubbleLifetime = StartCoroutine(BubbleLifetime());
    }

    /// <summary>
    /// Se dopo 3 secondi la bolla non viene toccata esplode naturalmente
    /// </summary>
    /// <returns></returns>
    private IEnumerator BubbleLifetime() {
        yield return new WaitForSecondsRealtime(3.0f);
        PopSound.Play();
        BubbleGameController.instance.SubScore(1);
        Destroy(gameObject);
    }

    /// <summary>
    /// Se la bolla è stata toccata, viene incrementato il punteggio
    /// </summary>
    /// <param name="caller"></param>
    public override void Interact(GameObject caller)
    {
        PopSound.Play();
        BubbleGameController.instance.AddScore(1);
        //AudioSource scoppio = GetComponent<AudioSource>();
        Destroy(gameObject);
    }

    /// <summary>
    /// Funzione per generare randomicamente la direzione della bolla
    /// </summary>
    public void Float()
    {        
        transform.eulerAngles = new Vector3(UnityEngine.Random.Range(-60, 60), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(-60, 60));
        Debug.Log("Float with angle: " + transform.eulerAngles);
        //float speed = 0.1f;
       
        Vector3 force = transform.up;
        //force = new Vector3(force.x, 1, force.z);
        gameObject.GetComponent<Rigidbody>().AddForce(force * BubbleGameController.instance.BubbleSpeed,ForceMode.VelocityChange);
    }
}
