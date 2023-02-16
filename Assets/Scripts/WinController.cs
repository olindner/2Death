using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinController : MonoBehaviour
{
    public void PlayAgain()
    {
        GameManager.Instance.StartGame();
    }
}
