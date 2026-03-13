using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    private Light2D lightSource;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 0.1f;
    private float timer;

    void Start()
    {
        lightSource = GetComponent<Light2D>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= flickerSpeed)
        {
            lightSource.intensity = Random.Range(minIntensity, maxIntensity);
            timer = 0;
        }
    }
}