using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameState = GameManager.GameState;

public class CanvasController : MonoBehaviour
{
    #region Variable References
    private AudioClip hoverClip;
    private AudioSource audioSource;

    private GameObject autoAttackButton;
    private GameObject backgroundObject;
    private GameObject menuButton;
    private GameObject menuPanel;
    private GameObject turretTextParent;

    private List<GameObject> turretTextList;

    private Sprite blueBackground;
    private Sprite greenBackground;
    private Sprite purpleBackground;
    
    private TextMeshProUGUI advancedTurretTextMesh;
    private TextMeshProUGUI basicTurretTextMesh;
    private TextMeshProUGUI buildTurretCostTextMesh;
    private TextMeshProUGUI enemiesRemainingTextMesh;
    private TextMeshProUGUI goldTextMesh;
    private TextMeshProUGUI regularTurretTextMesh;
    private TextMeshProUGUI waveNumberTextMesh;
    #endregion
    
    #region Built In Functions
    private void Awake()
    {
        GameManager.Instance.AutoAttackChanged += AutoAttackChanged;
        GameManager.Instance.EnemyCountChanged += EnemyCountChanged;
        GameManager.Instance.NewTurretBuilt += NewTurretBuilt;
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
            GameManager.Instance.NewTurretBuilt -= NewTurretBuilt;
            GameManager.Instance.TotalGoldChanged -= TotalGoldChanged;
            GameManager.Instance.WaveNumberChanged -= WaveNumberChanged;
        }
    }

    private void Start()
    {
        advancedTurretTextMesh = GameObject.Find("AdvancedTurretText").gameObject.GetComponent<TextMeshProUGUI>();

        audioSource = gameObject.AddComponent<AudioSource>();

        autoAttackButton = GameObject.Find("AutoAttackButton");
        autoAttackButton.GetComponent<Button>().onClick.AddListener(AutoAttackButtonClicked);
        InitButtonHoverEvent(autoAttackButton);

        backgroundObject = GameObject.Find("Background");
        backgroundObject.GetComponent<SpriteRenderer>().sprite = blueBackground;

        basicTurretTextMesh = GameObject.Find("BasicTurretText").gameObject.GetComponent<TextMeshProUGUI>();

        buildTurretCostTextMesh = GameObject.Find("BuildTurretCostText").gameObject.GetComponent<TextMeshProUGUI>();
        buildTurretCostTextMesh.text = $"Cost ${GameManager.Instance.TurretCosts[0]}";

        enemiesRemainingTextMesh = GameObject.Find("EnemiesRemainingText").gameObject.GetComponent<TextMeshProUGUI>();
        goldTextMesh = GameObject.Find("GoldText").gameObject.gameObject.GetComponent<TextMeshProUGUI>();

        hoverClip = Resources.Load("Click") as AudioClip;

        menuButton = GameObject.Find("MenuButton").gameObject;
        menuButton.GetComponent<Button>().onClick.AddListener(MenuButtonClicked);
        InitButtonHoverEvent(menuButton);

        menuPanel = GameObject.Find("MenuPanel");

        regularTurretTextMesh = GameObject.Find("RegularTurretText").GetComponent<TextMeshProUGUI>();

        turretTextParent = GameObject.Find("TurretTextParent");
        turretTextList = new List<GameObject>();
        foreach (var turretText in turretTextParent.GetComponentsInChildren<Transform>())
        {
            if(new []{ "BasicTurretText", "RegularTurretText", "AdvancedTurretText" }.Contains(turretText.gameObject.name))
            {
                turretTextList.Add(turretText.gameObject);
                turretText.gameObject.SetActive(false);
            }
        }

        waveNumberTextMesh = GameObject.Find("WaveNumberText").gameObject.GetComponent<TextMeshProUGUI>();

        // Only after Canvas has initialized successfully allow the first wave to spawn
        GameManager.Instance.UpdateGameState(GameState.Init);

        // NOW set things inactive
        menuPanel.SetActive(false);
    }
    #endregion

    #region Observables
    public void AutoAttackChanged(bool autoAttack)
    {
        autoAttackButton.GetComponent<Button>().interactable = false;
    }
    
    public void EnemyCountChanged(int newCount) 
    {
        enemiesRemainingTextMesh.text = $"Enemies Remaining {newCount}";
        // StartCoroutine(GrowTextAndFadeBack(enemiesRemainingTextMesh));
    }

    public void NewTurretBuilt(int indexOfNewTurret)
    {
        turretTextList[indexOfNewTurret].SetActive(true);
        if (indexOfNewTurret <= 1)
        {
            buildTurretCostTextMesh.text = $"Cost ${GameManager.Instance.TurretCosts[indexOfNewTurret + 1]}";
        }
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

        if (newWaveNumber % 3 == 0)
        {
            backgroundObject.GetComponent<SpriteRenderer>().sprite = greenBackground;
        }
        else if (newWaveNumber % 5 == 0)
        {
            backgroundObject.GetComponent<SpriteRenderer>().sprite = purpleBackground;
        }
        else
        {
            backgroundObject.GetComponent<SpriteRenderer>().sprite = blueBackground;
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

    private void AutoAttackButtonClicked()
    {
        GameManager.Instance.PurchaseAutoAttack();
    }

    private void MenuButtonClicked()
    {
        menuPanel.SetActive(!menuPanel.activeSelf);
    }

    private void HoverMouseNoise(UnityEngine.EventSystems.BaseEventData baseEvent)
    {
        audioSource.PlayOneShot(hoverClip);
    }
    #endregion

    #region Turrets
    public void BuildTurret()
    {
        GameManager.Instance.BuildTurret();
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
