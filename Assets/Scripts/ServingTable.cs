using UnityEngine;

public class ServingTable : MonoBehaviour
{
    public Transform platePlacementPoint; // Tabağın konulacağı nokta
    public GameObject currentPlate; // Masadaki mevcut tabak

    public bool IsOccupied()
    {
        return currentPlate != null;
    }

    // Servis masasına tabak koymayı dener
    public bool TryPlaceItem(GameObject item)
    {
        if (item == null) return false;

        Plate incomingPlate = item.GetComponent<Plate>();

        // Sadece tabakları kabul et
        if (incomingPlate == null)
        {
            Debug.LogWarning("ServingTable: Only plates can be placed on the serving table.");
            return false;
        }

        // Masa doluysa veya tabak boşsa kabul etme
        if (IsOccupied())
        {
            Debug.LogWarning("ServingTable: Table is already occupied.");
            return false;
        }

        if (incomingPlate.IsEmpty())
        {
            Debug.LogWarning("ServingTable: Cannot place an empty plate on the serving table.");
            return false;
        }

        // Tabakta yemek varsa kabul et
        currentPlate = item;
        item.transform.SetParent(platePlacementPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        Debug.Log($"ServingTable: Placed {item.name} on the serving table.");
        return true;
    }

    // Servis masasından tabak almayı dener
    public GameObject TryTakeItem()
    {
        if (!IsOccupied()) return null;

        GameObject plateToTake = currentPlate;
        currentPlate = null;
        plateToTake.transform.SetParent(null);
        Debug.Log($"ServingTable: Took {plateToTake.name} from the serving table.");
        return plateToTake;
    }
}
