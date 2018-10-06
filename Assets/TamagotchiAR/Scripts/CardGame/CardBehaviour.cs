using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBehaviour : Interactable {

    

    // Use this for initialization
    public override void Interact(GameObject caller)
    {

        Animator anim = GetComponent<Animator>();
        anim.SetInteger("Count", 1);

        
    }

}
