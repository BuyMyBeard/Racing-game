using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class EngineV2 : MonoBehaviour
{
    // Constants
    Rigidbody rb;
    [SerializeField] AnimationCurve accelerationCurve;
    [SerializeField] AnimationCurve dragCurve;
    [SerializeField] float maxSpeed = 200f;
    [SerializeField] float steerSpeed = 1f;
    [SerializeField] float accelerationDuration = 1f;
    [SerializeField] float brakeMultiplier = 0.97f;
    [SerializeField] float maxSteerAngle = 60f;
    [SerializeField] float speedToConsiderStationary = .01f;
    [SerializeField] float groundCheckDistance;
    [SerializeField] float wheelCloseEnoughToGroundDistance = 0.3f;
    [SerializeField] float terminalVelocity = -30;
    [SerializeField] TextMeshProUGUI speedometer;

    [SerializeField] WheelCollider frontLeftWheel;
    [SerializeField] WheelCollider frontRightWheel;
    [SerializeField] WheelCollider backLeftWheel;
    [SerializeField] WheelCollider backRightWheel;

    WheelCollider[] wheels;

    float SteerAngle
    {
        get => frontLeftWheel.steerAngle;
        set
        {
            frontLeftWheel.steerAngle = value;
            frontRightWheel.steerAngle = value;
        }
    }

    // Runtime vars
    float ivTVal => internalVelocity.magnitude / maxSpeed;
    float accelerationTVal;
    Vector3 internalVelocity;
    Vector3 currentMoveVelocity;
    Vector3 internalRotation;
    Vector3 currentRotation;
    float airTimer;
    float gravityYSpeed;

    bool gasInput = false;
    bool brakeInput = false;
    float steerInput = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        wheels = new WheelCollider[] { frontLeftWheel, frontRightWheel, backLeftWheel, backRightWheel };
    }
    public void OnGas(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            gasInput = true;
            accelerationTVal = ivTVal;
        }

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

    private void FixedUpdate()
    {
        if (frontLeftWheel.isGrounded && frontRightWheel.isGrounded)
        {
            if (gasInput)
            {
                if (internalVelocity.magnitude < maxSpeed)
                {
                    accelerationTVal += Time.fixedDeltaTime / accelerationDuration;
                    internalVelocity = accelerationCurve.Evaluate(accelerationTVal) * maxSpeed * Vector3.forward;
                }
            }

            if (brakeInput)
            {
                internalVelocity *= brakeMultiplier;
            }

            SteerAngle = Mathf.MoveTowards(SteerAngle, maxSteerAngle * steerInput, steerSpeed);

        }

        internalVelocity *= dragCurve.Evaluate(ivTVal);

        currentMoveVelocity = internalVelocity;
        HandleGravity();
        currentMoveVelocity.y = gravityYSpeed;
        transform.Translate(currentMoveVelocity);
        HandleRotation();
    }
    void HandleGravity()
    {
        Debug.DrawLine(transform.position + groundCheckDistance * Vector3.up, transform.position + Vector3.up * -groundCheckDistance, Color.red);
        if (!Physics.Raycast(transform.position + groundCheckDistance * Vector3.up, -Vector3.up, out RaycastHit rh, groundCheckDistance))
        {
            airTimer += Time.fixedDeltaTime;
            gravityYSpeed = Math.Max(Physics.gravity.y * airTimer, terminalVelocity) * Time.fixedDeltaTime;
        } else
        {
            airTimer = 0;
            gravityYSpeed = 0;
        }
    }

    void HandleRotation()
    {
        
        foreach (var wheel in wheels)
        {
            Vector3 wheelOffset = wheel.transform.position - transform.position;
            wheelOffset.y = 0;
            wheelOffset.Normalize();
            Physics.Raycast(wheel.transform.position, -Vector3.up, out RaycastHit rh, groundCheckDistance);
            Debug.DrawLine(wheel.transform.position, wheel.transform.position - Vector3.up * groundCheckDistance, Color.red);
             float diffToGround = Mathf.Abs(rh.distance - wheel.radius);
            if (diffToGround > wheelCloseEnoughToGroundDistance)
            {
                transform.Rotate(wheelOffset * Mathf.Sign(rh.distance - wheel.radius) * diffToGround);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        speedometer.SetText($"{Mathf.Round(currentMoveVelocity.magnitude)} km/h");
    }
}
