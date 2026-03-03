using UnityEngine;

public class TriggerBattle : MonoBehaviour
{
    [SerializeField] GameManager manager;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            manager.MakeDecision();
        }
    }
}
