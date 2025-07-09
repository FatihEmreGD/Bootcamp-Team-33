using UnityEngine;

public class IngredientSource : MonoBehaviour
{
    public Ingredient ingredientToGive;

    // This will be called by the player interaction script later
    public Ingredient TakeIngredient()
    {
        if (ingredientToGive != null)
        {
            Debug.Log("Player took: " + ingredientToGive.ingredientName);
            // In a real game, you might decrement a count here
            return ingredientToGive;
        }
        Debug.LogWarning("This source has no ingredient assigned!");
        return null;
    }

    // Tries to place an ingredient back into the source
    public bool TryPlaceIngredient(Ingredient ingredientToPlace)
    {
        if (ingredientToPlace != null && ingredientToPlace == ingredientToGive)
        {
            Debug.Log("Player placed: " + ingredientToPlace.ingredientName + " back into " + gameObject.name);
            // In a real game, you might increment a count here
            return true;
        }
        Debug.Log("Cannot place " + (ingredientToPlace != null ? ingredientToPlace.ingredientName : "nothing") + " here.");
        return false;
    }
}
