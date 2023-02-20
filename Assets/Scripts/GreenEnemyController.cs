using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameState = GameManager.GameState;

public class GreenEnemyController : MonoBehaviour
{
    #region Variable References
    [SerializeField] GameObject HealthText;

    private IEnumerator currentCoroutine = null;
    private GameObject targetPoint;
    private SpriteRenderer spriteRenderer;
    private Color dimmedColor = new Color(1, 1, 1, 0.8f);
    private Color highlightedColor = new Color(1, 1, 1, 1);
    private Color targetColor;
    private float health = 100f;
    private float damageFadeSpeed = 1f;
    private float speed = 1.2f;
    private int damageValue = 5;
    private int goldWorth = 5;
    private byte highlightedAlpha = 255;
    private byte dimmedAlpha = 200;
    #endregion

    #region Built In Functions
    void Start()
    {
        targetPoint = GameObject.Find("TargetPoint");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        targetColor = spriteRenderer.color;
        Dim(spriteRenderer);
    }

    void Update()
    {
        if (targetPoint != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.transform.position, speed * Time.deltaTime);
        }
    }
    #endregion

    #region Mouse Functions
    void OnMouseEnter()
    {
        Highlight(spriteRenderer);
    }

    void OnMouseDown()
    {
        foreach (var turret in GameManager.Instance.AllTurrets)
        {
            if (turret.activeSelf)
            {
                turret.GetComponent<StaffController>().SetTargetManually(gameObject.transform);
            }
        }

        TakeDamage(GameManager.Instance.ClickDamage);
    }

    void OnMouseExit()
    {
        Dim(spriteRenderer);
    }
    #endregion

    #region Collisions, Damage, and Death
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "projectile")
        {
            var damage = coll.gameObject.GetComponent<Projectile>().Damage;
            Destroy(coll.gameObject);
            TakeDamage(damage);
        }
        else
        {
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name == "TowerWall")
        {
            StartCoroutine(DamageTowerWallLoop());
        }
        else
        {
            return;
        }
    }

    private void TakeDamage(float damage)
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

    private void Die()
    {
        GameManager.Instance.ChangeTotalGoldBy(goldWorth);

        GameManager.Instance.EnemiesLeftThisWave--;

        Destroy(gameObject);
    }

    private IEnumerator DamageTowerWallLoop()
    {
        while(true)
        {
            GameManager.Instance.ChangeWallHealthBy(-damageValue);

            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #region Color Change Helpers
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
    #endregion
}
