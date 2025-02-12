using UnityEngine;

public class CapsuleBehavior : MonoBehaviour
{
    private AudioSource audioSource; 

    void Start()
    {
        
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Cube"))
        {
          
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}