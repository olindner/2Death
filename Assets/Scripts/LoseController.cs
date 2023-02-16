using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseController : MonoBehaviour
{
    public void TryAgain()
    {
        GameManager.Instance.StartGame();
    }
}
