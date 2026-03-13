using UnityEngine;

public class RoomBound : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameManager.OpenSceneMenu();
        }
    }
}
