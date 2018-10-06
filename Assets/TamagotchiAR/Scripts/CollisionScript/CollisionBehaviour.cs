using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Classe che gestisce le collisioni durante la fase di building
/// </summary>
public class CollisionBehaviour : MonoBehaviour {

    /// <summary>
    ///  Se due oggetti iniziano a collidere, viene settata la variabile buildColliding come vera
    /// </summary>
    private void OnTriggerEnter(Collider collision)
    {
        if (GameManager.instance.CurrentGameStatus == (int)GameManager.GameStatus.Build)
        {
            if (collision.gameObject.tag != "Ground")
            {
                AlienPlacerController.buildColliding = true;
            }
        }
    }


    /// <summary>
    ///  Se due oggetti smettono di collidere, viene settata la variabile buildColliding come false
    /// </summary>
    private void OnTriggerExit(Collider collision)
    {
        if (GameManager.instance.CurrentGameStatus == (int)GameManager.GameStatus.Build)
        {
            if (collision.gameObject.tag != "Ground")
                AlienPlacerController.buildColliding = false;
        }

    }
}
