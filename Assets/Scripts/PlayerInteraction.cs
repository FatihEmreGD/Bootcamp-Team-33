using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject heldItem;       // Oyuncunun elindeki eþya
    public Transform handPoint;       // Eþyayý tutacaðý nokta (karakterin eli gibi)
    public Table activeTable;  // Yaklaþýlan masa


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
