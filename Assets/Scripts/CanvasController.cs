using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasController : MonoBehaviour
{
    public static int TotalGold = 0;
    public GameObject GoldText;

    public static void AddGold(int amount) 
    {
        TotalGold += amount;
    }

    void Update()
    {
        GoldText.GetComponent<TextMeshProUGUI>().text = $"Gold: {TotalGold}";
    }
}
