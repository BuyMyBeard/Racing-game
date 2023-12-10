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
    [SerializeField] float forwardTorque = 20000;
    [SerializeField] float backwardTorque = 3000;
    [SerializeField] float brakeTorque = 5000;
    [SerializeField] float drag = 10;
    [SerializeField] float maxSteerAngle = 40;
    [SerializeField] float minSteerAngle = 5;
    [SerializeField] float minSteerAngleSpeed = 60;
    [SerializeField] float steerSpeed = 40;
    [SerializeField] float speedToConsiderStationary = .5f;
    /// <summary>
    /// Current forward speed in m / s
    /// </summary>
    float currentSpeed;
    bool gasInput = false;
    bool brakeInput = false;
    bool driftInput = false;
    float steerInput = 0;
    Rigidbody rb;
    float currentSteerAngle = 0;

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
        bool goingForward = Vector3.Dot(rb.velocity, transform.forward) > 0;
        if (rb.velocity.magnitude < speedToConsiderStationary)
        {
            if (brakeInput && gasInput)
            {
                Brake(brakeTorque);
                Motor(forwardTorque);
            }
            else if (brakeInput)
            {
                Brake(0);
                Motor(-backwardTorque);
            }
            else if (gasInput)
            {
                Brake(0);
                Motor(forwardTorque);
            }
            else
            {
                Brake(0);
                Motor(0);
            }
        }
        else if (goingForward)
        {
            Brake(brakeInput ? brakeTorque : 0);
            Motor(gasInput ? forwardTorque : 0);
        }
        else
        {
            Brake(gasInput ? brakeTorque : 0);
            Motor(brakeInput ? -backwardTorque : 0);
        }

        float slope = (minSteerAngle - maxSteerAngle) / minSteerAngleSpeed;
        float steerAngle = slope * Mathf.Clamp(currentSpeed, 0, minSteerAngleSpeed) + maxSteerAngle;
        float desiredSteerAngle = steerInput * steerAngle;
        float deltaAngle = steerSpeed * Time.deltaTime;
        currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, desiredSteerAngle, deltaAngle);
        Steer(currentSteerAngle);


    }
    private void FixedUpdate()
    {
        currentSpeed = rb.velocity.magnitude;
        UpdateSpeedometer();
        rb.AddForce(-rb.velocity * drag);
    }
}
