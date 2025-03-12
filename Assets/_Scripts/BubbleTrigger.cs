using UnityEngine;

public class BubbleTrigger : MonoBehaviour
{
    public ParticleSystem bubbleParticles; // Reference to the Particle System

    void Start()
    {
        //bubbleParticles = GetComponent<ParticleSystem>();
        bubbleParticles.Stop(); // Ensure bubbles are off by default
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PetFish") || other.CompareTag("PlayerHand")) // Fish or VR hand enters
        {
            bubbleParticles.Play(); // Start bubbles
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PetFish") || other.CompareTag("PlayerHand")) // Fish or VR hand exits
        {
            bubbleParticles.Stop(); // Stop bubbles
        }
    }
}