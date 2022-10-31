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

    void OnMouseDown()
    {
        CanvasController.GlobalTarget = gameObject;
        // var activeProjectiles = GameObject.FindGameObjectsWithTag("projectile");
        // foreach (var ap in activeProjectiles)
        // {
        //     ap.GetComponent<Projectile>().ShootTowards(transform.position);
        // }
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint.transform.position, Speed * Time.deltaTime);
    }
}
