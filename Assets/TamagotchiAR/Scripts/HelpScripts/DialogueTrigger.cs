using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    // Use this for initialization
    public Dialogue dialogue;
    public GameObject help;

    // Messo in caso i messaggi non vengano salvati
    /*public void Start()
    {
        dialogue = new Dialogue();
        dialogue.sentences[0] = "Benvenuto nella sezione help del gioco!";
        dialogue.sentences[1] = "Soddisfa i bisogni dell'alieno per andare avanti cliccando sui pulsanti giusti all'interno della scena. L'alieno avrà bisogno di cibo e cure, clicca i relativi pulsanti quando sarà necessario!";
        dialogue.sentences[2] = "Pulisci lo schermo quando è sporco cliccando il pulsante in basso a destra.";
        dialogue.sentences[3] = "Dai da mangiare, bere e cure all'alieno aprendo l'inventario in basso a sinistra. Tocca sull'icona dell'oggetto desiderato e posizionalo nella scena, dopo di che: trascina il cibo verso la citola, l'acqua verso l'abbeveratoio e le medicine sull'alieno.";
        dialogue.sentences[4] = "Gioca con l'alieno quando si avvicina a uno dei giochi. Scoppia le bolle, scegli la carta che secondo te è più alta o gioca con lui a palla!";
        dialogue.sentences[5] = "Se vuoi uscire dal gioco tocca l'ingranaggio in alto a sinistra, il gioco verrà salvato in automatico.\nSe hai bisogno ancora di questo menù clicca il pulsante in alto a destra.";
        dialogue.sentences[6] = "Divertiti :)";
    }*/

    /// <summary>
    /// Avvia la GUI Help e mette in pausa il gioco
    /// </summary>
    public void TriggerDialogue()
    {
        
        help.SetActive(true);
        Time.timeScale = 0;
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

    }
}
