using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {

    Queue<string> sentences;
    public Animator animator;
    public int[] transizioni;
    public int count = 0;
    public Text dialogueText;

    private void Start()
    {
        transizioni = new int[5];
        for (int i = 0; i < 5; i++)
        {

            transizioni[i] = i + 1;
        }
        sentences = new Queue<string>();
        sentences.Enqueue("Feed the alien to increment its happiness!");
        sentences.Enqueue("Cure the alien when it's ill!");
        sentences.Enqueue("Play with the alien! Choose the highest card or make bubbles pop around you!]]]");
        sentences.Enqueue("Clean the screen when it's dirty!");
    }




    // Update is called once per frame





    // Update is called once per frame
    public void Cliccato()
    {

        count++;

        if (count <= 4)
        {


            animator.SetInteger("Contatore", transizioni[count]);
            string sentence = sentences.Dequeue();
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));
            Debug.Log(count);

        }

        else
        {


            SceneManager.LoadScene("ScenaPrincipale");

        }

    }

    IEnumerator TypeSentence(string sentence)
    {

        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {

            dialogueText.text += letter;
            yield return null;


        }


    }



}
