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
    [SerializeField] GameObject MenuPanel;

    private AudioClip hoverClip;
    private AudioSource audioSource;

    private GameObject autoButton;
    private GameObject backgroundObject;
    private GameObject goldText;
    private GameObject menuButton;
    private GameObject waveNumberText;

    private Sprite blueBackground;
    private Sprite greenBackground;
    private Sprite purpleBackground;
    
    private Color32 EnableColor = new Color32(195, 225, 165, 255);
    private Color32 DisableColor = new Color32(225, 165, 165, 255);
    
    private void Awake()
    {
        GameManager.Instance.TotalGoldChanged += TotalGoldChanged;
        GameManager.Instance.AutoAttackChanged += AutoAttackChanged;
        GameManager.Instance.WaveNumberChanged += WaveNumberChanged;

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

        waveNumberText = gameObject.transform.Find("WaveNumberText").gameObject;
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

    public void WaveNumberChanged(int newWaveNumber) 
    {
        waveNumberText.GetComponent<TextMeshProUGUI>().text = $"Wave {newWaveNumber}";

        if (newWaveNumber == 3)
        {
            backgroundObject.GetComponent<SpriteRenderer>().sprite = greenBackground;
        }
        if (newWaveNumber == 5)
        {
            backgroundObject.GetComponent<SpriteRenderer>().sprite = purpleBackground;
        }
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

    private void HoverMouseNoise(UnityEngine.EventSystems.BaseEventData baseEvent)
    {
        audioSource.PlayOneShot(hoverClip);
    }
}
