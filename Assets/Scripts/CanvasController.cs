using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameState = GameManager.GameState;

public class CanvasController : MonoBehaviour
{
    //Turn these into Observable Lists
    public List<GameObject> TurretPlacements = new List<GameObject>();
    [SerializeField] List<GameObject> TurretPrefab;
    [SerializeField] GameObject MenuPanel;

    private AudioClip hoverClip;
    private AudioSource audioSource;

    private GameObject autoButton;
    private GameObject backgroundObject;
    private GameObject menuButton;

    private Sprite blueBackground;
    private Sprite greenBackground;
    private Sprite purpleBackground;
    
    private TextMeshProUGUI enemiesRemainingTextMesh;
    private TextMeshProUGUI goldTextMesh;
    private TextMeshProUGUI waveNumberTextMesh;

    private Color32 EnableColor = new Color32(195, 225, 165, 255);
    private Color32 DisableColor = new Color32(225, 165, 165, 255);
    
    #region Built In Functions
    private void Awake()
    {
        GameManager.Instance.AutoAttackChanged += AutoAttackChanged;
        GameManager.Instance.EnemyCountChanged += EnemyCountChanged;
        GameManager.Instance.TotalGoldChanged += TotalGoldChanged;
        GameManager.Instance.WaveNumberChanged += WaveNumberChanged;

        blueBackground = Resources.Load<Sprite>("BlueBackground");
        greenBackground = Resources.Load<Sprite>("GreenBackground");
        purpleBackground = Resources.Load<Sprite>("PurpleBackground");
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AutoAttackChanged -= AutoAttackChanged;
            GameManager.Instance.EnemyCountChanged -= EnemyCountChanged;
            GameManager.Instance.TotalGoldChanged -= TotalGoldChanged;
            GameManager.Instance.WaveNumberChanged -= WaveNumberChanged;
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

        enemiesRemainingTextMesh = gameObject.transform.Find("EnemiesRemainingText").gameObject.GetComponent<TextMeshProUGUI>();
        goldTextMesh = gameObject.transform.Find("GoldText").gameObject.gameObject.GetComponent<TextMeshProUGUI>();

        menuButton = gameObject.transform.Find("MenuButton").gameObject;
        menuButton.GetComponent<Button>().onClick.AddListener(MenuButtonClicked);
        InitButtonHoverEvent(menuButton);

        waveNumberTextMesh = gameObject.transform.Find("WaveNumberText").gameObject.GetComponent<TextMeshProUGUI>();

        // Only after Canvas has initialized successfully allow the first wave to spawn
        GameManager.Instance.UpdateGameState(GameState.SpawnWave);
    }
    #endregion

    #region Observables
    public void AutoAttackChanged(bool autoAttack)
    {
        autoButton.GetComponent<Image>().color = autoAttack ? EnableColor : DisableColor;
    }
    
    public void EnemyCountChanged(int newCount) 
    {
        enemiesRemainingTextMesh.text = $"Enemies Remaining {newCount}";
        // StartCoroutine(GrowTextAndFadeBack(enemiesRemainingTextMesh));
    }

    public void TotalGoldChanged(int newTotalGold) 
    {
        goldTextMesh.text = $"Gold {newTotalGold}";
        StartCoroutine(GrowTextAndFadeBack(goldTextMesh));
    }

    public void WaveNumberChanged(int newWaveNumber) 
    {
        waveNumberTextMesh.text = $"Wave {newWaveNumber}";
        StartCoroutine(GrowTextAndFadeBack(waveNumberTextMesh));

        if (newWaveNumber == 3)
        {
            backgroundObject.GetComponent<SpriteRenderer>().sprite = greenBackground;
        }
        if (newWaveNumber == 5)
        {
            backgroundObject.GetComponent<SpriteRenderer>().sprite = purpleBackground;
        }
    }
    #endregion

    #region Buttons
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

    private void AutoButtonClicked()
    {
        GameManager.Instance.ToggleAutoAttack();
    }

    private void MenuButtonClicked()
    {
        if (MenuPanel.activeSelf) MenuPanel.SetActive(false);
        else MenuPanel.SetActive(true);
    }

    private void HoverMouseNoise(UnityEngine.EventSystems.BaseEventData baseEvent)
    {
        audioSource.PlayOneShot(hoverClip);
    }
    #endregion

    #region Turrets
    public void BuildTurret()
    {
        var numberOfUnlockedTurrets = GameManager.Instance.AllTurrets.Count;
        if (numberOfUnlockedTurrets >= 3) return;

        if (!SuccessfullyPaidForTurret(numberOfUnlockedTurrets)) return;

        var turretPrefabToInstantiate = TurretPrefab[numberOfUnlockedTurrets];
        var positionToAddPrefab = TurretPlacements[numberOfUnlockedTurrets].transform.position;

        var instantiatedTurret = Instantiate(turretPrefabToInstantiate, positionToAddPrefab, turretPrefabToInstantiate.transform.rotation);

        GameManager.Instance.AllTurrets.Add(instantiatedTurret);
    }

    private bool SuccessfullyPaidForTurret(int numberOfUnlockedTurrets)
    {
        var costOfNextTurret = GameManager.Instance.TurretCosts[numberOfUnlockedTurrets];
        if (GameManager.Instance.TotalGold < costOfNextTurret) return false;

        GameManager.Instance.ChangeTotalGoldBy(-costOfNextTurret);
        return true;
    }
    #endregion

    #region Helpers
    private IEnumerator GrowTextAndFadeBack(TextMeshProUGUI mesh)
    {
        float sizeStart = 40f;
        float sizeEnd = 36f;
        float t = 0f;
        while(t < 1f)
        {            
            t += Time.deltaTime;
            mesh.fontSize = (int)Mathf.Lerp (sizeStart, sizeEnd, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }        
    }
    #endregion
}
