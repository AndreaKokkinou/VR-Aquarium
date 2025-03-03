using UnityEngine;

public class SwimmingMovement : MonoBehaviour
{
    public float swimForce = 10000f;
    public float drag = 0.98f;
    public float maxSpeed = 100f;
    public float minStrokeVelocity = 0.01f; // Minimum speed to count as a stroke

    private Rigidbody rb;
    private Vector3 lastHandPositionLeft;
    private Vector3 lastHandPositionRight;

    public Transform boundaryCenter;
    public float boundaryRadius = 10f;
    private float leftDot;
    private float rightDot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {

        ProcessSwimming();
        ApplyBoundaryConstraints();
    }

    void ProcessSwimming()
    {
        // Get controller positions
        Vector3 leftHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Vector3 rightHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        // Calculate hand movement velocity
        Vector3 leftHandVelocity = lastHandPositionLeft - leftHandPosition;
        Vector3 rightHandVelocity = lastHandPositionRight - rightHandPosition;

        // Get player's forward direction
        Vector3 playerForward = transform.forward;

        if (OVRInput.Get(OVRInput.Button.One))
        {
            leftDot = Vector3.Dot(leftHandVelocity, playerForward);
            rightDot = Vector3.Dot(rightHandVelocity, playerForward);
        }

        else
        {
            leftDot = Vector3.Dot(leftHandVelocity, -playerForward);
            rightDot = Vector3.Dot(rightHandVelocity, -playerForward);
        }

        Vector3 totalForce = Vector3.zero;

        // Only apply force if hands are moving backward (negative dot product)
        if (leftDot < -minStrokeVelocity)
        {
            totalForce += leftHandVelocity * swimForce;
        }
        if (rightDot < -minStrokeVelocity)
        {
            totalForce += rightHandVelocity * swimForce;
        }

        // Apply force to the Rigidbody
        rb.AddForce(totalForce, ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

        // Apply drag
        rb.linearVelocity *= drag;

        // Store last positions for the next frame
        lastHandPositionLeft = leftHandPosition;
        lastHandPositionRight = rightHandPosition;
    }

    void ApplyBoundaryConstraints()
    {
        if (boundaryCenter == null) return;

        Vector3 offset = transform.position - boundaryCenter.position;
        if (offset.magnitude > boundaryRadius)
        {
            Vector3 clampedPosition = boundaryCenter.position + offset.normalized * boundaryRadius;
            rb.position = clampedPosition;
            rb.linearVelocity = Vector3.zero; 
        }
    }
}
