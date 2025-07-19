using UnityEngine;

public class Plate : MonoBehaviour
{
    [field: SerializeField]
    public Recipe heldDish { get; private set; }

    public enum PlateState
    {
        Empty,
        HasDish,
        Dirty
    }

    public PlateState currentState = PlateState.Empty;

    public bool IsEmpty()
    {
        return heldDish == null && currentState == PlateState.Empty;
    }

    public bool TryPlaceDish(Recipe dish)
    {
        if (!IsEmpty() || dish == null)
        {
            return false;
        }

        heldDish = dish;
        currentState = PlateState.HasDish;
        Debug.Log($"Placed {dish.name} on the plate.");
        // Activate a visual representation of the dish on the plate here
        return true;
    }

    public Recipe TakeDish()
    {
        if (IsEmpty())
        {
            return null;
        }

        Recipe dish = heldDish;
        heldDish = null;
        currentState = PlateState.Dirty; // Plate becomes dirty after dish is taken
        Debug.Log($"Took {dish.name} from the plate. Plate is now dirty.");
        // Deactivate the visual representation of the dish on the plate here
        return dish;
    }

    public void CleanPlate()
    {
        if (currentState == PlateState.Dirty)
        {
            heldDish = null;
            currentState = PlateState.Empty;
            Debug.Log("Plate has been cleaned.");
        }
    }
}
