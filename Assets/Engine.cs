using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [SerializeField] WheelCollider[] frontWheels;
    [SerializeField] WheelCollider[] backWheels;
    // Start is called before the first frame update
    void Awake()
    {
        foreach(var wheel in frontWheels)
        {
            wheel.steerAngle = 45;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var wheel in frontWheels)
        {
            wheel.motorTorque = 50;
        }
        foreach(var wheel in backWheels)
        {
            wheel.motorTorque = 50;
        }
    }
}
