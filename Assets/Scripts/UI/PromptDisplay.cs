using UnityEngine;
using TMPro;
public class PromptDisplay : MonoBehaviour
{
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private string promptText; 
    private bool wasDisplayed = false;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!wasDisplayed && other.gameObject.CompareTag("Player"))
        {
            promptPanel.SetActive(true);
            prompt.text = promptText;  
            
        }
       
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            promptPanel.SetActive(false);
            prompt.text = "";  
            wasDisplayed = true;
        }
    }
}
