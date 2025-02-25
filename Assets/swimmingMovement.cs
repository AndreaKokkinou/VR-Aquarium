using UnityEngine;
using UnityEngine.XR;

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
        
        Vector3 leftHandPosition = InputTracking.GetLocalPosition(XRNode.LeftHand);
        Vector3 rightHandPosition = InputTracking.GetLocalPosition(XRNode.RightHand);

       
        Vector3 leftHandVelocity = (lastHandPositionLeft - leftHandPosition) * swimForce;
        Vector3 rightHandVelocity = (lastHandPositionRight - rightHandPosition) * swimForce;

        Vector3 totalForce = leftHandVelocity + rightHandVelocity;

    
        rb.AddForce(totalForce, ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

     
        rb.linearVelocity *= drag;
        
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
