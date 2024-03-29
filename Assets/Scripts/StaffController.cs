using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StaffController : MonoBehaviour
{
    #region Variable References
    [SerializeField] GameObject Projectile;
    private float spawnTimer = 2f;
    private AudioClip BallShotClip;
    private AudioSource audioSource;
    private Transform targetTransform = null;
    #endregion
 
    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        BallShotClip = Resources.Load("BallShot2") as AudioClip;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        
        if (spawnTimer <= 0.0f )
        {
            if (GameManager.Instance.AllEnemies.Count > 0)
            {
                if (targetTransform == null && GameManager.Instance.AutoAttack)
                {
                    FindClosestEnemy();
                }
                if (targetTransform != null)
                {
                    SpawnProjectile();
                }
            }

            spawnTimer = 2f;
        }
    }
    
    private void SpawnProjectile()
    {
        if (targetTransform == null) {
            return;
        }

        var projectile = Instantiate(Projectile, transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetTargetTransform(targetTransform);
        audioSource.PlayOneShot(BallShotClip, 0.7F);
    }

    // Could eventually optimize to cache, queue, or k-nearest neighbors
    private void FindClosestEnemy()
    {
        targetTransform = GameManager.Instance.AllEnemies.Where(n => n)
            .OrderBy(n => (n.transform.position - transform.position).sqrMagnitude)
            .FirstOrDefault().transform;
    }

    public void SetTargetManually(Transform transformToTarget)
    {
        targetTransform = transformToTarget;
    }
}
