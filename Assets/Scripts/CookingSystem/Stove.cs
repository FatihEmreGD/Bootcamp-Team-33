using UnityEngine;
using System.Collections;
using System.Linq;

public class Stove : MonoBehaviour
{
    public Transform potPlacementPoint;
    public Pot currentPot;
    private Recipe currentRecipe;
    private Coroutine cookingCoroutine;

    public bool IsOccupied => currentPot != null;

    public bool PlacePot(Pot pot)
    {
        if (IsOccupied)
        {
            Debug.Log("Stove is already occupied. Cannot place pot.");
            return false;
        }

        currentPot = pot;
        pot.transform.SetParent(potPlacementPoint);
        pot.transform.localPosition = Vector3.zero;
        pot.transform.localRotation = Quaternion.identity;
        
        Debug.Log("Pot placed on stove.");

        // Check if the pot was previously cooking and resume
        if (currentPot.wasCooking && currentPot.activeRecipe != null)
        {
            // Verify if the ingredients still match the active recipe
            if (RecipeManager.Instance != null && RecipeManager.Instance.GetMatchingRecipe(currentPot.ingredientsInside) == currentPot.activeRecipe)
            {
                currentRecipe = currentPot.activeRecipe;
                StartCooking(currentPot.currentCookingProgress); // Resume cooking
                Debug.Log($"Resuming cooking for {currentRecipe.name} from {currentPot.currentCookingProgress:F2} seconds.");
            }
            else
            {
                Debug.Log("Pot was cooking but ingredients no longer match the active recipe. Starting new check.");
                CheckRecipeAndStartCookingPublic();
            }
        }
        else
        {
            CheckRecipeAndStartCookingPublic(); // Start new cooking check
        }
        return true;
    }

    public Pot RemovePot()
    {
        if (!IsOccupied)
        {
            Debug.Log("Stove has no pot to remove.");
            return null;
        }

        Pot potToReturn = currentPot;
        
        // Save cooking progress before removing
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
            cookingCoroutine = null;
            potToReturn.wasCooking = true;
            potToReturn.activeRecipe = currentRecipe;
            Debug.Log($"Cooking stopped and progress saved for {potToReturn.name} at {potToReturn.currentCookingProgress:F2} seconds.");
        }
        else
        {
            potToReturn.wasCooking = false;
            potToReturn.currentCookingProgress = 0f;
            potToReturn.activeRecipe = null;
        }

        currentPot = null;
        currentRecipe = null; // Clear current recipe for the stove

        Debug.Log("Pot removed from the stove.");
        return potToReturn;
    }

    // Public method to be called from PlayerInteraction
    public void CheckRecipeAndStartCookingPublic()
    {
        Debug.Log("Stove: CheckRecipeAndStartCookingPublic called.");
        CheckRecipeAndStartCooking();
    }

    private void CheckRecipeAndStartCooking()
    {
        if (!IsOccupied || currentPot.currentState != Pot.PotState.HasIngredients)
        {
            Debug.Log("Stove: Cannot start cooking: Stove not occupied or pot has no ingredients.");
            return;
        }

        if (RecipeManager.Instance == null)
        {
            Debug.LogError("Stove: RecipeManager not found in scene! Cannot check recipes.");
            return;
        }

        Debug.Log("Stove: Ingredients in pot: " + string.Join(", ", currentPot.ingredientsInside.Select(i => i.ingredientName)));

        Recipe foundRecipe = RecipeManager.Instance.GetMatchingRecipe(currentPot.ingredientsInside);
        
        if (foundRecipe != null)
        {
            currentRecipe = foundRecipe;
            StartCooking(); // Start from beginning if new recipe or not previously cooking
        }
        else
        {
            Debug.Log("Stove: No matching recipe found for the ingredients in the pot.");
            // If no recipe found, ensure cooking is stopped
            if (cookingCoroutine != null)
            {
                StopCoroutine(cookingCoroutine);
                cookingCoroutine = null;
                currentPot.currentState = Pot.PotState.HasIngredients; // Reset state
                currentPot.wasCooking = false;
                currentPot.currentCookingProgress = 0f;
                currentPot.activeRecipe = null;
                Debug.Log("Cooking stopped due to no matching recipe.");
            }
        }
    }

    private void StartCooking(float startTime = 0f)
    {
        if (currentRecipe != null)
        {
            Debug.Log("Stove: Recipe found: " + currentRecipe.name + ". Starting to cook.");
            currentPot.currentState = Pot.PotState.Cooking;
            if (cookingCoroutine != null)
            {
                StopCoroutine(cookingCoroutine);
            }
            cookingCoroutine = StartCoroutine(CookingProcess(currentRecipe.cookingTime, currentRecipe.burnTime, startTime));
        }
    }

    private IEnumerator CookingProcess(float cookingTime, float burnTime, float startTime)
    {
        float timer = startTime;
        // Cooking phase
        while (timer < cookingTime)
        {
            timer += Time.deltaTime;
            currentPot.currentCookingProgress = timer; // Save current progress
            // Update UI progress bar here
            Debug.Log($"Stove: Cooking progress: {timer / cookingTime * 100:F0}%");
            yield return null;
        }

        currentPot.currentState = Pot.PotState.Cooked;
        Debug.Log("Stove: Food is cooked!");
        currentPot.currentCookingProgress = cookingTime; // Ensure it's at max

        // Burn phase
        float burnTimer = 0f;
        while (burnTimer < burnTime)
        {
            burnTimer += Time.deltaTime;
            // Update UI progress bar here (e.g., change color to red)
            Debug.Log($"Stove: Burn progress: {burnTimer / burnTime * 100:F0}%");
            yield return null;
        }

        currentPot.currentState = Pot.PotState.Burnt;
        Debug.Log("Stove: Food is burnt!");
        // Handle burnt food (e.g., visual change, cannot be served)
    }
}