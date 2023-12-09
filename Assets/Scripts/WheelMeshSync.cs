using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMeshSync : MonoBehaviour
{
    WheelCollider wheelCollider;
    float prevSteeringAngle = 0;

    void Awake()
    {
        wheelCollider = transform.parent.parent.GetChildByName("Colliders").GetChildByName(name).GetComponent<WheelCollider>();
    }

    void Update()
    {
        transform.Rotate(wheelCollider.rpm * 360 / 60 * Time.deltaTime, 0, 0);
        transform.RotateAround(transform.position, transform.parent.up ,wheelCollider.steerAngle - prevSteeringAngle);
        prevSteeringAngle = wheelCollider.steerAngle;
    }
}
