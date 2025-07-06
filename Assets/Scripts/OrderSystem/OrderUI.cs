using System.Collections.Generic;
using UnityEngine;

public class OrderUI : MonoBehaviour
{
    [SerializeField] private GameObject orderTicketPrefab;
    [SerializeField] private Transform ordersContainer;

    private Dictionary<string, GameObject> activeOrderTickets = new Dictionary<string, GameObject>();

    private void Start()
    {
        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.OnOrderCreated += HandleOrderCreated;
            OrderManager.Instance.OnOrderCompleted += HandleOrderCompleted;
            OrderManager.Instance.OnOrderFailed += HandleOrderFailed;
        }
    }

    private void OnDestroy()
    {
        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.OnOrderCreated -= HandleOrderCreated;
            OrderManager.Instance.OnOrderCompleted -= HandleOrderCompleted;
            OrderManager.Instance.OnOrderFailed -= HandleOrderFailed;
        }
    }

    private void HandleOrderCreated(Order newOrder)
    {
        if (orderTicketPrefab == null || ordersContainer == null)
        {
            Debug.LogError("Prefab or container not set in OrderUI.");
            return;
        }

        GameObject ticketObject = Instantiate(orderTicketPrefab, ordersContainer);
        ticketObject.name = "OrderTicket_" + newOrder.orderID;
        
        OrderTicketUI ticketUI = ticketObject.GetComponent<OrderTicketUI>();
        if (ticketUI != null)
        {
            ticketUI.SetOrder(newOrder);
            activeOrderTickets.Add(newOrder.orderID, ticketObject);
        }
    }

    private void HandleOrderCompleted(string orderId)
    {
        if (activeOrderTickets.ContainsKey(orderId))
        {
            Destroy(activeOrderTickets[orderId]);
            activeOrderTickets.Remove(orderId);
        }
    }

    private void HandleOrderFailed(string orderId)
    {
        HandleOrderCompleted(orderId);
    }
} 