using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [SerializeField] WheelCollider[] frontWheels;
    [SerializeField] WheelCollider[] backWheels;
    [SerializeField] float steerAngle = 0;
    [SerializeField] float frontWheelTorque = 1000;
    [SerializeField] float backWheelTorque = 1000;
     // Start is called before the first frame update
    void Awake()
    {
        foreach(var wheel in frontWheels)
        {
             wheel.steerAngle = steerAngle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var wheel in frontWheels)
        {
            wheel.motorTorque = frontWheelTorque;
        }
        foreach(var wheel in backWheels)
        {
            wheel.motorTorque = backWheelTorque;
        }
    }

    private void OnValidate()
    {
        foreach (var wheel in frontWheels)
        {
            wheel.steerAngle = steerAngle;
        }
    }
}
