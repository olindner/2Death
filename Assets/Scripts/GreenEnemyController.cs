using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenEnemyController : MonoBehaviour
{
    public float Speed = 0.1f;
    private GameObject targetPoint;
    private int goldWorth = 5;

    void Start()
    {
        targetPoint = GameObject.Find("TargetPoint");
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint.transform.position, Speed * Time.deltaTime);
    }

    void OnMouseDown()
    {
        CanvasController.GlobalTarget = gameObject;
    }

    void OnTriggerEnter2D (Collider2D coll)
    {
        Debug.Log("Collided Green");
        //Switch statement on enemy attack?
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
