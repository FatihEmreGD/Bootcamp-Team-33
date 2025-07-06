using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject heldItem;       // Oyuncunun elindeki e�ya
    public Transform handPoint;       // E�yay� tutaca�� nokta (karakterin eli gibi)
    public Table activeTable;  // Yakla��lan masa


    public void PickUpItem(GameObject item)
    {
        heldItem = item;
        item.transform.SetParent(handPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    public GameObject DropItem()
    {
        GameObject item = heldItem;
        heldItem = null;
        return item;
    }

    public bool IsHoldingItem()
    {
        return heldItem != null;
    }
}
