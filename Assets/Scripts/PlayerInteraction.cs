using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject heldItem;       // Oyuncunun elindeki eşya
    public Transform handPoint;       // Eşyayı tutacağı nokta
    public GameObject ingredientItemPrefab; // Oyuncunun alacağı malzemeyi temsil eden prefab

    [Header("Interaction Settings")]
    public float interactionRadius = 2f; // Etkileşim algılama yarıçapı
    public LayerMask interactableLayer; // Etkileşime geçilebilir nesnelerin Layer'ı
    public float dropDistance = 1f; // Eşyayı bırakırken oyuncudan ne kadar uzağa bırakılacağı

    private List<GameObject> currentInteractables = new List<GameObject>(); // Etkileşim alanındaki nesneler
    private GameObject closestInteractable; // Etkileşime geçilecek en yakın nesne

    void FixedUpdate()
    {
        FindInteractables();
    }

    void FindInteractables()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);

        List<GameObject> newInteractables = new List<GameObject>();
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject && hitCollider.gameObject != heldItem)
            {
                newInteractables.Add(hitCollider.gameObject);
            }
        }

        currentInteractables = newInteractables;

        closestInteractable = GetClosestInteractable();

        if (closestInteractable != null)
        {
            Debug.Log("Closest Interactable: " + closestInteractable.name);
        }
        else
        {
            Debug.Log("No interactable in range.");
        }
    }

    public GameObject GetClosestInteractable()
    {
        if (currentInteractables.Count == 0)
        {
            return null;
        }

        return currentInteractables
            .OrderBy(obj => Vector3.Distance(transform.position, obj.transform.position))
            .FirstOrDefault();
    }

    public void Interact()
    {
        Debug.Log($"PlayerInteraction: Interact called. IsHoldingItem: {IsHoldingItem()}");
        // Eğer elde bir eşya varsa, öncelik onu bırakmak veya kullanmaktır.
        if (IsHoldingItem())
        {
            // Elinde malzeme varsa
            IngredientItem heldIngredient = heldItem.GetComponent<IngredientItem>();
            if (heldIngredient != null)
            {
                Debug.Log($"PlayerInteraction: Holding ingredient: {heldIngredient.ingredient.ingredientName}");
                // En yakın etkileşime geçilebilir nesneye bak
                if (closestInteractable != null)
                {
                    Debug.Log($"PlayerInteraction: Closest interactable: {closestInteractable.name}");
                    // IngredientSource'a geri koymayı dene
                    IngredientSource targetIngredientSource = closestInteractable.GetComponent<IngredientSource>();
                    if (targetIngredientSource != null)
                    {
                        if (targetIngredientSource.TryPlaceIngredient(heldIngredient.ingredient))
                        {
                            Debug.Log("PlayerInteraction: Ingredient placed back into source. Destroying held item.");
                            Destroy(heldItem); // Destroy the item from hand
                            heldItem = null;
                            return; // Interaction handled
                        }
                        else
                        {
                            Debug.Log("PlayerInteraction: Cannot place " + heldIngredient.ingredient.ingredientName + " into " + closestInteractable.name + ". Item remains in hand.");
                            return; // Item remains in hand
                        }
                    }

                    // Tencereye malzeme koymayı dene (yerdeki veya ocaktaki)
                    Pot targetPot = closestInteractable.GetComponent<Pot>();
                    if (targetPot != null)
                    {
                        if (targetPot.AddIngredient(heldIngredient.ingredient))
                        {
                            Debug.Log("PlayerInteraction: Ingredient added to pot. Destroying held item.");
                            Destroy(heldItem);
                            heldItem = null;
                            // If the pot is on a stove, tell the stove to re-check recipe
                            Stove stoveHoldingPot = targetPot.transform.parent?.GetComponentInParent<Stove>();
                            if (stoveHoldingPot != null)
                            {
                                stoveHoldingPot.CheckRecipeAndStartCookingPublic(); // Call the public method
                                Debug.Log("PlayerInteraction: Pot is on a stove. Stove should re-check recipe.");
                            }
                            return;
                        }
                    }

                    // Ocağa malzeme koymayı dene (eğer ocakta tencere varsa)
                    Stove targetStove = closestInteractable.GetComponent<Stove>();
                    if (targetStove != null && targetStove.IsOccupied && targetStove.currentPot != null)
                    {
                        if (targetStove.currentPot.AddIngredient(heldIngredient.ingredient))
                        {
                            Debug.Log("PlayerInteraction: Ingredient added to pot on stove. Destroying held item.");
                            Destroy(heldItem);
                            heldItem = null;
                            // Tell the stove to re-check recipe
                            targetStove.CheckRecipeAndStartCookingPublic(); // Call the public method
                            Debug.Log("PlayerInteraction: Pot is on this stove. Stove should re-check recipe.");
                            return;
                        }
                    }
                    else if (targetStove != null && !targetStove.IsOccupied) // Ocak boşsa ve malzeme koymaya çalışıyorsa
                    {
                        Debug.Log("PlayerInteraction: Cannot add ingredient to an empty stove. Place a pot first. Item remains in hand.");
                        return; // Item remains in hand
                    }

                    // Masaya bırakmayı dene
                    Table targetTable = closestInteractable.GetComponent<Table>();
                    if (targetTable != null)
                    {
                        Debug.Log("PlayerInteraction: Trying to place item on table.");
                        bool placedOnTable = targetTable.TryPlaceItem(heldItem);
                        if (placedOnTable)
                        {
                            Debug.Log("PlayerInteraction: Item placed on table. Destroying held item.");
                            Destroy(heldItem); // FIX: Destroy held item after successful placement on table
                            heldItem = null;
                            return;
                        }
                        else
                        {
                            Debug.Log("PlayerInteraction: Failed to place item on table. Item remains in hand.");
                            return; // Item remains in hand
                        }
                    }
                }

                // If no specific interaction handled, and it's an ingredient, just warn and keep in hand
                Debug.Log("PlayerInteraction: No valid place to put ingredient. Item remains in hand.");
                return;
            }

            // Elinde tencere varsa
            Pot heldPot = heldItem.GetComponent<Pot>();
            if (heldPot != null)
            {
                Debug.Log($"PlayerInteraction: Holding pot: {heldPot.name}");
                if (closestInteractable != null)
                {
                    Debug.Log($"PlayerInteraction: Closest interactable: {closestInteractable.name}");
                    // Ocağa tencere koymayı dene
                    Stove targetStove = closestInteractable.GetComponent<Stove>();
                    if (targetStove != null)
                    {
                        Debug.Log("PlayerInteraction: Trying to place pot on stove.");
                        if (targetStove.PlacePot(heldPot))
                        {
                            heldItem = null;
                            Debug.Log("PlayerInteraction: Pot placed on stove.");
                            return;
                        }
                        else
                        {
                            Debug.Log("PlayerInteraction: Failed to place pot on stove. Pot remains in hand.");
                            return; // Pot remains in hand
                        }
                    }

                    // Masaya bırakmayı dene
                    Table targetTable = closestInteractable.GetComponent<Table>();
                    if (targetTable != null)
                    {
                        Debug.Log("PlayerInteraction: Trying to place pot on table.");
                        if (targetTable.TryPlaceItem(heldItem))
                        {
                            heldItem = null;
                            Debug.Log("PlayerInteraction: Pot placed on table.");
                            return;
                        }
                        else
                        {
                            Debug.Log("PlayerInteraction: Failed to place pot on table. Pot remains in hand.");
                            return; // Pot remains in hand
                        }
                    }
                }

                // Hiçbir yere bırakılamıyorsa veya etkileşime geçilecek nesne yoksa elde tut
                Debug.Log("PlayerInteraction: No valid place to put pot. Item remains in hand.");
                return;
            }

            // Elinde başka bir eşya varsa ve bırakacak yer yoksa elde tut
            Debug.Log("PlayerInteraction: No valid place to put item. Item remains in hand.");
            return;
        }
        // Elde eşya yoksa, nesnelerle etkileşime geçmeyi dene (alma işlemleri)
        else
        {
            Debug.Log("PlayerInteraction: Not holding item. Attempting to pick up.");
            if (closestInteractable == null)
            {
                Debug.Log("PlayerInteraction: No object to interact with.");
                return;
            }

            // Malzeme kaynağından almayı dene
            IngredientSource ingredientSource = closestInteractable.GetComponent<IngredientSource>();
            if (ingredientSource != null)
            {
                Ingredient ingredient = ingredientSource.TakeIngredient();
                if (ingredient != null)
                {
                    GameObject newIngredientItemGO = Instantiate(ingredientItemPrefab);
                    IngredientItem newIngredientItem = newIngredientItemGO.GetComponent<IngredientItem>();
                    if (newIngredientItem != null)
                    {
                        newIngredientItem.ingredient = ingredient;
                        PickUpItem(newIngredientItemGO);
                        Debug.Log("PlayerInteraction: Picked up ingredient: " + ingredient.ingredientName);
                    }
                    else
                    {
                        Debug.LogError("PlayerInteraction: IngredientItem prefab does not have an IngredientItem component!");
                    }
                }
                return;
            }

            // Yerdeki tencereyi almayı dene
            Pot targetPot = closestInteractable.GetComponent<Pot>();
            if (targetPot != null)
            {
                PickUpItem(targetPot.gameObject);
                Debug.Log("PlayerInteraction: Picked up pot from ground.");
                return;
            }

            // Ocaktan tencere almayı dene
            Stove targetStove = closestInteractable.GetComponent<Stove>();
            if (targetStove != null && targetStove.IsOccupied)
            {
                Pot potToTake = targetStove.RemovePot();
                if (potToTake != null)
                {
                    PickUpItem(potToTake.gameObject);
                    Debug.Log("PlayerInteraction: Pot taken from stove.");
                }
                return;
            }

            // Masadan eşya almayı dene
            Table targetTable = closestInteractable.GetComponent<Table>();
            if (targetTable != null)
            {
                GameObject itemFromTable = targetTable.TryTakeItem();
                if (itemFromTable != null)
                {
                    PickUpItem(itemFromTable);
                    Debug.Log("PlayerInteraction: Item taken from table.");
                }
                return;
            }

            Debug.Log("PlayerInteraction: Cannot interact with this object.");
        }
    }

    public void PickUpItem(GameObject item)
    {
        heldItem = item;
        item.transform.SetParent(handPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        Debug.Log($"PlayerInteraction: Picked up {item.name}.");
    }

    public GameObject DropItem()
    {
        // Bu metot artık sadece PlayerInteraction içinde dahili olarak kullanılacak
        // ve eşyayı yere bırakmak yerine yok edecek.
        if (heldItem == null) return null;

        GameObject item = heldItem;
        heldItem = null;

        Debug.Log($"PlayerInteraction: Dropped {item.name} (disappeared).");
        Destroy(item);
        return null; // Artık GameObject döndürmüyor
    }

    public bool IsHoldingItem()
    {
        return heldItem != null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
