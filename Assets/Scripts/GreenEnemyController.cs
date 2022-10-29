using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenEnemyController : MonoBehaviour
{
    public float Speed = 0.5f;
    private GameObject targetPoint;
    private int goldWorth = 5;

    void Start()
    {
        targetPoint = GameObject.Find("TargetPoint");
    }

    void OnMouseDown()
    {
        Debug.Log("Killed");

        CanvasController.AddGold(goldWorth);

        Destroy(gameObject);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint.transform.position, Speed * Time.deltaTime);
    }
}
