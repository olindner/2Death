using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class CanvasController : MonoBehaviour
{
    public static int TotalGold = 0;
    public static GameObject GlobalTarget;

    //Turn these into Observable Lists
    public List<GameObject> TurretPlacements = new List<GameObject>();
    [SerializeField] List<GameObject> TurretPrefab;
    [SerializeField] GameObject SpawnPoint;
    [SerializeField] GameObject GreenEnemy;
    [SerializeField] GameObject MenuPanel;

    private List<GameObject> Turrets = new List<GameObject>();
    private List<int> EnemiesPerWave = new List<int>{ 0, 5, 10, 15, 20, 25};
    private static GameObject GoldText;
    private static GameObject WaveText;
    private int waveNumber = 0;
    
    void Start()
    {
        GoldText = gameObject.transform.Find("GoldText").gameObject;
        WaveText = gameObject.transform.Find("WaveText").gameObject;
        WaveText.GetComponent<TextMeshProUGUI>().text = $"Wave: {waveNumber.ToString()}";
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

        var turrentIndexToAdd = Turrets.Count;

        if (turrentIndexToAdd >= 3) return;

        var turretPrefabToInstantiate = TurretPrefab[turrentIndexToAdd];
        var positionToAddPrefab = TurretPlacements[turrentIndexToAdd].transform.position;

        var instantiatedTurret = Instantiate(turretPrefabToInstantiate, positionToAddPrefab, turretPrefabToInstantiate.transform.rotation);

        Turrets.Add(instantiatedTurret);
    }

    public void SpawnWave()
    {
        waveNumber++;
        WaveText.GetComponent<TextMeshProUGUI>().text = $"Wave: {waveNumber.ToString()}";
        StartCoroutine(SpawnWaveInternal(EnemiesPerWave[waveNumber]));
    }

    IEnumerator SpawnWaveInternal(int numberOfEnemies)
    {
        for (var i = 0; i < numberOfEnemies; i++)
        {
            Instantiate(GreenEnemy, SpawnPoint.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(3);
        }
        yield return null;
    }
}
