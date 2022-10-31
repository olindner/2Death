using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed = 1f;

    void Update()
    {
        if (CanvasController.GlobalTarget == null) return;
        
        Debug.Log("Proj updating");
        transform.position = Vector3.Lerp(transform.position, CanvasController.GlobalTarget.transform.position, Speed * Time.deltaTime);
    }
}
