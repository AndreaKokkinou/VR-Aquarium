using UnityEngine;

public class CubeBehavior : MonoBehaviour
{
    public float speed = 5f;
    public GameObject cubePrefab;
    private Vector3 initialPosition;
    private float respawnTimer = 2f;
    private float timeLeft;

    public float cubeLifetime = 10f;

    void Start()
    {

        initialPosition = transform.position;


        timeLeft = respawnTimer;

        Destroy(gameObject, cubeLifetime);
    }

    void Update()
    {

        transform.position += Vector3.forward * speed * Time.deltaTime;


        timeLeft -= Time.deltaTime;


        if (timeLeft <= 0)
        {
            RespawnCube();
            timeLeft = respawnTimer;
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Capsule"))
        {
            Destroy(gameObject);
        }
    }

    void RespawnCube()
    {

        GameObject newCube = Instantiate(cubePrefab, initialPosition, Quaternion.identity);


        Renderer cubeRenderer = newCube.GetComponent<Renderer>();
        if (Random.value > 0.5f)
        {
            cubeRenderer.material.color = Color.red;
        }
        else
        {
            cubeRenderer.material.color = Color.blue;
        }


        Rigidbody rb = newCube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }


        Destroy(newCube, cubeLifetime);
    }
}