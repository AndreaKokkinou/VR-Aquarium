using UnityEngine;
using UnityEngine.XR;

public class SwimmingMechanic : MonoBehaviour
{
    public float swimForce = 5f; // Strength of each stroke
    public float drag = 0.98f; // Water resistance
    public float maxSpeed = 3f; // Prevents unnatural speeds

    private Rigidbody rb;
    private Vector3 lastHandPositionLeft;
    private Vector3 lastHandPositionRight;

    public Transform boundaryCenter;
    public float boundaryRadius = 10f; // Limits swimming area

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // No gravity, we control buoyancy
    }

    void FixedUpdate()
    {
        ProcessSwimming();
        ApplyBoundaryConstraints();
    }

    void ProcessSwimming()
    {
        // Get the VR controller positions
        Vector3 leftHandPosition = InputTracking.GetLocalPosition(XRNode.LeftHand);
        Vector3 rightHandPosition = InputTracking.GetLocalPosition(XRNode.RightHand);

        // Calculate the movement force based on hand motion
        Vector3 leftHandVelocity = (lastHandPositionLeft - leftHandPosition) * swimForce;
        Vector3 rightHandVelocity = (lastHandPositionRight - rightHandPosition) * swimForce;

        Vector3 totalForce = leftHandVelocity + rightHandVelocity;

        // Apply force but limit max speed
        rb.AddForce(totalForce, ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

        // Apply water drag
        rb.linearVelocity *= drag;
        // Store the last positions for next frame
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
            rb.linearVelocity = Vector3.zero; // Stop movement when hitting the boundary
        }
    }
}