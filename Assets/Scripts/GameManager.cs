using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
 
    public static GameManager Instance
    {
        // Could add a lock if needed
        get
        {
            if(_instance == null)
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
        else {
            _instance = this;
        }
        
        DontDestroyOnLoad(this);
    }
    #endregion

    [SerializeField] UnityEvent TotalGoldChanged;
    [SerializeField] UnityEvent AutoAttackChanged;

    #region GameState

    public enum GameState
    {
        Title,
        MainGame,
        SpawnWave,
        Gameplay,
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
                break;
            case GameState.MainGame:
                SceneManager.LoadScene("GameScene");
                break;
            case GameState.SpawnWave:
                break;
            case GameState.Gameplay:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                break;
        }
    }
    #endregion
    
    #region TotalGold
    private int totalGold = 0;
    public int TotalGold 
    {
        get
        {
            return totalGold;
        }
        set 
        {
            totalGold = value;
            TotalGoldChanged?.Invoke();
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

    #region AllEnemies
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

    public void AddEnemy(GameObject newEnemy)
    {
        AllEnemies.Add(newEnemy);
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

    public void AddTurret(GameObject newTurret)
    {
        AllTurrets.Add(newTurret);
    }
    #endregion

    #region AutoAttack
    private bool autoAttack;
    public bool AutoAttack
    {
        get
        {
            return autoAttack;
        }
        set
        {
            autoAttack = !autoAttack;
            AutoAttackChanged?.Invoke();
        }
    }
    #endregion

    private void Start()
    {
        AllEnemies = new List<GameObject>();
        AllTurrets = new List<GameObject>();

        UpdateGameState(GameState.Title);
    }

    public void StartGame()
    {
        UpdateGameState(GameState.MainGame);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
