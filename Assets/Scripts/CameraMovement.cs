using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float catchUpSpeed = 1;
    [Header("Pitch clamping")]
    [Range(-90, 90)]
    [SerializeField] float camMinClamp = -90;
    [Range(-90, 90)]
    [SerializeField] float camMaxClamp = 90;

    [Header("Drift behind player")]
    [Tooltip("Speed at which the camera drifts behind the player when moving and not touching the camera")]
    [SerializeField] float driftSpeed = 10;

    [Tooltip("Time before drift fully kicks in")]
    [SerializeField] float easeInTime = 1;

    [Range(0, 5)]
    [Tooltip("Time before camera drift starts kicking in")]
    [SerializeField] float driftTimer = 1;

    [SerializeField] float driftMinSpeed = 10;

    [SerializeField] float speedToReachMaxDrift = 10;

    [Range(0, 2)]
    [SerializeField] float followTargetVerticalOffset = 0;

    [SerializeField] float controllerSensitivity = 140;
    [SerializeField] float mouseSensitivity = 3;
    Vector2 lookInput = Vector2.zero;
    Rigidbody rb;

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public float CamMinClamp => camMinClamp;
    public float CameraMaxClamp => camMaxClamp;

    float driftTime = 0;

    PlayerInput playerInput;

    public string CurrentControlScheme { get => playerInput.currentControlScheme; }
    public bool KeyboardAndMouseActive { get => CurrentControlScheme == "Keyboard&Mouse"; }
    public bool GamepadActive { get => CurrentControlScheme == "Gamepad"; }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        followTarget.parent = null;
        rb = GetComponent<Rigidbody>();
        //GetComponentInChildren<CinemachineVirtualCamera>().transform.parent = null;
    }
    void Update()
    {
        followTarget.position = transform.position + Vector3.up * followTargetVerticalOffset;
        float appliedSens = GamepadActive ? controllerSensitivity : mouseSensitivity;

        // Quaternion * Quaternion is the same as applying rotation from second to first
        Quaternion cameraRotation = followTarget.transform.localRotation *= Quaternion.AngleAxis(lookInput.x * appliedSens * Time.deltaTime, Vector3.up);

        cameraRotation *= Quaternion.AngleAxis(-lookInput.y * appliedSens * Time.deltaTime, Vector3.right);

        Vector3 cameraAngles = cameraRotation.eulerAngles;

        cameraAngles.z = 0;

        // convert [0,360[ degrees to ]-180:180] degrees to avoid looping at 0 and allowing negative angles.
        // 0 degrees is direction of original angle when camera looks at player
        cameraAngles.x = cameraAngles.x > 180 ? cameraAngles.x - 360 : cameraAngles.x;
        cameraAngles.x = Mathf.Clamp(cameraAngles.x, camMinClamp, camMaxClamp);

        followTarget.transform.localEulerAngles = new Vector3(cameraAngles.x, cameraAngles.y, 0);

        ApplyCameraDrift();
    }

    private void ApplyCameraDrift()
    {

        // Moves Camera behind the player when no look input is given
        Vector3 movementDirection = rb.velocity;
        movementDirection.y = 0;
        if (lookInput.magnitude == 0)
        {
            driftTime += Time.deltaTime;
            if (driftTime < driftTimer) return;

            Vector2 camDirection = new Vector2(followTarget.transform.forward.x, followTarget.transform.forward.z);
            Vector2 playerDirection = new Vector2(rb.velocity.x, rb.velocity.z).normalized;

            float dot = Vector2.Dot(playerDirection, camDirection);

            float dotFactor = (1 - Mathf.Abs(dot)) * Mathf.Clamp01((driftTime - driftTimer) / easeInTime);
            float speedFactor = Mathf.Clamp(movementDirection.magnitude, 0, speedToReachMaxDrift) / speedToReachMaxDrift;
            Quaternion to = Quaternion.LookRotation(movementDirection.normalized + Vector3.down * .5f, Vector3.up);
            followTarget.transform.rotation = Quaternion.Lerp(followTarget.transform.rotation, to, Time.deltaTime * dotFactor * driftSpeed * speedFactor);
        }
        else
            driftTime = 0;
    }
    public void SyncFollowTarget()
    {
        followTarget.position = transform.position + Vector3.up * followTargetVerticalOffset;
    }

    public void ResetRotation()
    {
        followTarget.transform.rotation = transform.rotation * Quaternion.Euler(30, 0, 0);
    }
}