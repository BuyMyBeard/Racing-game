using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Engine : MonoBehaviour
{
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider backLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] TextMeshProUGUI speedometer;
    [SerializeField] float maxSpeed = 67.3f;
    [SerializeField] float drag = 10;
    /// <summary>
    /// Current forward speed in m / s
    /// </summary>
    float currentSpeed;
    bool gasInput = false;
    bool brakeInput = false;
    bool driftInput = false;
    float steerInput = 0;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void UpdateSpeedometer() => speedometer.SetText($"{ Mathf.Round(currentSpeed /  1000 * 3600)} km/h");
    void Brake(float value)
    {
        backLeft.brakeTorque = value;
        backRight.brakeTorque = value;
        frontLeft.brakeTorque = value;
        frontRight.brakeTorque = value;
    }

    void Motor(float value)
    {
        backLeft.motorTorque = value;
        backRight.motorTorque = value;
    }

    void Steer(float angle)
    {
        frontLeft.steerAngle = angle;
        frontRight.steerAngle = angle;
    }


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
            steerInput = context.ReadValue<float>();

        else if (context.canceled)
            steerInput = 0;
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
        currentSpeed = rb.velocity.magnitude;
        UpdateSpeedometer();
        Brake(brakeInput ? 20000 : 0);
        Motor(gasInput && currentSpeed < maxSpeed ? 5000 : 0);
        Steer(steerInput * 40);
        rb.AddForce(-rb.velocity * drag);
    }
}
