using UnityEngine;

public class SwimmingMovement : MonoBehaviour
{
    public float swimForce = 5f;
    public float drag = 0.98f;
    public float maxSpeed = 3f;

    private Rigidbody rb;
    private Vector3 lastHandPositionLeft;
    private Vector3 lastHandPositionRight;

    public Transform boundaryCenter;
    public float boundaryRadius = 10f;

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
        // Use Meta's SDK to get controller positions
        Vector3 leftHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Vector3 rightHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        // Calculate hand movement velocity
        Vector3 leftHandVelocity = (lastHandPositionLeft - leftHandPosition) * swimForce;
        Vector3 rightHandVelocity = (lastHandPositionRight - rightHandPosition) * swimForce;

        Vector3 totalForce = leftHandVelocity + rightHandVelocity;

        // Apply force to the Rigidbody
        rb.AddForce(totalForce, ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

        // Apply drag
        rb.linearVelocity *= drag;

        // Store the last position
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
