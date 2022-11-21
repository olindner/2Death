using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

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
    private static List<GameObject> AllEnemies = new List<GameObject>();
    private static GameObject GoldText;
    private static GameObject WaveText;
    private static GameObject targetPoint;
    private static GameObject autoButton;
    private static bool autoAttackOn = false;
    
    private static Color32 EnableColor = new Color32(195, 225, 165, 255);
    private static Color32 DisableColor = new Color32(225, 165, 165, 255);
    private int waveNumber = 0;
    
    void Start()
    {
        GoldText = gameObject.transform.Find("GoldText").gameObject;
        WaveText = gameObject.transform.Find("WaveText").gameObject;
        autoButton = gameObject.transform.Find("AutoButton").gameObject;
        autoButton.GetComponent<Image>().color = DisableColor;
        targetPoint = GameObject.Find("TargetPoint").gameObject;
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
        Debug.Log($"RT Global should be null: {GlobalTarget}");

        if (autoAttackOn)
        {
            GlobalTarget = FindClosestEnemy();
            Debug.Log($"RT set Global: {GlobalTarget}");
        }
        Debug.Log($"Done setting and Global is: {GlobalTarget}");
    }

    public void SpawnEnemy()
    {
        AllEnemies.RemoveAll(enemy => enemy == null);
        var enemy = Instantiate(GreenEnemy, SpawnPoint.transform.position, Quaternion.identity);
        AllEnemies.Add(enemy);
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
        AllEnemies.RemoveAll(enemy => enemy == null);
        for (var i = 0; i < numberOfEnemies; i++)
        {
            var enemy = Instantiate(GreenEnemy, SpawnPoint.transform.position, Quaternion.identity);
            AllEnemies.Add(enemy);
            yield return new WaitForSeconds(5);
        }
        yield return null;
    }

    static GameObject FindClosestEnemy()
    {
        AllEnemies.RemoveAll(enemy => enemy == null);

        var enemy = AllEnemies.Where(n => n)
            .OrderBy(n => (n.transform.position - targetPoint.transform.position).sqrMagnitude)
            .FirstOrDefault();
        Debug.Log($"FCE Global: {enemy}");
        return enemy;
    }

    public static void AutoAttackToggle()
    {
        if (autoAttackOn)
        {
            autoButton.GetComponent<Image>().color = DisableColor;
        }
        else
        {
            autoButton.GetComponent<Image>().color = EnableColor;
            GlobalTarget = FindClosestEnemy();
            Debug.Log($"AAT Global is now: {GlobalTarget}");
        }

        autoAttackOn = !autoAttackOn;
    }
}
