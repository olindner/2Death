using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GreenEnemyController : MonoBehaviour
{
    private float Speed = 0.3f;
    [SerializeField] GameObject HealthText;

    private GameObject targetPoint;
    private SpriteRenderer spriteRenderer;
    private Color32 DimColor = new Color32(255, 255, 255, 225);
    private Color32 BrightColor = new Color32(255, 255, 255, 255);
    private float health = 100f;
    private int goldWorth = 5;

    void Start()
    {
        targetPoint = GameObject.Find("TargetPoint");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = DimColor; //Start with dim so that mouse over will "highlight"
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.transform.position, Speed * Time.deltaTime);
    }

    void OnMouseDown()
    {
        foreach (var turret in GameManager.Instance.AllTurrets)
        {
            turret.GetComponent<StaffController>().SetTargetManually(gameObject.transform);
        }
    }

    void OnMouseOver()
    {
        spriteRenderer.color = BrightColor;
    }

    void OnMouseExit()
    {
        spriteRenderer.color = DimColor;
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
        Debug.Log("Died!");
        GameManager.Instance.TotalGold += goldWorth;
        Destroy(gameObject);
    }
}
