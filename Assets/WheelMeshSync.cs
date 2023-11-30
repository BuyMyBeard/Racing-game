using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMeshSync : MonoBehaviour
{
    WheelCollider wheelCollider;
    // Start is called before the first frame update
    void Awake()
    {
        wheelCollider = transform.parent.parent.GetChildByName("Colliders").GetChildByName(name).GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(wheelCollider.rpm * 360 / 60 * Time.deltaTime, 0, 0);
    }
}
