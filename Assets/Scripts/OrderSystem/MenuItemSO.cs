using UnityEngine;

[CreateAssetMenu(fileName = "NewMenuItem", menuName = "Order System/Menu Item")]
public class MenuItemSO : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite itemIcon;
} 