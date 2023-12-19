using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] TextMeshProUGUI speedometer;

    [SerializeField] WheelCollider frontLeftWheel;
    [SerializeField] WheelCollider frontRightWheel;

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

    bool gasInput = false;
    bool brakeInput = false;
    float steerInput = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        currentMoveVelocity = Vector3.zero;
        if (gasInput)
        {
            if (internalVelocity.magnitude < maxSpeed)
            {
                accelerationTVal += Time.deltaTime / accelerationDuration;
                internalVelocity = accelerationCurve.Evaluate(accelerationTVal) * maxSpeed * transform.forward;
            }
        }
        
        if (brakeInput)
        {
            internalVelocity *= brakeMultiplier;
        }

        SteerAngle = Mathf.MoveTowards(SteerAngle, maxSteerAngle * steerInput, steerSpeed);

        internalVelocity *= dragCurve.Evaluate(ivTVal);

        currentMoveVelocity.y = rb.velocity.y;
        currentMoveVelocity = internalVelocity;
        rb.velocity = currentMoveVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        speedometer.SetText($"{Mathf.Round(currentMoveVelocity.magnitude)} km/h");
    }
}
