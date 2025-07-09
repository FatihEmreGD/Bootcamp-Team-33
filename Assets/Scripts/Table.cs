using UnityEngine;

public class Table : MonoBehaviour
{
    public Transform itemPoint;
    public GameObject currentItem;

    // Tabağı masaya bırakmayı dener
    public bool TryPlaceItem(GameObject item)
    {
        Debug.Log($"Table ({gameObject.name}): TryPlaceItem called with item: {(item != null ? item.name : "NULL")}. Current item on table: {(currentItem != null ? currentItem.name : "NULL")}");

        if (item == null) return false; // Gelen eşya null ise işlem yapma

        // Gelen eşyanın türünü belirle
        IngredientItem incomingIngredient = item.GetComponent<IngredientItem>();
        Pot incomingPot = item.GetComponent<Pot>();

        // Case 1: Masada hiçbir şey yok - Herhangi bir eşya konulabilir
        if (currentItem == null)
        {
            Debug.Log($"Table ({gameObject.name}): Table is empty. Placing {item.name}.");
            // Boş masaya malzeme veya tencere konulabilir
            currentItem = item;
            item.transform.SetParent(itemPoint);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            Debug.Log($"Table ({gameObject.name}): Item {item.name} placed on empty table. Returning TRUE.");
            return true;
        }

        // Case 2: Masada bir eşya var
        else 
        {
            Debug.Log($"Table ({gameObject.name}): Table is NOT empty. Current item: {currentItem.name}.");
            // Gelen eşya malzeme ise
            if (incomingIngredient != null)
            {
                Pot potOnTable = currentItem.GetComponent<Pot>();
                if (potOnTable != null) // Masadaki eşya tencere ise, malzemeyi tencereye koymayı dene
                {
                    Debug.Log($"Table ({gameObject.name}): Found pot on table: {potOnTable.name}. Attempting to add ingredient {incomingIngredient.ingredient.ingredientName}.");
                    if (potOnTable.AddIngredient(incomingIngredient.ingredient))
                    {
                        Debug.Log($"Table ({gameObject.name}): Ingredient {incomingIngredient.ingredient.ingredientName} added to pot on table {gameObject.name}. Returning TRUE.");
                        return true; // Malzeme tencereye eklendi
                    }
                    else
                    {
                        Debug.LogWarning($"Table ({gameObject.name}): Pot.AddIngredient failed for {incomingIngredient.ingredient.ingredientName}. Pot state: {potOnTable.currentState}. Item remains in hand. Returning FALSE.");
                        return false; // Malzeme tencereye eklenemedi, elde kalır
                    }
                }
                else // Masadaki eşya tencere değilse, malzemeyi koyamazsın
                {
                    Debug.LogWarning($"Table ({gameObject.name}): Incoming is ingredient, but current item ({currentItem.name}) is not a pot. Cannot place ingredient. Item remains in hand. Returning FALSE.");
                    return false; // Malzeme masaya konamaz, elde kalır
                }
            }
            // Gelen eşya tencere ise
            else if (incomingPot != null)
            {
                Debug.LogWarning($"Table ({gameObject.name}): Cannot place pot {item.name}. Table already has {currentItem.name}. Item remains in hand. Returning FALSE.");
                return false; // Masa dolu, tencere konamaz, elde kalır
            }
            // Gelen eşya başka bir tür ise (ne malzeme ne tencere)
            else
            {
                Debug.LogWarning($"Table ({gameObject.name}): Cannot place {item.name}. Table already has {currentItem.name}. Item remains in hand. Returning FALSE.");
                return false; // Masa dolu, eşya konamaz, elde kalır
            }
        }
    }

    // Masadan tabağı almayı dener
    public GameObject TryTakeItem()
    {
        Debug.Log($"Table ({gameObject.name}): TryTakeItem called. Current item on table: {(currentItem != null ? currentItem.name : "NULL")}");
        if (currentItem != null)
        {
            GameObject itemToTake = currentItem;
            currentItem = null;
            
            itemToTake.transform.SetParent(null); // Parent'ı kaldır
            Debug.Log($"Table ({gameObject.name}): Item {itemToTake.name} taken from table. Returning item.");
            return itemToTake;
        }
        Debug.Log($"Table ({gameObject.name}): is empty. Nothing to take. Returning NULL.");
        return null;
    }
}
