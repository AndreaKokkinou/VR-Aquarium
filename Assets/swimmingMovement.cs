using UnityEngine;

public class SwimmingMovement : MonoBehaviour
{
    public float swimForce = 100f;
    public float drag = 0.98f;
    public float maxSpeed = 10f;
    public float minStrokeVelocity = 0.05f; // Minimum speed to count as a stroke

    private Rigidbody rb;
    private Vector3 lastHandPositionLeft;
    private Vector3 lastHandPositionRight;

    public GameObject ForwardObjectAnchor;

    public Transform boundaryCenter;
    public float boundaryRadius = 10f;
    private float leftDot;
    private float rightDot;

    private AudioSource audioSource;
    public AudioClip bubbleSound;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        print("RigidBody: " + rb);
        rb.useGravity = false;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;

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
        Vector3 playerForward = ForwardObjectAnchor.transform.forward;
        print("Player Forward Direction: " + playerForward);

        if (OVRInput.Get(OVRInput.Button.One))
        {
            print("Ran OVR Input Button One");
            leftDot = Vector3.Dot(leftHandVelocity, playerForward);
            rightDot = Vector3.Dot(rightHandVelocity, playerForward);
        }

        else
        {
            print("Ran else for OVR Input Button One");
            leftDot = Vector3.Dot(leftHandVelocity, -playerForward);
            rightDot = Vector3.Dot(rightHandVelocity, -playerForward);
        }

        if (OVRInput.Get(OVRInput.Button.Two))
        {
            leftDot = Vector3.Dot(Vector3.zero, playerForward);
            rightDot = Vector3.Dot(Vector3.zero, playerForward);
        }

        Vector3 totalForce = Vector3.zero;
        bool playedSound = false;

        // Only apply force if hands are moving backward (negative dot product)
        if (leftDot < -minStrokeVelocity)
        {
            totalForce += leftHandVelocity * swimForce;
            playedSound = true;
        }
        if (rightDot < -minStrokeVelocity)
        {
            totalForce += rightHandVelocity * swimForce;
            playedSound = true;
        }

        print("Total Force Vals: " + totalForce);


        // Apply force to the Rigidbody
        rb.AddForce(totalForce, ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

        // Apply drag
        rb.linearVelocity *= drag;

        if (playedSound && bubbleSound != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(bubbleSound);
        }

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
