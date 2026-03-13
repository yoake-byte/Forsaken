using UnityEngine;
using System.Collections;

public class ShockwaveTrigger : MonoBehaviour
{
    private Material ShockwaveMat;
    
    [Header("Settings")]
    public float duration = 0.8f;   
    public float startDist = -0.1f; //Start position
    public float endDist = 1.0f;    
    public bool playOnStart = true;   

    [Header("Strength Settings")]
    public float startStrength = -0.2f; // Initial strength
    public float endStrength = -0.2f;   // Final strength (0 means the shockwave effect will fade out)

    [Header("Shader Properties")]
    public string distProperty = "_wavedistance";  //Var name in shader
    public string strengthProperty = "_shockwavestrength"; //Var name in shader

    private void Awake()
    {
        // Get the material instance from the renderer
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            ShockwaveMat = rend.material;
            ShockwaveMat.SetFloat(distProperty, startDist);
            ShockwaveMat.SetFloat(strengthProperty, startStrength);
        }
    }

    private void Start()
    {
        // Trigger by default if playOnStart is true
        if (playOnStart)
        {
            PlayShockwave();
        }
    }

    public void PlayShockwave()
    {
        StopAllCoroutines(); 
        StartCoroutine(AnimateWave());
    }

    IEnumerator AnimateWave()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float lerpProgress = elapsed / duration;

            float currentDist = Mathf.Lerp(startDist, endDist, lerpProgress);
            ShockwaveMat.SetFloat(distProperty, currentDist);

            //Linearly change Strength
            float currentStrength = Mathf.Lerp(startStrength, endStrength, lerpProgress);
            ShockwaveMat.SetFloat(strengthProperty, currentStrength);

            yield return null;
        }
        ShockwaveMat.SetFloat(distProperty, -0.1f);
    }
}