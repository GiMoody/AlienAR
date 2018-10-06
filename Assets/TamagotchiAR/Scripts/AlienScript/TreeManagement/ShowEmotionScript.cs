using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

/// <summary>
/// Classe che contiene il task dell'albero per scelgiere quale emozione deve mostrare l'alieno a fine azione
/// </summary>
public class ShowEmotionScript: MonoBehaviour {
    // Variabili per stabilire se l'alieno è arrabbiato o no
    public static bool isAngry = false;
    int ANGRY_CYCLE = 3;
    int count_angry_cycle = 0;

    // Vettori per salvare i materiali
    Material[] alienMaterial = new Material[4];


    [Task]
    void ShowEmotion()
    {
        GameObject alien = GameObject.FindGameObjectWithTag("Alien_Baby");
        alien.GetComponent<AlienBehaviour>().CambiaAnimazione((int)AlienBehaviour.AnimationStatus.idle);

        // Se l'alieno è malato, questo sarà triste
        if (AlienNeeds.instance.IsIll())
            alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.sad);
        else 
        {
            // Se è arrabbiato controllo il ciclo di "arrabbiatura"
            if (isAngry)
            {
                if (count_angry_cycle < ANGRY_CYCLE)
                {
                    // Cambio con texture arrabbiata
                    alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.angry);
                    count_angry_cycle++;
                }
                else
                {
                    count_angry_cycle = 0;
                    isAngry = false;
                }

            }
            else if (AlienNeeds.instance.getHappyness() > (AlienNeeds.MAX_HAPPY / 2 + 10))
            {
                // Cambio con texture felice
                alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.happy);
            }
            else if (AlienNeeds.instance.getHappyness() <= (AlienNeeds.MAX_HAPPY / 2 - 10))
            {
                // Cambio con texture triste
                alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.sad);
            }
            else
            {
                // Cambio con texture normale
                alien.GetComponent<AlienBehaviour>().CambiaStato((int)AlienBehaviour.EmotionsStatus.standard);
            }
        }
        Task.current.Succeed();

    }
}
