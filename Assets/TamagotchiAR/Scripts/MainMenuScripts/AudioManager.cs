using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

    // Use this for initialization
    public Slider slider;
    public AudioSource audio;
	
	// Update is called once per frame
	void Update () {

        audio.volume = slider.value;
		
	}
}
