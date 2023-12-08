using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Engine : MonoBehaviour
{
    [SerializeField] WheelCollider[] frontWheels;
    [SerializeField] WheelCollider[] backWheels;
    [SerializeField] float steerAngle = 0;
    [SerializeField] float frontWheelTorque = 1000;
    [SerializeField] float backWheelTorque = 1000;
    bool gasInput = false;
    bool brakeInput = false;
    bool driftInput = false;
    float steerInput = 0;

    public void OnGas(InputAction.CallbackContext context)
    {
        if (context.started)
            gasInput = true;

        else if (context.canceled)
            gasInput = false;
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        if (context.started)
            brakeInput = true;

        else if (context.canceled)
            brakeInput = false;
    }

    public void OnSteer(InputAction.CallbackContext context)
    {
        if (context.performed)
            steerAngle = context.ReadValue<float>();

        else if (context.canceled)
            steerAngle = 0;
    }
    public void OnDrift(InputAction.CallbackContext context)
    {
        if (context.started)
            driftInput = true;

        else if (context.canceled)
            driftInput = false;
    }
    // Update is called once per frame
    void Update()
    {
        float motorTorque;
        if (brakeInput == gasInput)
            motorTorque = 0;
        else if (brakeInput)
            motorTorque = -5000;
        else
            motorTorque = 5000;
        foreach (var wheel in backWheels)
            wheel.motorTorque = motorTorque;

        foreach(var wheel in frontWheels)
            wheel.steerAngle = steerInput * 40;
    }
}
