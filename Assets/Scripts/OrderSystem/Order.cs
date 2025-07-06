using System.Collections.Generic;

[System.Serializable]
public class Order
{
    public string orderID;
    public List<MenuItemSO> items;

    public Order(List<MenuItemSO> requiredItems)
    {
        orderID = System.Guid.NewGuid().ToString();
        items = requiredItems;
    }
} 