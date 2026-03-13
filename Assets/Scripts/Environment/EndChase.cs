using UnityEngine;

public class EndChase : MonoBehaviour
{
    [SerializeField] private CutsceneManager cutsceneManager;
    [SerializeField] private int timelineIndex;
    [SerializeField] private GameObject eva;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            eva.transform.position = other.gameObject.transform.position;
            cutsceneManager.PlayCutScene(timelineIndex);
            Destroy(gameObject);
        }
    }
}
