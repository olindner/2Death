using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
 
    void Awake()
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

    public int TotalGold {get; set; }
}
