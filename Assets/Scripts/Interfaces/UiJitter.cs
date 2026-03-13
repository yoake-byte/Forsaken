using UnityEngine;
using UnityEngine.EventSystems;

//script so when you hover over a button it shakes a little
public class UIJitter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float shakeMagnitude = 5f;
    public float shakeSpeed = 50f;
    float timer;

    private Vector3 originalPos;
    private bool isHovering = false;

    void Start() => originalPos = transform.localPosition;

    public void OnPointerEnter(PointerEventData eventData) => isHovering = true;

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        transform.localPosition = originalPos;
    }

    void Update()
    {
        if (isHovering)
        {
            timer += Time.deltaTime;

            if (timer >= (1f / shakeSpeed))
            {
                float x = Random.Range(-1f, 1f) * shakeMagnitude;
                float y = Random.Range(-1f, 1f) * shakeMagnitude;
                transform.localPosition = originalPos + new Vector3(x, y, 0);
                timer=0;

            }
        }
    }
}