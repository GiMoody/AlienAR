using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe per gestire le nuvolette di bisogno dell'alieno
/// </summary>
public class ProvaScript : MonoBehaviour {
    /// <summary>
    /// Renderer della sprite (nuvoletta)
    /// </summary>
    SpriteRenderer spriteRenderer;

    /// <summary>
    /// Enum che contiene tutti i tipi di nuvoletta dell'alieno
    /// </summary>
    public enum BubbleType { hunger, thirst, sleep, clean, bubble, cards, ball, health};

    /// <summary>
    /// Vettore di sprite corrispondente
    /// </summary>
    public Sprite[] sprites;
    
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update () {
        // Ad ogni update la nuvoletta guarda il giocatore
        transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.position);
	}

    /// <summary>
    /// Avvia Corutine per far apparire la nuvoletta e cambiare il bisogno in caso sia già attiva
    /// </summary>
    public void LaunchFadeIn(int iconType)
    {
        spriteRenderer.sprite = sprites[iconType];

        if (!spriteRenderer.enabled)
        {
            spriteRenderer.enabled = true;
            StartCoroutine(FadeIn(1f));
        }
    }

    /// <summary>
    /// Avvia Corutine per far scomparire la nuvoletta
    /// </summary>
    public void LaunchFadeOut()
    {
        if(spriteRenderer.enabled)
            StartCoroutine(FadeOut(1f));
    }

    IEnumerator FadeIn(float duration) {
        while (spriteRenderer.color.a < 1)
        {
            Color newColor = spriteRenderer.color;
            newColor.a += Time.deltaTime / duration;
            spriteRenderer.color = newColor;
            yield return null;
        }
    }

    IEnumerator FadeOut(float duration)
    {
        while (spriteRenderer.color.a > 0)
        {
            Color newColor = spriteRenderer.color;
            newColor.a -= Time.deltaTime / duration;
            spriteRenderer.color = newColor;
            yield return null;
        }
        spriteRenderer.enabled = false;
    }
}
