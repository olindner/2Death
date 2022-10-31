using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffController : MonoBehaviour
{
    public GameObject Projectile;

    private float spawnTimer = 2f;
 
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        
        if (spawnTimer <= 0.0f)
        {
            SpawnProjectile();
            spawnTimer = 2f;
        }
    }
    
    void SpawnProjectile()
    {
        if (CanvasController.GlobalTarget == null) return;

        Instantiate(Projectile, transform.position, Quaternion.identity);
    }
}
