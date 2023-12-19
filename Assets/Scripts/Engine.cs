using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Engine : MonoBehaviour
{
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider backLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] float accelerationSoundRPMTreshold = 30;
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
    [SerializeField] float motorAcceleration = 500;
    [SerializeField] float brakeAcceleration = 500;



    [Header("Audio")]
    [SerializeField] AudioSource idleAudioSource;
    [SerializeField] AudioSource accelerationAudioSource;
    [SerializeField] AudioSource brakeAudioSource;
    [SerializeField] AudioSource crashAudioSource;
    [SerializeField] float pitchFactor = .001f;
    [SerializeField] float volumeFactor = .001f;
    [SerializeField] float minPitch = 0.5f;
    [SerializeField] float maxPitch = 1.5f;
    [SerializeField] float fadeSpeed = 1;
    [SerializeField] float brakeSoundTreshold = 5;
    [SerializeField] float crashForceTreshold = 100;
    [SerializeField] float hopYSpeed = 100f;

    /// <summary>
    /// Current forward speed in m / s
    /// </summary>
    float currentSpeed;
    bool gasInput = false;
    bool brakeInput = false;
    bool driftInput = false;
    bool isBraking = false;
    bool isRevvingEngine = false;
    float steerInput = 0;
    Rigidbody rb;
    CheckpointManager checkpointManager;
    CameraMovement cameraMovement;
    float currentSteerAngle = 0;
    bool GoingForward => Vector3.Dot(rb.velocity, transform.forward) > 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        checkpointManager = FindObjectOfType<CheckpointManager>();
        if (checkpointManager == null) Debug.Log("Can't find Checkpoint Manager");
        cameraMovement = GetComponent<CameraMovement>();
    }
    void UpdateSpeedometer() => speedometer.SetText($"{ Mathf.Round(currentSpeed /  1000 * 3600)} km/h");
    void Brake(float value)
    {
        //backLeft.brakeTorque = Mathf.MoveTowards(backLeft.brakeTorque, value, brakeAcceleration * Time.deltaTime);
        //backRight.brakeTorque = Mathf.MoveTowards(backRight.motorTorque, value, brakeAcceleration * Time.deltaTime);
        //frontLeft.brakeTorque = Mathf.MoveTowards(frontLeft.motorTorque, value, brakeAcceleration * Time.deltaTime);
        //frontRight.brakeTorque = Mathf.MoveTowards(frontRight.motorTorque, value, brakeAcceleration * Time.deltaTime);
        backLeft.brakeTorque = value;
        backRight.brakeTorque = value;
        frontLeft.brakeTorque = value;
        frontRight.brakeTorque = value;
    }

    void Motor(float value)
    {
        backLeft.motorTorque­ = Mathf.MoveTowards(backLeft.motorTorque, value, motorAcceleration * Time.deltaTime);
        backRight.motorTorque = Mathf.MoveTowards(backRight.motorTorque, value, motorAcceleration * Time.deltaTime);
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
        {
            driftInput = true;
            if (!GoingForward) return;

            rb.velocity += Vector3.up * hopYSpeed;
        }

        else if (context.canceled)
            driftInput = false;
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if (context.started)
            Respawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude < speedToConsiderStationary)
        {
            if (brakeInput && gasInput)
            {
                Brake(brakeTorque);
                Motor(forwardTorque);
                isBraking = true;
                isRevvingEngine = true;
            }
            else if (brakeInput)
            {
                Brake(0);
                Motor(-backwardTorque);
                isBraking = false;
                isRevvingEngine = true;
            }
            else if (gasInput)
            {
                Brake(0);
                Motor(forwardTorque);
                isBraking = false;
                isRevvingEngine = true;
            }
            else
            {
                Brake(0);
                Motor(0);
                isBraking = false;
                isRevvingEngine = false;
            }
        }
        else if (GoingForward)
        {
            Brake(brakeInput ? brakeTorque : 0);
            Motor(gasInput ? forwardTorque : 0);
            isBraking = brakeInput;
            isRevvingEngine = gasInput;
        }
        else
        {
            Brake(gasInput ? brakeTorque : 0);
            Motor(brakeInput ? -backwardTorque : 0);
            isBraking = gasInput;
            isRevvingEngine = brakeInput;

        }

        float slope = (minSteerAngle - maxSteerAngle) / minSteerAngleSpeed;
        float steerAngle = slope * Mathf.Clamp(currentSpeed, 0, minSteerAngleSpeed) + maxSteerAngle;
        float desiredSteerAngle = steerInput * steerAngle;
        float deltaAngle = steerSpeed * Time.deltaTime;
        currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, desiredSteerAngle, deltaAngle);
        Steer(currentSteerAngle);

        MakeEngineSound();
    }
    private void FixedUpdate()
    {
        currentSpeed = rb.velocity.magnitude;
        UpdateSpeedometer();
        rb.AddForce(-rb.velocity * drag);
    }

    public void MakeEngineSound()
    {
        float currentRPM = (Mathf.Abs(backLeft.rpm) + Mathf.Abs(backRight.rpm)) / 2; 
        bool wheelsGrounded = frontLeft.isGrounded || frontRight.isGrounded || backLeft.isGrounded || backRight.isGrounded;
        if (currentRPM < accelerationSoundRPMTreshold && !isRevvingEngine)
        {
            accelerationAudioSource.mute = true;
            idleAudioSource.volume = Mathf.MoveTowards(idleAudioSource.volume, 1, Time.deltaTime * fadeSpeed);
            brakeAudioSource.mute = !(isBraking && wheelsGrounded && (currentSpeed > brakeSoundTreshold || isRevvingEngine));
        }
        else
        {
            accelerationAudioSource.mute = false;

            if (isRevvingEngine)
            {
                accelerationAudioSource.volume = Mathf.Clamp(currentRPM * volumeFactor, .25f, .75f);
                accelerationAudioSource.pitch = Mathf.Clamp(currentRPM * pitchFactor, minPitch, maxPitch);
                idleAudioSource.volume = Mathf.MoveTowards(idleAudioSource.volume, .5f, Time.deltaTime * fadeSpeed);
            }
            else
            {
                accelerationAudioSource.volume = 0;
                idleAudioSource.volume = Mathf.MoveTowards(idleAudioSource.volume, 1, Time.deltaTime * fadeSpeed);
            }
            brakeAudioSource.mute = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        float force = collision.impulse.magnitude / Time.fixedDeltaTime;
        Debug.Log(force);
        if (force > crashForceTreshold)
            crashAudioSource.Play();
    }

    public void Respawn()
    {
        Transform respawnPosition = checkpointManager.CurrentRespawnPosition;
        transform.SetPositionAndRotation(respawnPosition.position, respawnPosition.rotation);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        backLeft.motorTorque = 0;
        backRight.motorTorque = 0;
        cameraMovement.ResetRotation();
    }
}
