using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    private void Start()
    {
        AllEnemies = new List<GameObject>();
        AllTurrets = new List<GameObject>();
    }
    #endregion

    [SerializeField] UnityEvent TotalGoldChanged;
    [SerializeField] UnityEvent AutoAttackChanged;

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
}
