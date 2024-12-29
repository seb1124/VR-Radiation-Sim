using UnityEngine;
using UnityEngine.Events;
 // Ensure you have this namespace if using XR interaction system

public class GeigerCounter : MonoBehaviour
{
    public Transform player; // Reference to the player/camera transform.
    public AudioSource clickSound; // AudioSource for the Geiger counter click sound.
    public AudioClip geigerClickClip; // AudioClip for the Geiger counter click sound.

    public float maxClickRate = 20f; // Maximum clicks per second.
    public float minClickRate = 0.5f; // Minimum clicks per second.
    public float detectionRange = 10f; // Maximum range of detection.

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable; // Reference to the XRGrabInteractable component
    private float nextClickTime = 0f;
    private Transform radioactiveElement;   // Dynamically assigned by first object found with "Radioactive" tag

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(); // Get the XRGrabInteractable component
        if (grabInteractable == null)
        {
            Debug.LogError("GeigerCounter script must be attached to an object with XRGrabInteractable component.");
            return;
        }

    }

    void Start(){
        // Find the radioactive element by tag
        GameObject radioactiveObject = GameObject.FindWithTag("Radioactive");
        if (radioactiveObject == null)
        {
            Debug.LogError("No GameObject with the 'Radioactive' tag found in the scene.");
            return;
        }
        radioactiveElement = radioactiveObject.transform;
    }

    void Update()
    {
        // Check if the Geiger counter is grabbed
        if (grabInteractable.isSelected)
        {
            // Calculate distance between player and radioactive element.
            float distance = Vector3.Distance(player.position, radioactiveElement.position);

            // Map distance to click frequency.
            float clickRate = Mathf.Lerp(maxClickRate, minClickRate, distance / detectionRange);
            clickRate = Mathf.Clamp(clickRate, minClickRate, maxClickRate);

            // Adjust volume based on distance
            clickSound.volume = Mathf.Clamp(1.0f / (distance * distance), 0.1f, 1.0f);

            // Play click sound based on click rate.
            if (Time.time >= nextClickTime && distance <= detectionRange)
            {
                clickSound.clip = geigerClickClip; // Assign the .wav clip
                clickSound.Play();
                nextClickTime = Time.time + (1f / clickRate);
            }
        }
        else
        {
            // Stop the sound when not being grabbed
            clickSound.Stop();
        }
    }
}
