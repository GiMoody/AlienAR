using System;
using UnityEngine;
using System.Collections;

public class ClickManager : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }
        Debug.Log("Screen touched!");

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(touch.position);

        if (Physics.Raycast(ray, out hit))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();

            if (interactable != null)
                interactable.Interact(gameObject);

        }
    }
}
