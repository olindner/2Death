using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasController : MonoBehaviour
{
    public static int TotalGold = 0;
    public GameObject SpawnPoint;
    public GameObject GreenEnemy;
    public static GameObject GlobalTarget;

    private GameObject GoldText;
    
    void Start()
    {
        GoldText = gameObject.transform.Find("GoldText").gameObject;
    }

    public static void AddGold(int amount) 
    {
        TotalGold += amount;
    }

    public static void ResetTarget()
    {
        GlobalTarget = null;
    }

    void Update()
    {
        GoldText.GetComponent<TextMeshProUGUI>().text = $"Gold: {TotalGold}";
    }

    public void SpawnEnemy()
    {
        Instantiate(GreenEnemy, SpawnPoint.transform.position, Quaternion.identity);
    }
}
