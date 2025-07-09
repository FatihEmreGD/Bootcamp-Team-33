using UnityEngine;

public class UIController : MonoBehaviour
{
    public PlayerInteraction playerInteraction;
    public GameObject actionButton; 

    void Update()
    {
        if (actionButton != null && playerInteraction != null)
        {
            // actionButton'ı, oyuncu etkileşime geçilebilir bir nesneye yakınsa aktif et
            actionButton.SetActive(playerInteraction.GetClosestInteractable() != null);
        }
    }

    public void OnUniversalButtonClick()
    {
        if (playerInteraction != null)
        {
            playerInteraction.Interact(); // Doğrudan PlayerInteraction'ın Interact metodunu çağır
        }
        else
        {
            Debug.Log("PlayerInteraction referansı ayarlanmamış.");
        }
    }
}