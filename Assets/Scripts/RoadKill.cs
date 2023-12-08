using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadKill : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var hits = rb.SweepTestAll(rb.velocity, rb.velocity.magnitude * Time.fixedDeltaTime, QueryTriggerInteraction.Collide);
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out Ragdoll ragdoll))
            {
                ragdoll.EnableRagdoll();
            }
        }
    }
}
