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

        // if (closestInteractable != null)
        // {
        //     Debug.Log("Closest Interactable: " + closestInteractable.name);
        // }
        // else
        // {
        //     Debug.Log("No interactable in range.");
        // }
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
        if (!canMove) return; // If player cannot move, do not allow new interactions

        Debug.Log($"PlayerInteraction: Interact called. IsHoldingItem: {IsHoldingItem()}");
        // Eğer elde bir eşya varsa, öncelik onu bırakmak veya kullanmaktır.
        if (IsHoldingItem())
        {
            // Declare variables once at the top of this scope
            Pot heldPot = heldItem.GetComponent<Pot>();
            Plate heldPlate = heldItem.GetComponent<Plate>();
            IngredientItem heldIngredient = heldItem.GetComponent<IngredientItem>();

            // Bulaşık tezgahıyla etkileşim
            DishwashingStation targetDishwashingStation = closestInteractable.GetComponent<DishwashingStation>();
            if (targetDishwashingStation != null)
            {
                Debug.Log("PlayerInteraction: Interacting with Dishwashing Station.");
                if (heldPot != null)
                {
                    if (heldPot.currentState == Pot.PotState.Dirty || heldPot.ingredientsInside.Count > 0 || heldPot.cookedDish != null)
                    {
                        StartCoroutine(targetDishwashingStation.StartWashing(heldItem, this));
                        heldItem = null; // Item is now on the washing station
                        return;
                    }
                    else
                    {
                        Debug.Log("PlayerInteraction: Pot is not dirty or has no contents.");
                        return;
                    }
                }

                if (heldPlate != null)
                {
                    if (heldPlate.currentState == Plate.PlateState.Dirty || !heldPlate.IsEmpty())
                    {
                        StartCoroutine(targetDishwashingStation.StartWashing(heldItem, this));
                        heldItem = null; // Item is now on the washing station
                        return;
                    }
                    else
                    {
                        Debug.Log("PlayerInteraction: Plate is not dirty or empty.");
                        return;
                    }
                }

                Debug.LogWarning("PlayerInteraction: Cannot wash this item.");
                return;
            }

            // Çöp kutusuyla etkileşim
            TrashCan targetTrashCan = closestInteractable.GetComponent<TrashCan>();
            if (targetTrashCan != null)
            {
                Debug.Log("PlayerInteraction: Interacting with TrashCan.");
                if (heldPot != null)
                {
                    heldPot.EmptyAndDirty();
                    return;
                }

                if (heldPlate != null)
                {
                    if (!heldPlate.IsEmpty())
                    {
                        heldPlate.TakeDish(); // Take the dish, making the plate dirty
                        Debug.Log("PlayerInteraction: Dish taken from plate. Plate is now dirty.");
                    }
                    else
                    {
                        Debug.Log("PlayerInteraction: Plate is already empty.");
                    }
                    return;
                }

                if (heldIngredient != null)
                {
                    Destroy(heldItem);
                    heldItem = null;
                    Debug.Log("PlayerInteraction: Ingredient trashed.");
                    return;
                }
                Debug.LogWarning("PlayerInteraction: Cannot trash this item.");
                return;
            }

            // Elinde malzeme varsa
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
                        if (targetTable.TryPlaceItem(heldItem))
                        {
                            Debug.Log("PlayerInteraction: Item placed on table.");
                            heldItem = null; // Item is now on the table, so we are no longer holding it
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
                        Debug.Log("PlayerInteraction: Trying to place pot on stove. Current heldItem: " + (heldItem != null ? heldItem.name : "NULL"));
                        if (targetStove.PlacePot(heldPot))
                        {
                            Debug.Log("PlayerInteraction: Pot successfully placed on stove. Setting heldItem to null.");
                            
                            heldItem = null;
                            Debug.Log("PlayerInteraction: heldItem is now NULL.");
                            return;
                        }
                        else
                        {
                            Debug.Log("PlayerInteraction: Failed to place pot on stove. Pot remains in hand. Current heldItem: " + (heldItem != null ? heldItem.name : "NULL"));
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

            // Elinde tabak varsa
            if (heldPlate != null)
            {
                Debug.Log($"PlayerInteraction: Holding a plate.");
                if (closestInteractable != null)
                {
                    ServingTable targetServingTable = closestInteractable.GetComponent<ServingTable>();
                    if (targetServingTable != null)
                    {
                        if (targetServingTable.TryPlaceItem(heldItem))
                        {
                            heldItem = null;
                            Debug.Log("PlayerInteraction: Plate placed on serving table.");
                            return;
                        }
                        else
                        {
                            Debug.Log("PlayerInteraction: Failed to place plate on serving table.");
                            return;
                        }
                    }

                    Table targetTable = closestInteractable.GetComponent<Table>();
                    if (targetTable != null)
                    {
                        if (targetTable.TryPlaceItem(heldItem))
                        {
                            heldItem = null;
                            return;
                        }
                    }

                    Stove targetStove = closestInteractable.GetComponent<Stove>();
                    if (targetStove != null)
                    {
                        targetStove.TryServeFood(heldPlate);
                        return;
                    }

                    Pot targetPot = closestInteractable.GetComponent<Pot>();
                    if (targetPot != null)
                    {
                        if (targetPot.currentState == Pot.PotState.Cooked)
                        {
                            if (heldPlate.IsEmpty())
                            {
                                Recipe servedDish = targetPot.ServeDish();
                                if (servedDish != null)
                                {
                                    heldPlate.TryPlaceDish(servedDish);
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
                        return;
                    }
                }
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

            // Bulaşık tezgahıyla etkileşim
            DishwashingStation targetDishwashingStation = closestInteractable.GetComponent<DishwashingStation>();
            if (targetDishwashingStation != null)
            {
                Debug.Log("PlayerInteraction: Interacting with Dishwashing Station. Nothing to pick up.");
                return;
            }

            // Malzeme kaynağından almayı dene
            IngredientSource ingredientSource = closestInteractable.GetComponent<IngredientSource>();
            if (ingredientSource != null)
            {
                Ingredient ingredient = ingredientSource.TakeIngredient();
                if (ingredient != null)
                {
                    GameObject newIngredientItemGO = Instantiate(ingredient.ingredientPrefab);
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

            // Servis masasından eşya almayı dene
            ServingTable targetServingTable = closestInteractable.GetComponent<ServingTable>();
            if (targetServingTable != null)
            {
                GameObject itemFromServingTable = targetServingTable.TryTakeItem();
                if (itemFromServingTable != null)
                {
                    PickUpItem(itemFromServingTable);
                    Debug.Log("PlayerInteraction: Item taken from serving table.");
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

    private bool canMove = true;
    public JoystickPlayerExample playerMovementScript; // Reference to your player movement script
    private bool _isWashingInterruptionRequested = false; // New flag for washing interruption

    public void SetPlayerMovement(bool canMove)
    {
        this.canMove = canMove;
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = canMove;
        }
        else
        {
            Debug.LogWarning("PlayerMovementScript reference is not set in PlayerInteraction.");
        }
        Debug.Log($"Player movement set to: {canMove}");
    }

    // This method will be called by the UI button
    public void OnUniversalInteractionButtonPress()
    {
        Debug.Log("PlayerInteraction: Universal Interaction Button Pressed.");
        if (!canMove) // If player is currently in a washing state (cannot move)
        {
            _isWashingInterruptionRequested = true; // Request interruption
            Debug.Log("PlayerInteraction: Washing interruption requested.");
        }
        else
        {
            // If not in a washing state, proceed with normal interaction
            Interact();
        }
    }

    // Called by DishwashingStation to check if interruption is requested
    public bool CheckWashingInterruptionRequest()
    {
        if (_isWashingInterruptionRequested)
        {
            _isWashingInterruptionRequested = false; // Reset the flag after checking
            return true;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}