using UnityEngine;
using System.Collections.Generic;

public class Pot : MonoBehaviour
{
    public List<Ingredient> ingredientsInside = new List<Ingredient>();
    
    public enum PotState
    {
        Empty,
        HasIngredients,
        Cooking,
        Cooked,
        Burnt
    }

    public PotState currentState = PotState.Empty;

    // New variables for cooking progress
    public float currentCookingProgress = 0f;
    public bool wasCooking = false;
    public Recipe activeRecipe = null;

    public bool AddIngredient(Ingredient ingredient)
    {
        if (currentState == PotState.Cooking || currentState == PotState.Cooked || currentState == PotState.Burnt)
        {
            Debug.LogWarning($"Pot: Cannot add ingredient {ingredient.ingredientName}. Pot is {currentState}.");
            return false;
        }

        ingredientsInside.Add(ingredient);
        currentState = PotState.HasIngredients;
        Debug.Log("Added: " + ingredient.name);
        // Update UI here in the future
        return true;
    }

    public void ClearPot()
    {
        ingredientsInside.Clear();
        currentState = PotState.Empty;
        currentCookingProgress = 0f;
        wasCooking = false;
        activeRecipe = null;
        Debug.Log("Pot is now empty.");
    }
}