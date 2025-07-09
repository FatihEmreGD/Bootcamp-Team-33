using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Cooking/Recipe")]
public class Recipe : ScriptableObject
{
    public List<Ingredient> requiredIngredients;
    public float cookingTime = 20f;
    public float burnTime = 20f;
    public GameObject cookedDishPrefab;
}
