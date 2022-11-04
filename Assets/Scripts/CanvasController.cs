using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class CanvasController : MonoBehaviour
{
    public static int TotalGold = 0;
    public static GameObject GlobalTarget;

    public List<GameObject> TurretPlacements = new List<GameObject>();
    public GameObject TurretPrefab;
    public GameObject SpawnPoint;
    public GameObject GreenEnemy;
    public GameObject MenuPanel;

    private List<GameObject> Turrets = new List<GameObject>();
    private List<int> EnemiesPerWave = new List<int>{ 0, 5, 10, 15, 20, 25};
    private static GameObject GoldText;
    private static GameObject WaveText;
    private int waveNumber = 1;
    
    void Start()
    {
        GoldText = gameObject.transform.Find("GoldText").gameObject;
        // WaveText = gameObject.transform.Find("WaveText").gameObject;
    }

    public static void AddGold(int amount) 
    {
        TotalGold += amount;
        GoldText.GetComponent<TextMeshProUGUI>().text = $"Gold: {TotalGold}";
    }

    public static void ResetTarget()
    {
        GlobalTarget = null;
    }

    public void SpawnEnemy()
    {
        Instantiate(GreenEnemy, SpawnPoint.transform.position, Quaternion.identity);
    }

    public void OpenMenu()
    {
        if (MenuPanel.activeSelf) MenuPanel.SetActive(false);
        else MenuPanel.SetActive(true);
    }

    public void BuildTurret()
    {
        // Check if enough money

        // Check which placement spot to use
        var turrentIndexToAdd = Turrets.Count;
        if (turrentIndexToAdd >= 3) return;

        var newTurret = Instantiate(TurretPrefab, TurretPlacements[turrentIndexToAdd].transform.position, TurretPrefab.transform.rotation);
        Turrets.Add(newTurret);
    }

    // IEnumerator SpawnWave(int numberOfEnemies)
    // {

    // }
}
