using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHitDetector : MonoBehaviour
{
    LayerMask carLayer;
    Ragdoll ragdoll;
    private void Awake()
    {
        ragdoll = GetComponentInParent<Ragdoll>();
        carLayer = LayerMask.GetMask("Car");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (carLayer.Contains(collision.collider.gameObject.layer))
        {
            ragdoll.EnableRagdoll();
        }
    }
}
