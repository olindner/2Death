using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed = 3f;
    public float Damage = 20f;

    void Update()
    {
        if (CanvasController.GlobalTarget == null) 
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, CanvasController.GlobalTarget.transform.GetComponent<Renderer>().bounds.center, Speed * Time.deltaTime);
    }
}
