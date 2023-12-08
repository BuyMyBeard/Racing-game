using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        Collider[] tempColliders = GetComponentsInChildren<Collider>();
        colliders = tempColliders.Skip(1).ToArray();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
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
            collider.enabled = true;
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
            collider.enabled = false;
    }
}
