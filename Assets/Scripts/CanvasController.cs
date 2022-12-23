using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    //Turn these into Observable Lists
    public List<GameObject> TurretPlacements = new List<GameObject>();
    [SerializeField] List<GameObject> TurretPrefab;
    [SerializeField] GameObject SpawnPoint;
    [SerializeField] GameObject GreenEnemy;
    [SerializeField] GameObject MenuPanel;

    private List<int> EnemiesPerWave = new List<int>{ 0, 5, 10, 15, 20, 25};
    
    private GameObject goldText;
    private GameObject waveText;
    private GameObject autoButton;
    private GameObject backgroundObject;
    private Sprite blueBackground;
    private Sprite greenBackground;
    private Sprite purpleBackground;
    
    private Color32 EnableColor = new Color32(195, 225, 165, 255);
    private Color32 DisableColor = new Color32(225, 165, 165, 255);
    private int waveNumber = 0;
    
    private void Awake()
    {
        GameManager.Instance.TotalGoldChanged += TotalGoldChanged;
        GameManager.Instance.AutoAttackChanged += AutoAttackChanged;
    }

    private void OnDestroy()
    {
        GameManager.Instance.TotalGoldChanged -= TotalGoldChanged;
        GameManager.Instance.AutoAttackChanged -= AutoAttackChanged;
    }

    private void Start()
    {
        goldText = gameObject.transform.Find("GoldText").gameObject;

        waveText = gameObject.transform.Find("WaveText").gameObject;
        waveText.GetComponent<TextMeshProUGUI>().text = $"Wave: {waveNumber.ToString()}";

        autoButton = gameObject.transform.Find("AutoButton").gameObject;
        autoButton.GetComponent<Button>().onClick.AddListener(ButtonClicked);
        autoButton.GetComponent<Image>().color = DisableColor;

        backgroundObject = GameObject.Find("Background");
        blueBackground = Resources.Load<Sprite>("BlueBackground");
        greenBackground = Resources.Load<Sprite>("GreenBackground");
        purpleBackground = Resources.Load<Sprite>("PurpleBackground");
        backgroundObject.GetComponent<SpriteRenderer>().sprite = blueBackground;
    }

    public void TotalGoldChanged(int newTotalGold) 
    {
        goldText.GetComponent<TextMeshProUGUI>().text = $"Gold: {newTotalGold}";
    }

    public void SpawnEnemy()
    {
        var enemy = Instantiate(GreenEnemy, SpawnPoint.transform.position, Quaternion.identity);
        GameManager.Instance.AddEnemy(enemy);
    }

    public void OpenMenu()
    {
        if (MenuPanel.activeSelf) MenuPanel.SetActive(false);
        else MenuPanel.SetActive(true);
    }

    public void BuildTurret()
    {
        // Check if enough money

        var turrentIndexToAdd = GameManager.Instance.AllTurrets.Count;

        if (turrentIndexToAdd >= 3) return;

        var turretPrefabToInstantiate = TurretPrefab[turrentIndexToAdd];
        var positionToAddPrefab = TurretPlacements[turrentIndexToAdd].transform.position;

        var instantiatedTurret = Instantiate(turretPrefabToInstantiate, positionToAddPrefab, turretPrefabToInstantiate.transform.rotation);

        GameManager.Instance.AllTurrets.Add(instantiatedTurret);
    }

    public void SpawnWave()
    {
        waveNumber++;

        if (waveNumber == 2)
        {
            backgroundObject.GetComponent<SpriteRenderer>().sprite = greenBackground;
        }
        if (waveNumber == 3)
        {
            backgroundObject.GetComponent<SpriteRenderer>().sprite = purpleBackground;
        }

        waveText.GetComponent<TextMeshProUGUI>().text = $"Wave: {waveNumber.ToString()}";
        StartCoroutine(SpawnWaveInternal(EnemiesPerWave[waveNumber]));
    }

    IEnumerator SpawnWaveInternal(int numberOfEnemies)
    {
        for (var i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(5);
        }
        yield return null;
    }

    public void AutoAttackChanged(bool autoAttack)
    {
        autoButton.GetComponent<Image>().color = autoAttack ? EnableColor : DisableColor;
    }

    private void ButtonClicked()
    {
        GameManager.Instance.ToggleAutoAttack();
    }
}
