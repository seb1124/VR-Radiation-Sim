using UnityEngine;

public class RadioactiveElementExposure : MonoBehaviour
{
    public GameObject player; // Reference to the player
    public Material filmGrainMaterial; // Reference to the film grain material
    public float maxExposureDistance = 10.0f; // Maximum distance for exposure
    public float maxFilmGrainIntensity = 1.0f; // Maximum intensity of the film grain effect

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        float intensity = Mathf.Lerp(0, maxFilmGrainIntensity, 1 - Mathf.Clamp01(distance / maxExposureDistance));
        filmGrainMaterial.SetFloat("_GrainIntensity", intensity);
    }
}
