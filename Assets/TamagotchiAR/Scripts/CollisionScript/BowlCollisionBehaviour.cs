using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe che gestisce l'interazioni tra la ciotola del cibo e le cibarie
/// </summary>
public class BowlCollisionBehaviour : MonoBehaviour
{
    public static bool isFull;

    // Materiali dei vari oggetti con cui la ciotola piò interagire
    public Material[] typeFood = new Material[2];

    // Use this for initialization
    void Start()
    {
        isFull = false;
        NeedBehaviour.OnFullBowlComplete += FoodCollision;
        CalculateNeed.OnClearBowl += CleanBowl;
    }

    private void OnDestroy()
    {
        NeedBehaviour.OnFullBowlComplete -= FoodCollision;
        CalculateNeed.OnClearBowl -= CleanBowl;
    }
    /// <summary>
    /// Se l'oggetto è in collisione con un cibo, la ciotola si riempie; se no non succede nulla
    /// </summary>
    private void FoodCollision(GameObject food)
    {
        if (food.tag == "Food" && !isFull)
        {
            GameObject inactiveObject = transform.Find("Cibo").gameObject;
            inactiveObject.SetActive(true);
            if (food.name == "Carota(Clone)")
                inactiveObject.GetComponent<Renderer>().material = typeFood[0];
            if (food.name == "Ciliegia(Clone)")
                inactiveObject.GetComponent<Renderer>().material = typeFood[1];
            isFull = true;
        }
        else return;
    }


    /// <summary>
    /// Funzione che svuolta la ciotola del cibo
    /// </summary>
    public void CleanBowl(int needSatisfied)
    {
        if (isFull && (needSatisfied == 1))
        {
            GameObject inactiveObject = transform.Find("Cibo").gameObject;
            inactiveObject.SetActive(false);
            isFull = false;
        }

    }
}
