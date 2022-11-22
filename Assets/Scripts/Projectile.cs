using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    enum DamageClass
    {
        Green, 
        Yellow, 
        Red
    };

    [SerializeField] DamageClass damageClass;
    [HideInInspector] public float Damage;
    private float Speed = 3f;
    private Transform targetTransform = null;

    void Start()
    {
        switch (damageClass)
        {
            case(DamageClass.Green):
                Damage = 5;
                break;
            case(DamageClass.Yellow):
                Damage = 10;
                break;
            case(DamageClass.Red):
                Damage = 20;
                break;
            default:
                Damage = 1;
                break;
        }
    }

    void Update()
    {
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetTransform.GetComponent<Renderer>().bounds.center, Speed * Time.deltaTime);
    }

    public void SetTargetTransform(Transform transformToTarget)
    {
        targetTransform = transformToTarget;
    }
}
