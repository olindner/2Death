using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    private static bool applicationIsQuitting = false;

    public static GameManager Instance
    {
        // Could add a lock if needed
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }
 
            return _instance;
        }
    }
 
    private void Awake()
    {
        if (_instance) 
        {
            Destroy(gameObject);
        }
        else 
        {
            _instance = this;
        }
        
        DontDestroyOnLoad(this);
    }

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
    #endregion

    #region Observables
    public event Action<bool> AutoAttackChanged;
    public event Action<int> EnemyCountChanged;
    public event Action<int> NewTurretBuilt;
    public event Action<int> TotalGoldChanged;
    public event Action<int> WallHealthChanged;
    public event Action<int> WaveNumberChanged;
    #endregion

    #region Buttons
    // This is binded to the "Start Game" button in the Menu, Lose, and Win scenes
    public void StartGame()
    {
        UpdateGameState(GameState.MainGame);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    #region GameState

    public enum GameState
    {
        Title,
        MainGame,
        Init,
        SpawnWave,
        GamePlay,
        Win,
        Lose
    }

    private GameState state;
    public GameState State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
        }
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Title:
                var titleClip = Resources.Load("TitleScreen") as AudioClip;
                AudioSourcer.clip = titleClip;
                AudioSourcer.Play();

                break;
            case GameState.MainGame:
                SceneManager.LoadScene("GameScene");

                var gameClip = Resources.Load("NormalGame") as AudioClip;
                AudioSourcer.clip = gameClip;
                AudioSourcer.Play();

                break;
            case GameState.Init:
                TotalGold = 0;
                WallHealth = 100;
                WaveNumber = 0;

                InitTurrets();

                break;
            case GameState.SpawnWave:
                Debug.Log("SpawnWave called");
                StartCoroutine(SpawnWaveDriver());
                break;
            case GameState.GamePlay:
                // Not sure if any logic needed here
                break;
            case GameState.Win:
                SceneManager.LoadScene("WinScene");
                
                var winClip = Resources.Load("WinMusic") as AudioClip;
                AudioSourcer.clip = winClip;
                AudioSourcer.time = 0.5f;
                AudioSourcer.Play();

                break;
            case GameState.Lose:
                SceneManager.LoadScene("LoseScene");

                var loseMusic = Resources.Load("LoseMusic") as AudioClip;
                AudioSourcer.clip = loseMusic;
                AudioSourcer.time = 0.5f;
                AudioSourcer.Play();
                break;
            default:
                break;
        }
    }
    #endregion

    #region Audio

    private AudioClip HoverClip;
    private AudioSource audioSourcer;
    public AudioSource AudioSourcer 
    {
        get
        {
            return audioSourcer;
        }
        set 
        {
            audioSourcer = value;
        }
    }

    public void HoverMouseNoise()
    {
        AudioSourcer.PlayOneShot(HoverClip);
    }
    #endregion
    
    #region TotalGold
    private int totalGold;
    public int TotalGold 
    {
        get
        {
            return totalGold;
        }
        set 
        {
            totalGold = value;
        }
    }

    public void ChangeTotalGoldBy(int goldDelta)
    {
        TotalGold += goldDelta;
        TotalGoldChanged?.Invoke(TotalGold);

        if (TotalGold >= 250)
        {
            UpdateGameState(GameState.Win);
        }
    }
    #endregion

    #region WallHealth
    private int wallHealth;
    public int WallHealth 
    {
        get
        {
            return wallHealth;
        }
        set 
        {
            wallHealth = value;
        }
    }

    public void ChangeWallHealthBy(int healthDelta)
    {
        WallHealth += healthDelta;
        WallHealthChanged?.Invoke(WallHealth);

        if (WallHealth <= 0)
        {
            UpdateGameState(GameState.Lose);
        }
    }
    #endregion

    #region ClickDamage
    private float clickDamage = 10;
    public float ClickDamage 
    {
        get
        {
            return clickDamage;
        }
        set 
        {
            clickDamage = value;
        }
    }
    #endregion

    #region Waves and Enemies
    public readonly List<int> EnemiesPerWave = new List<int>{ 0, 5, 10, 15, 20, 25 };
    private int waveNumber;
    public int WaveNumber 
    {
        get
        {
            return waveNumber;
        }
        set 
        {
            waveNumber = value;
            TotalEnemiesThisWave = EnemiesPerWave[waveNumber];
            EnemiesLeftThisWave = TotalEnemiesThisWave;
            WaveNumberChanged?.Invoke(waveNumber);
        }
    }
    private int totalEnemiesThisWave;
    public int TotalEnemiesThisWave
    {
        get
        {
            return totalEnemiesThisWave;
        }
        set 
        {
            totalEnemiesThisWave = value;
        }
    }
    private int enemiesLeftThisWave;
    public int EnemiesLeftThisWave
    {
        get
        {
            return enemiesLeftThisWave;
        }
        set 
        {
            enemiesLeftThisWave = value;
            EnemyCountChanged?.Invoke(enemiesLeftThisWave);

            if (enemiesLeftThisWave == 0 && State == GameState.GamePlay)
            {
                // Could trigger a wait screen to prepare for next wave
                UpdateGameState(GameState.SpawnWave);
            }
        }
    }

    private GameObject GreenEnemy;

    private IEnumerator SpawnWaveDriver()
    {
        WaveNumber++;

        if (WaveNumber >= EnemiesPerWave.Count - 1)
        {
            UpdateGameState(GameState.Win);
            yield return null;
        }

        // This short delay is for displaying title
        yield return new WaitForSeconds(1);

        int numberOfEnemiesToSpawn = EnemiesPerWave[WaveNumber];

        for (var i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            SpawnEnemy();

            // Final enemy spawn should not delay game logic
            if (i != numberOfEnemiesToSpawn)
            {
                yield return new WaitForSeconds(5);
            }
        }

        UpdateGameState(GameState.GamePlay);

        yield return null;
    }

    private void SpawnEnemy()
    {
        var enemy = Instantiate(GreenEnemy, new Vector2(-11, -2), Quaternion.identity);
        AddEnemy(enemy);
    }

    private List<GameObject> allEnemies;
    public List<GameObject> AllEnemies
    {
        get
        {
            allEnemies.RemoveAll(enemy => enemy == null);
            return allEnemies; // Could return AsReadOnly()
        }
        set
        {
            allEnemies = value;
        }
    }

    private void AddEnemy(GameObject newEnemy)
    {
        AllEnemies.Add(newEnemy);
    }
    #endregion

    #region Turrets
    private List<GameObject> allTurrets;
    public List<GameObject> AllTurrets
    {
        get
        {
            return allTurrets;
        }
        set
        {
            allTurrets = value;
        }
    }

    public List<int> TurretCosts = new List<int>{ 10, 50, 100 };
    private GameObject TurretParentGameObject;

    private void InitTurrets()
    {
        AllTurrets = new List<GameObject>();
        TurretParentGameObject = GameObject.Find("TurretParent");
        foreach (var turret in TurretParentGameObject.GetComponentsInChildren<Transform>())
        {
            if(new []{ "BasicTurret", "RegularTurret", "AdvancedTurret" }.Contains(turret.gameObject.name))
            {
                AllTurrets.Add(turret.gameObject);
                turret.gameObject.SetActive(false);
            }
        }
        UpdateGameState(GameState.SpawnWave);
    }

    public void BuildTurret()
    {
        var numberOfActiveTurrets = AllTurrets.Where(turret => turret.activeSelf).Count();
        // If all are active already, can't activate anymore
        if (numberOfActiveTurrets == AllTurrets.Count) return;

        if (!SuccessfullyPaidForTurret(numberOfActiveTurrets)) return;

        AllTurrets[numberOfActiveTurrets].SetActive(true);

        NewTurretBuilt?.Invoke(numberOfActiveTurrets);
    }

    private bool SuccessfullyPaidForTurret(int numberOfActiveTurrets)
    {
        var costOfNextTurret = TurretCosts[numberOfActiveTurrets];
        if (TotalGold < costOfNextTurret) return false;

        ChangeTotalGoldBy(-costOfNextTurret);
        return true;
    }
    #endregion

    #region AutoAttack
    private int costOfAutoAttack = 100;
    private bool autoAttack = false;
    public bool AutoAttack
    {
        get
        {
            return autoAttack;
        }
        set
        {
            autoAttack = value;
            AutoAttackChanged?.Invoke(autoAttack);
        }
    }

    public void PurchaseAutoAttack()
    {
        if (TotalGold < costOfAutoAttack || AutoAttack == true) return;

        ChangeTotalGoldBy(-costOfAutoAttack);
        AutoAttack = true;
    }
    #endregion

    #region Built In Functions
    private void Start()
    {
        AllEnemies = new List<GameObject>();
        AudioSourcer = gameObject.AddComponent<AudioSource>();
        GreenEnemy = Resources.Load("GreenEnemy") as GameObject;
        HoverClip = Resources.Load("Click") as AudioClip;

        UpdateGameState(GameState.Title);
    }
    #endregion

    
}