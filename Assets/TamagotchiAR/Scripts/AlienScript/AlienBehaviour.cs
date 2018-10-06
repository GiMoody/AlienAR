using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using GoogleARCore;
using GoogleARCore.HelloAR;

public class AlienBehaviour : MonoBehaviour
{
    // Enums per le varie animazioni e stati emotivi dell'alieno
    public enum EmotionsStatus { standard, sleeping, happy, eating, sad, angry };
    public enum AnimationStatus { idle, jump, eating, die };

    // Animazione e stato di default
    public int DefaultState = (int)EmotionsStatus.standard;
    public int DefaultAnimation = (int)AnimationStatus.idle;

    // Animazione e stato corrente
    private int _CurrentStatus;
    private int _CurrentAnimation;

    public Texture[] phases;

    /// <summary>
    /// Ritorna lo stato corrente dell'alieno
    /// </summary>
    public int CurrentStatus
    {
        get
        {
            return _CurrentStatus;
        }

    }
    
    /// <summary>
    /// Ritorna l'animazione corrente dell'alieno
    /// </summary>
    public int CurrentAnimation
    {
        get
        {
            return _CurrentAnimation;
        }

    }

    /// <summary>
    /// Cambia lo stato dell'alieno in quello indicato da statusCode
    /// </summary>
    public void CambiaStato(int statusCode)
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.material.mainTexture = phases[statusCode];
        }
        else
        {
            SkinnedMeshRenderer smr = GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                smr.material.mainTexture = phases[statusCode];
            }

        }
        _CurrentStatus = statusCode;
    }


    /// <summary>
    /// Cambia l'animazione dell'alieno in quella indicatA da animationCode
    /// </summary>
    public void CambiaAnimazione(int animationCode)
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetInteger("Contatore", animationCode);
        }
        _CurrentStatus = animationCode;
    }

    // Use this for initialization
    void Start()
    {
        // Appena avviato il gioco vengono settate animazione e stato di default dell'alieno
        CambiaStato(DefaultState);
        CambiaAnimazione(DefaultAnimation);
    }

}
