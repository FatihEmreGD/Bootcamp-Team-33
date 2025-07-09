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
            Debug.Log($"RecipeManager: Checking recipe: {recipe.name} (Required: {string.Join(", ", recipe.requiredIngredients.Select(i => i.ingredientName))})");

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