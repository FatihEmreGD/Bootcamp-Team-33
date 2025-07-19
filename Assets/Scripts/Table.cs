using UnityEngine;

public class Table : MonoBehaviour
{
    public Transform itemPoint;
    public GameObject currentItem;

    public bool IsOccupied()
    {
        return currentItem != null;
    }

    public bool TryPlaceItem(GameObject item)
    {
        if (item == null) return false;

        IngredientItem incomingIngredient = item.GetComponent<IngredientItem>();
        Pot incomingPot = item.GetComponent<Pot>();
        Plate incomingPlate = item.GetComponent<Plate>();

        if (!IsOccupied())
        {
            PlaceItemOnEmptyTable(item);
            return true;
        }
        else
        {
            if (incomingIngredient != null)
            {
                Pot potOnTable = currentItem.GetComponent<Pot>();
                if (potOnTable != null)
                {
                    if (potOnTable.AddIngredient(incomingIngredient.ingredient))
                    {
                        Destroy(item);
                        return true;
                    }
                    return false;
                }
                Debug.LogWarning("Table is occupied.");
                return false;
            }
            else if (incomingPot != null)
            {
                Debug.LogWarning("Table is occupied.");
                return false;
            }
            else if (incomingPlate != null)
            {
                Pot potOnTable = currentItem.GetComponent<Pot>();
                if (potOnTable != null)
                {
                    if (potOnTable.currentState == Pot.PotState.Cooked)
                    {
                        if (incomingPlate.IsEmpty())
                        {
                            Recipe servedDish = potOnTable.ServeDish();
                            if (servedDish != null)
                            {
                                incomingPlate.TryPlaceDish(servedDish);
                                Debug.Log($"Served {servedDish.name} onto the plate.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Plate is not empty!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Pot is not cooked!");
                    }
                    return false; // The plate itself is not placed on the table
                }
                Debug.LogWarning("Table is occupied.");
                return false;
            }
            return false;
        }
    }

    private void PlaceItemOnEmptyTable(GameObject item)
    {
        currentItem = item;
        item.transform.SetParent(itemPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    public GameObject TryTakeItem()
    {
        if (!IsOccupied()) return null;

        GameObject itemToTake = currentItem;
        currentItem = null;
        itemToTake.transform.SetParent(null);
        return itemToTake;
    }
}
