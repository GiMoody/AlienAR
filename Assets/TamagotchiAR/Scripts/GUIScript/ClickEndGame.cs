using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickEndGame : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Gestisce l'interazione alla fine del gioco
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("FineGioco");
        GameManager.instance.pressedScreenEndGame = true;
    }

}
