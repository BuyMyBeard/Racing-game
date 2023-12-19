using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] Transform respawnPosition;
    [SerializeField] Mesh arrow;
    CheckpointManager checkpointManager;
    public Transform RespawnPosition => respawnPosition;
    private void Awake()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
        if (checkpointManager == null) Debug.Log("Checkpoint Manager is missing");
    }

    private void OnTriggerEnter(Collider other)
    {
        checkpointManager.PassCheckpoint(this);
    }

    private void OnDrawGizmos()
    {
        if (respawnPosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawMesh(arrow, respawnPosition.position, respawnPosition.rotation * Quaternion.Euler(90, 90, 0), new Vector3(100, 100, 100));
        }
    }
}
