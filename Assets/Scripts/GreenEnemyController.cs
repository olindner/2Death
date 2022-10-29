using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenEnemyController : MonoBehaviour
{
    private int goldWorth = 5;

    void OnMouseDown()
    {
        Debug.Log("Killed");

        CanvasController.AddGold(goldWorth);

        Destroy(gameObject);
    }
}
