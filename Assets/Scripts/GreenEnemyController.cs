using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GreenEnemyController : MonoBehaviour
{
    public float Speed = 0.3f;
    public GameObject HealthText;

    private GameObject targetPoint;
    private float health = 100f;
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

    // Hit with Projectile
    void OnTriggerEnter2D (Collider2D coll)
    {
        var damage = coll.gameObject.GetComponent<Projectile>().Damage;
        health -= damage;

        if (health <= 0f)
        {
            Die();
        }
        else
        {
            HealthText.GetComponent<TextMeshPro>().text = health.ToString();
        }

        Destroy(coll.gameObject);
    }

    void Die()
    {
        CanvasController.AddGold(goldWorth);
        CanvasController.ResetTarget();
        Destroy(gameObject);
    }
}
