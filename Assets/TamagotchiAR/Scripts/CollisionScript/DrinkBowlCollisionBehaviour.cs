using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe che gestisce l'interazioni tra la ciotola dell'acqua ed le bevande
/// </summary>
public class DrinkBowlCollisionBehaviour : MonoBehaviour {

    public static bool isFull;

    // Materiali dei vari oggetti con cui la ciotola piò interagire
    public Material[] typeDrink = new Material[1];
    public Material defaultMaterial;
    
    // Use this for initialization
    void Start()
    {
        isFull = false;
        NeedBehaviour.OnFullDrinkBowlComplete += DrinkCollision;
        CalculateNeed.OnClearBowl += CleanWaterBowl;
    }
    private void OnDestroy()
    {
        NeedBehaviour.OnFullDrinkBowlComplete -= DrinkCollision;
        CalculateNeed.OnClearBowl -= CleanWaterBowl;
    }

    /// <summary>
    /// Se l'oggetto è in collisione con una bevanda, la ciotola si riempie; se no non succede nulla
    /// </summary>
    private void DrinkCollision(GameObject drink)
    {
        if (drink.tag == "Drink" && !isFull)
        {
            GameObject inactiveObject = transform.Find("Container").gameObject;
            if (drink.name == "Acqua(Clone)")
                inactiveObject.GetComponent<Renderer>().material = typeDrink[0];
            isFull = true;
        }
        else return;
    }

    /// <summary>
    /// Funzione che svuolta la ciotola dell'acqua
    /// </summary>
    public void CleanWaterBowl(int needSatisfied)
    {
        if (isFull && (needSatisfied == 2))
        {
            GameObject inactiveObject = transform.Find("Container").gameObject;
            inactiveObject.GetComponent<Renderer>().material = defaultMaterial;
    
            isFull = false;
        }
    }
}
