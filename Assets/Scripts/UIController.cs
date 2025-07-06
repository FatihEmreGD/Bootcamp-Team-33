using UnityEngine;

public class UIController : MonoBehaviour
{
    public PlayerInteraction playerInteraction;
    public GameObject actionButton; 

    void Update()
    {
        if (actionButton != null && playerInteraction != null)
        {
            actionButton.SetActive(playerInteraction.activeTable != null);
        }
    }

    public void OnUniversalButtonClick()
    {
        if (playerInteraction != null && playerInteraction.activeTable != null)
        {
            playerInteraction.activeTable.OnUIClick();
        }
        else
        {
            Debug.Log("Hiçbir masaya yakýn deðilsin.");
        }
    }
}
