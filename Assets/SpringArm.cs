using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArm : MonoBehaviour
{
    [SerializeField] Vector3 offset = new(0, 0, -6);
    [SerializeField] float cameraRadius = .02f;
    [SerializeField] LayerMask collideAgainst;

    private void Update()
    {
        
        if (Physics.SphereCast(transform.parent.position, cameraRadius, (transform.position - transform.parent­.position).normalized, out RaycastHit hit, offset.magnitude, collideAgainst))
        {
            transform.localPosition = offset.normalized * (hit.distance - cameraRadius);
            Debug.Log("hit " + hit.distance);
        }
        else
            transform.localPosition = offset;
    }
}
