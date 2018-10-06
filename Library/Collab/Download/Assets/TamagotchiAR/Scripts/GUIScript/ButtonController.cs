using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe che controlla parte dei comandi della GUI: precisamente controlla se i bottoni vengono abilitati o no
/// </summary>
public class ButtonController : MonoBehaviour {
    
    /// <summary>
    /// Enum di tutti i bottoni presenti sulla scena
    /// </summary>
    public enum Buttons { cibo, ciliegia, carota, acqua, cura, cerotto, pillola, pulizia, help, inventario, opzioni }

    // Vettore di bottoni
    public GameObject[] buttons;

    // GUI per l'help e l'uscita dal gioco
    public GameObject HelpGUI;
    public GameObject ExitGameGUI;

    /// <summary>
    /// Gestisce quali iconi devono essere visualizzate sullo schermo a seconda dei comandi dell'utente 
    /// </summary>
    public void ClickOnInventario()
    {
        // Comandi attivi solo se non si è in modalità HELP
        if (!HelpGUI.activeSelf)
        {
            // Se i bottini cibo e cura sono attivati e si riclicca sul bottone invetario questi vengono disattivati
            if (buttons[(int)Buttons.cibo].activeInHierarchy && buttons[(int)Buttons.cura].activeInHierarchy)
            {
                // Disabilita anche la GUI di exit
                DisableExitGui();

                buttons[(int)Buttons.cibo].SetActive(false);
                buttons[(int)Buttons.cura].SetActive(false);

                //Cibo
                buttons[(int)Buttons.ciliegia].SetActive(false);
                buttons[(int)Buttons.carota].SetActive(false);
                buttons[(int)Buttons.acqua].SetActive(false);

                //Cura
                buttons[(int)Buttons.cerotto].SetActive(false);
                buttons[(int)Buttons.pillola].SetActive(false);
            }
            // Se invece non sono attivi, vengono attivati
            else
            {
                buttons[(int)Buttons.cibo].SetActive(true);
                buttons[(int)Buttons.cura].SetActive(true);
            }
        }
    }

    /// <summary>
    /// Gestisce quali icone riguardo a cibo e bevande devono essere visualizzate sullo schermo a seconda dei comandi dell'utente 
    /// </summary>
    public void ClickOnCibo()
    {
        // Comandi attivi solo se non si è in modalità HELP
        if (!HelpGUI.activeSelf)
        {
            // Disabilita anche la GUI di exit
            DisableExitGui();

            // Se i bottini ciliegia, carota e acqua sono attivati e si riclicca sul bottone cibo questi vengono disattivati
            if (buttons[(int)Buttons.ciliegia].activeInHierarchy && buttons[(int)Buttons.carota].activeInHierarchy && buttons[(int)Buttons.acqua].activeInHierarchy)
            {
                buttons[(int)Buttons.ciliegia].SetActive(false);
                buttons[(int)Buttons.carota].SetActive(false);
                buttons[(int)Buttons.acqua].SetActive(false);
            }
            // Se invece non sono attivi, vengono attivati
            else
            {
                buttons[(int)Buttons.ciliegia].SetActive(true);
                buttons[(int)Buttons.carota].SetActive(true);
                buttons[(int)Buttons.acqua].SetActive(true);
            }
        }
    }

    /// <summary>
    /// Gestisce quali icone riguardo alle cure devono essere visualizzate sullo schermo a seconda dei comandi dell'utente 
    /// </summary>
    public void ClickOnCure()
    {
        // Comandi attivi solo se non si è in modalità HELP
        if (!HelpGUI.activeSelf)
        {            
            // Disabilita anche la GUI di exit
            DisableExitGui();

            // Se i bottini cerotto e pillola sono attivati e si riclicca sul bottone cure questi vengono disattivati
            if (buttons[(int)Buttons.cerotto].activeInHierarchy && buttons[(int)Buttons.pillola].activeInHierarchy)
            {
                buttons[(int)Buttons.cerotto].SetActive(false);
                buttons[(int)Buttons.pillola].SetActive(false);
            }
            // Se invece non sono attivi, vengono attivati
            else
            {
                buttons[(int)Buttons.cerotto].SetActive(true);
                buttons[(int)Buttons.pillola].SetActive(true);
            }
        }
    }


    void Update()
    {
        // In caso siano attive le GUI riguardo l'help e l'exit vengono disattivati tutti i bottoni dell'inventario
        if (GameManager.instance.isNavMeshReady == true)
        {
            if (HelpGUI.activeInHierarchy || ExitGameGUI.activeInHierarchy)
            {
                buttons[(int)Buttons.cibo].SetActive(false);
                buttons[(int)Buttons.cura].SetActive(false);

                //Cibo
                buttons[(int)Buttons.ciliegia].SetActive(false);
                buttons[(int)Buttons.carota].SetActive(false);
                buttons[(int)Buttons.acqua].SetActive(false);

                //Cura
                buttons[(int)Buttons.cerotto].SetActive(false);
                buttons[(int)Buttons.pillola].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Gestisce il comando opzioni
    /// </summary>
    public void ClickOnOption()
    {
        // Se non è attivato il comando help
        if (!HelpGUI.activeInHierarchy)
        {
            // Se è già attivo il comando opzioni, disattivo la GUI exit e tolgo dalla pausa il gioco
            if (ExitGameGUI.activeInHierarchy)
            {
                Time.timeScale = 1;
                ExitGameGUI.SetActive(false);
            }
            // Se non è attivo , attivo la GUI exit e metto in pausa il gioco
            else
            {
                Time.timeScale = 0;
                ExitGameGUI.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Disattiva la GUI exit e toglie il gioco dalla pausa 
    /// </summary>
    public void DisableExitGui() {
        Time.timeScale = 1;
        ExitGameGUI.SetActive(false);
    }
}
