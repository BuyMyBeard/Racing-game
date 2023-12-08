using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    Collider[] colliders;
    Animator animator;
    NavMeshAgent agent;
    public bool IsRagdolling { get; private set; } = false;

    private void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            // collider.isTrigger = true;
            collider.AddComponent<CarHitDetector>();
        }

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    [ContextMenu("Ragdoll")]
    public void EnableRagdoll()
    {
        if (IsRagdolling) return;
        IsRagdolling = true;
        animator.enabled = false;
        agent.enabled = false;
        foreach (var rigidbody in rigidbodies)
            rigidbody.isKinematic = false;

        foreach (var collider in colliders)
            collider.isTrigger = false;
    }
    [ContextMenu("Reset")]
    public void ResetRagdoll()
    {
        if (!IsRagdolling) return;
        IsRagdolling = false;
        animator.enabled = true;
        agent.enabled = true;
        foreach (var rigidbody in rigidbodies)
            rigidbody.isKinematic = true;

        foreach (var collider in colliders)
            collider.isTrigger = true;
    }
}
