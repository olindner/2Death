using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CanvasController : MonoBehaviour
{
    //Turn these into Observable Lists
    public List<GameObject> TurretPlacements = new List<GameObject>();
    [SerializeField] List<GameObject> TurretPrefab;
    [SerializeField] GameObject SpawnPoint;
    [SerializeField] GameObject GreenEnemy;
    [SerializeField] GameObject MenuPanel;

    private List<int> EnemiesPerWave = new List<int>{ 0, 5, 10, 15, 20, 25};

    private AudioClip hoverClip;
    private AudioSource audioSource;

    private GameObject autoButton;
    private GameObject backgroundObject;
    private GameObject goldText;
    private GameObject menuButton;
    private GameObject spawnWaveButton;
    private GameObject waveText;

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

        blueBackground = Resources.Load<Sprite>("BlueBackground");
        greenBackground = Resources.Load<Sprite>("GreenBackground");
        purpleBackground = Resources.Load<Sprite>("PurpleBackground");
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TotalGoldChanged -= TotalGoldChanged;
            GameManager.Instance.AutoAttackChanged -= AutoAttackChanged;
        }
    }

    private void Start()
    {
        hoverClip = Resources.Load("Click") as AudioClip;
        audioSource = gameObject.AddComponent<AudioSource>();

        autoButton = gameObject.transform.Find("AutoButton").gameObject;
        autoButton.GetComponent<Button>().onClick.AddListener(AutoButtonClicked);
        autoButton.GetComponent<Image>().color = DisableColor;
        InitButtonHoverEvent(autoButton);

        backgroundObject = GameObject.Find("Background");
        backgroundObject.GetComponent<SpriteRenderer>().sprite = blueBackground;

        goldText = gameObject.transform.Find("GoldText").gameObject;

        menuButton = gameObject.transform.Find("MenuButton").gameObject;
        menuButton.GetComponent<Button>().onClick.AddListener(MenuButtonClicked);
        InitButtonHoverEvent(menuButton);

        spawnWaveButton = gameObject.transform.Find("SpawnWaveButton").gameObject;
        spawnWaveButton.GetComponent<Button>().onClick.AddListener(SpawnWaveButtonClicked);
        InitButtonHoverEvent(spawnWaveButton);

        waveText = gameObject.transform.Find("WaveText").gameObject;
        waveText.GetComponent<TextMeshProUGUI>().text = $"Wave: {waveNumber.ToString()}";
    }

    private void InitButtonHoverEvent(GameObject button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;

        entry.callback = new EventTrigger.TriggerEvent();
        UnityEngine.Events.UnityAction<BaseEventData> call = new UnityEngine.Events.UnityAction<BaseEventData>(HoverMouseNoise);
        entry.callback.AddListener(call);
        trigger.triggers.Add(entry);
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

    private void AutoButtonClicked()
    {
        GameManager.Instance.ToggleAutoAttack();
    }

    private void MenuButtonClicked()
    {
        if (MenuPanel.activeSelf) MenuPanel.SetActive(false);
        else MenuPanel.SetActive(true);
    }
    
    private void SpawnWaveButtonClicked()
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

    private void HoverMouseNoise(UnityEngine.EventSystems.BaseEventData baseEvent)
    {
        audioSource.PlayOneShot(hoverClip);
    }
}
