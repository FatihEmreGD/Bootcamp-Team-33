using UnityEngine;
using System.Collections.Generic;

public class Pot : MonoBehaviour
{
    public List<Ingredient> ingredientsInside = new List<Ingredient>();
    public Recipe cookedDish { get; private set; }

    public enum PotState
    {
        Empty,
        HasIngredients,
        Cooking,
        Cooked,
        Burnt,
        Dirty
    }

    public PotState currentState = PotState.Empty;

    // New variables for cooking progress
    public float currentCookingProgress = 0f;
    public bool wasCooking = false;
    public Recipe activeRecipe = null;

    public bool AddIngredient(Ingredient ingredient)
    {
        if (currentState == PotState.Cooking || currentState == PotState.Cooked || currentState == PotState.Burnt || currentState == PotState.Dirty)
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

    public void SetCooked(Recipe recipe)
    {
        cookedDish = recipe;
        currentState = PotState.Cooked;
    }

    public Recipe ServeDish()
    {
        if (currentState != PotState.Cooked)
        {
            return null;
        }

        Recipe dishToServe = cookedDish;
        ClearPot();
        currentState = PotState.Dirty;
        Debug.Log("Dish served. Pot is now dirty.");
        return dishToServe;
    }

    public void ClearPot()
    {
        ingredientsInside.Clear();
        cookedDish = null;
        currentState = PotState.Empty;
        currentCookingProgress = 0f;
        wasCooking = false;
        activeRecipe = null;
        Debug.Log("Pot is now empty.");
    }

    public void CleanPot()
    {
        if (currentState == PotState.Dirty)
        {
            ClearPot();
            Debug.Log("Pot has been cleaned.");
        }
    }

    public void EmptyAndDirty()
    {
        if (ingredientsInside.Count > 0 || cookedDish != null)
        {
            ClearPot();
            currentState = PotState.Dirty;
            Debug.Log("Pot emptied and is now dirty.");
        }
        else
        {
            Debug.Log("Pot is already empty.");
        }
    }
}