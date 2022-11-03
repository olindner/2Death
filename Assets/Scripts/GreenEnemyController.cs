using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenEnemyController : MonoBehaviour
{
    public float Speed = 0.3f;
    private GameObject targetPoint;
    private int goldWorth = 5;

    void Start()
    {
        targetPoint = GameObject.Find("TargetPoint");
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.transform.position, Speed * Time.deltaTime);
    }

    void OnMouseDown()
    {
        CanvasController.GlobalTarget = gameObject;
    }

    void OnTriggerEnter2D (Collider2D coll)
    {
        Destroy(coll.gameObject);
        Die();
    }

    void Die()
    {
        CanvasController.AddGold(goldWorth);
        CanvasController.ResetTarget();
        Destroy(gameObject);
    }
}
