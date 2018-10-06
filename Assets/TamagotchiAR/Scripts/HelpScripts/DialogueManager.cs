using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    // Use this for initialization
    public GameObject help;
    public Queue<string> sentences;

    public Text dialogueText;

    // Use this for initialization
    void Start()
    {
        sentences = new Queue<string>();
    }

    /// <summary>
    /// Inizia l'animazione della GUI help
    /// </summary>
    public void StartDialogue(Dialogue dialogue)
    {
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    /// <summary>
    /// Passa alla riga successiva fino a che non finiscono
    /// </summary>
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    /// <summary>
    /// Permette l'animazione della frase
    /// </summary>
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    /// <summary>
    /// A fine GUI help viene chiuso il menù e tolto dalla pausa il gioco
    /// </summary>
    void EndDialogue()
    {
        help.SetActive(false);
        Time.timeScale = 1;
    }
}
