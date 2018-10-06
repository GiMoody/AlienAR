using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionExit : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Gestisce l'interazione con l'immagine dentro la GUI Exit
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Se l'immagine viene cliccata si esce dalla pausa e si disattiva la GUI exit
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
