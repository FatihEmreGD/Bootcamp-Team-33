using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class OrderTicketUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI orderItemsText;
    private Order associatedOrder;

    public void SetOrder(Order order)
    {
        associatedOrder = order;
        UpdateOrderText();
    }

    private void UpdateOrderText()
    {
        if (associatedOrder == null || associatedOrder.items == null) return;

        StringBuilder sb = new StringBuilder();
        Dictionary<string, int> itemCounts = new Dictionary<string, int>();

        foreach (var item in associatedOrder.items)
        {
            if (itemCounts.ContainsKey(item.itemName))
            {
                itemCounts[item.itemName]++;
            }
            else
            {
                itemCounts.Add(item.itemName, 1);
            }
        }

        // Build the string
        foreach (var entry in itemCounts)
        {
            sb.AppendLine($"{entry.Value}x {entry.Key}");
        }

        orderItemsText.text = sb.ToString();
    }
} 