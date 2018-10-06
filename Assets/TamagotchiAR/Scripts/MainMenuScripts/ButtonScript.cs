using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ButtonScript : MonoBehaviour {
    private Vector3 scale;
    float originalWidth = 800.0f;  // define here the original resolution
    float originalHeight = 600.0f; // you used to create the GUI contents 

    public void CambiaTexture() {
        
        Debug.Log("Button script called");
        GameObject[] aliens = GameObject.FindGameObjectsWithTag("Alien_Baby");
        foreach (GameObject current in aliens) {
            current.GetComponent<AlienBehaviour>().CambiaStato(5);
        }

    }

    public void OnGUI() {
       // Debug.Log("OnGUI Called");
        
        scale.x = Screen.width / originalWidth; // calculate vert scale
        scale.y = scale.x; // this will keep your ratio base on Vertical scale
        scale.z = 1;
        //float scaleX = Screen.width / originalWidth; // store this for translate
        float scaleY = Screen.height / originalHeight;
        Matrix4x4 svMat = GUI.matrix; // save current matrix
        // substitute matrix - only scale is altered from standard
        GUI.matrix = Matrix4x4.TRS(new Vector3((scale.x - scaleY) / 2 * originalHeight, 0, 0), Quaternion.identity, scale);

    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
