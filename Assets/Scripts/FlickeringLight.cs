using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    [Header("Flicker Settings")]
    [Tooltip("Minimum light intensity during flicker")]
    public float minIntensity = 0.5f;

    [Tooltip("Maximum light intensity during flicker")]
    public float maxIntensity = 2f;

    [Tooltip("How fast the light flickers")]
    public float flickerSpeed = 10f;

    [Tooltip("Smoothness of the flicker")]
    public float smoothness = 5f;

    [Header("Optional")]
    [Tooltip("Enable random flicker variation")]
    public bool randomFlicker = true;

    private Light pointLight;
    private float randomOffset;

    void Start()
    {
        pointLight = GetComponent<Light>();

        // Makes each light flicker differently
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float noise;

        if (randomFlicker)
        {
            noise = Mathf.PerlinNoise(Time.time * flickerSpeed + randomOffset, 0f);
        }
        else
        {
            noise = Mathf.PingPong(Time.time * flickerSpeed, 1f);
        }

        float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        pointLight.intensity = Mathf.Lerp(
            pointLight.intensity,
            targetIntensity,
            Time.deltaTime * smoothness
        );
    }
}