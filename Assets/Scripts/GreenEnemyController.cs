using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameState = GameManager.GameState;

public class GreenEnemyController : MonoBehaviour
{
    [SerializeField] GameObject HealthText;

    private IEnumerator currentCoroutine = null;
    private GameObject targetPoint;
    private SpriteRenderer spriteRenderer;
    private Color dimmedColor = new Color(1, 1, 1, 0.8f);
    private Color highlightedColor = new Color(1, 1, 1, 1);
    private Color targetColor;
    private float health = 100f;
    private float damageFadeSpeed = 1f;
    private float speed = 0.3f;
    private int goldWorth = 5;
    private byte highlightedAlpha = 255;
    private byte dimmedAlpha = 200;

    void Start()
    {
        targetPoint = GameObject.Find("TargetPoint");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        targetColor = spriteRenderer.color;
        Dim(spriteRenderer);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.transform.position, speed * Time.deltaTime);
    }

    void OnMouseEnter()
    {
        Highlight(spriteRenderer);
    }

    void OnMouseDown()
    {
        foreach (var turret in GameManager.Instance.AllTurrets)
        {
            turret.GetComponent<StaffController>().SetTargetManually(gameObject.transform);
        }

        TakeDamage(GameManager.Instance.ClickDamage);
    }

    void OnMouseExit()
    {
        Dim(spriteRenderer);
    }

    // Hit with Projectile
    void OnTriggerEnter2D (Collider2D coll)
    {
        var damage = coll.gameObject.GetComponent<Projectile>().Damage;
        
        Destroy(coll.gameObject);

        TakeDamage(damage);
    }

    void TakeDamage(float damage)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        Redify(spriteRenderer);
        
        currentCoroutine = TargetColorFade(spriteRenderer, damageFadeSpeed);
        StartCoroutine(currentCoroutine);

        health -= damage;

        if (health <= 0f)
        {
            Die();
        }
        else
        {
            HealthText.GetComponent<TextMeshPro>().text = health.ToString();
        }
    }

    // Fades to targetColor over time
    IEnumerator TargetColorFade(SpriteRenderer sr, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            sr.color = Color.Lerp(sr.color, targetColor , time / (duration * highlightedAlpha));
            time += Time.deltaTime;
            yield return null;
        }

        sr.color = targetColor;
    }

    void Highlight(SpriteRenderer sr)
    {
        Color32 colorVar = sr.color;
        colorVar.a = highlightedAlpha;
        sr.color = colorVar;

        // When mouse over, adjust target color alpha to highlighted
        targetColor = highlightedColor;
    }

    void Dim(SpriteRenderer sr)
    {
        Color32 colorVar = sr.color;
        colorVar.a = dimmedAlpha;
        sr.color = colorVar;

        // When mouse away, adjust target color alpha to dimmed
        targetColor = dimmedColor;
    }

    void Redify(SpriteRenderer sr)
    {
        Color32 colorVar = sr.color;
        colorVar.r = 255;
        colorVar.g = 0;
        colorVar.b = 0;
        sr.color = colorVar;
    }

    void Die()
    {
        GameManager.Instance.ChangeTotalGoldBy(goldWorth);

        GameManager.Instance.EnemyDied();

        Destroy(gameObject);
    }
}
