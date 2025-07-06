using UnityEngine;

public class Table : MonoBehaviour
{
    public Transform itemPoint;
    public GameObject currentItem;

    private PlayerInteraction player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerInteraction>();
            player.activeTable = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player != null)
                player.activeTable = null;
        }
    }

    public void OnUIClick()
    {
        Debug.Log("Masa üzerindeki butona tıklandı!");

        if (player == null) return;

        if (currentItem != null && !player.IsHoldingItem())
        {
            player.PickUpItem(currentItem);
            currentItem = null;
        }
        else if (currentItem == null && player.IsHoldingItem())
        {
            GameObject item = player.DropItem();
            currentItem = item;
            item.transform.SetParent(itemPoint);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
    }
}
