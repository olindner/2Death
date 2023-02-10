using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

    public event Action<bool> AutoAttackChanged;
    public event Action<int> EnemyCountChanged;
    public event Action<int> TotalGoldChanged;
    public event Action<int> WaveNumberChanged;

    #region GameState

    public enum GameState
    {
        Title,
        MainGame,
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
            case GameState.SpawnWave:
                //Start spawning coroutine, then in coroutine will eventually trigger new state
                StartCoroutine(SpawnWaveDriver());
                break;
            case GameState.GamePlay:
                // Not sure if any logic needed here
                break;
            case GameState.Win:
                SceneManager.LoadScene("WinScene");
                // Play win music
                break;
            case GameState.Lose:
                SceneManager.LoadScene("LoseScene");
                break;
            default:
                break;
        }
    }
    #endregion

    #region Audio

    private AudioClip hoverClip;
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
        AudioSourcer.PlayOneShot(hoverClip);
    }
    #endregion

    #region Wave
    private List<int> EnemiesPerWave = new List<int>{ 0, 5, 10, 15, 20, 25 };
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
            WaveNumberChanged?.Invoke(waveNumber);
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
        else
        {
            UpdateGameState(GameState.GamePlay);
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

        yield return null;
    }

    private void SpawnEnemy()
    {
        var enemy = Instantiate(GreenEnemy, new Vector2(-11, -2), Quaternion.identity);
        AddEnemy(enemy);
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

    #region AllEnemies
    private List<GameObject> allEnemies;
    public List<GameObject> AllEnemies
    {
        get
        {
            allEnemies.RemoveAll(enemy => enemy == null);
            StartCoroutine(UpdateEnemyCount());
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

    public void EnemyDied()
    {
        StartCoroutine(UpdateEnemyCount());

        if (AllEnemies.Count <= 1)
        {
            //TODO: Check for win condition

            // Could trigger a wait screen to prepare for next wave
            UpdateGameState(GameState.SpawnWave);
        }
    }

    private IEnumerator UpdateEnemyCount()
    {
        // Hack for allowing enemy to die before updating count
        yield return new WaitForSeconds(0.1f);

        allEnemies.RemoveAll(enemy => enemy == null);
        EnemyCountChanged?.Invoke(allEnemies.Count);
    }
    #endregion

    #region AllTurrets
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

    public void AddTurret(GameObject newTurret)
    {
        AllTurrets.Add(newTurret);
    }
    #endregion

    #region AutoAttack
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

    public void ToggleAutoAttack()
    {
        AutoAttack = !AutoAttack;
    }
    #endregion

    private void Start()
    {
        AllEnemies = new List<GameObject>();
        AllTurrets = new List<GameObject>();
        AudioSourcer = gameObject.AddComponent<AudioSource>();
        hoverClip = Resources.Load("Click") as AudioClip;
        GreenEnemy = Resources.Load("GreenEnemy") as GameObject;

        UpdateGameState(GameState.Title);
    }

    // This is binded to the "Start Game" button in the Menu scene
    public void StartGame()
    {
        UpdateGameState(GameState.MainGame);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
