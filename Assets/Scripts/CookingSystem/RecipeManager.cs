using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    public List<Recipe> allRecipes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public Recipe GetMatchingRecipe(List<Ingredient> ingredients)
    {
        Debug.Log("RecipeManager: Attempting to find matching recipe for ingredients: " + string.Join(", ", ingredients.Select(i => i.ingredientName)));

        // Check all recipes
        foreach (Recipe recipe in allRecipes)
        {
            if (recipe == null) // Added null check for recipe itself
            {
                Debug.LogWarning("RecipeManager: Found a null recipe in allRecipes list. Skipping.");
                continue;
            }

            // Safely get ingredient names for logging
            string requiredIngredientsNames = "N/A";
            if (recipe.requiredIngredients != null)
            {
                requiredIngredientsNames = string.Join(", ", recipe.requiredIngredients.Select(i => i != null ? i.ingredientName : "NULL"));
            }
            Debug.Log($"RecipeManager: Checking recipe: {recipe.name} (Required: {requiredIngredientsNames})");

            // Check if requiredIngredients list is null or empty
            if (recipe.requiredIngredients == null || recipe.requiredIngredients.Count == 0)
            {
                Debug.Log($"RecipeManager: Recipe {recipe.name} has no required ingredients or requiredIngredients list is null. Skipping.");
                continue;
            }

            // Check if ingredient counts match
            if (recipe.requiredIngredients.Count != ingredients.Count)
            {
                Debug.Log($"RecipeManager: Count mismatch for {recipe.name}. Required: {recipe.requiredIngredients.Count}, In Pot: {ingredients.Count}");
                continue;
            }

            // Check if all required ingredients are present in the pot
            bool allIngredientsMatch = true;
            foreach (Ingredient requiredIngredient in recipe.requiredIngredients)
            {
                if (requiredIngredient == null) // Added null check for individual ingredient
                {
                    Debug.LogWarning($"RecipeManager: Found a null ingredient in recipe {recipe.name}. Skipping this recipe.");
                    allIngredientsMatch = false;
                    break;
                }

                if (!ingredients.Contains(requiredIngredient))
                {
                    Debug.Log($"RecipeManager: Missing ingredient {requiredIngredient.ingredientName} for {recipe.name}.");
                    allIngredientsMatch = false;
                    break;
                }
            }

            if (allIngredientsMatch)
            {
                Debug.Log($"RecipeManager: Found matching recipe: {recipe.name}");
                return recipe;
            }
        }
        Debug.Log("RecipeManager: No matching recipe found.");
        return null; // No matching recipe found
    }
}