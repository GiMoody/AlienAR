using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickAction : MonoBehaviour, IPointerClickHandler{

    /// <summary>
    /// Gesisce interazione con l'immagine della GUI dell'evoluzione e della morte
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // OnClick code goes here ...
        if (tag == "EvolutionGUI")
            GameManager.instance.ClickOnEvolutionMessage = true;
        else if (tag == "DeathGUI")
            GameManager.instance.ClickOnDeathMessage = true;
    }
}
