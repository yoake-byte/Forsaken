using UnityEngine;

public class RoomBound : MonoBehaviour
{
    [SerializeField] private CutsceneManager cutsceneManager;
    [SerializeField] private int sceneIndex;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            cutsceneManager.PlayCutScene(sceneIndex);
        }
    }
}
