using UnityEngine;

public class Table : MonoBehaviour
{
    public Transform itemPoint; // E�ya nereye yerle�ecek
    public GameObject currentItem; // Masadaki e�ya (tencere)
    public GameObject uiIcon; // UI ikon

    private bool playerNearby = false;
    private PlayerInteraction player;
    public Canvas canvas;

    private void Start()
    {
        if (canvas != null)
        {
            canvas.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            canvas.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        // UI ba�ta kapal� olsun
        if (uiIcon != null)
            uiIcon.SetActive(false);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            player = other.GetComponent<PlayerInteraction>();
            UpdateUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            player = null;
            uiIcon.SetActive(false);
        }
    }

    private void UpdateUI()
    {
        uiIcon.SetActive(playerNearby);
    }

    public void OnUIClick()
    {
        Debug.Log("Button clicked on table!");
        if (player == null) return;


        if (currentItem != null && !player.IsHoldingItem())
        {
            // Tencereyi al
            player.PickUpItem(currentItem);
            currentItem = null;
        }
        else if (currentItem == null && player.IsHoldingItem())
        {
            // Tencereyi b�rak
            GameObject item = player.DropItem();
            currentItem = item;
            item.transform.SetParent(itemPoint);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }

        UpdateUI();
    }
}
