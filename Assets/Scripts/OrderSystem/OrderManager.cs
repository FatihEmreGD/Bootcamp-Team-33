using System.Collections.Generic;
using UnityEngine;
using System;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    public event Action<Order> OnOrderCreated;
    public event Action<string> OnOrderCompleted;
    public event Action<string> OnOrderFailed;

    [SerializeField] private List<MenuItemSO> possibleMenuItems;
    [SerializeField] private int maxItemsInOrder = 3;

    private List<Order> activeOrders = new List<Order>();
    private LevelSystem levelSystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (LevelSystemManager.Instance != null)
        {
            levelSystem = LevelSystemManager.Instance.levelSystem;
        }
        else
        {
            Debug.LogError("OrderManager needs a LevelSystemManager in the scene to function.");
        }
    }

    private void Update()
    {
        // Create a new order when 'P' is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            CreateNewOrder();
        }
    }

    public void CreateNewOrder()
    {
        if (possibleMenuItems == null || possibleMenuItems.Count == 0)
        {
            Debug.LogError("No possible menu items assigned in the OrderManager.");
            return;
        }

        List<MenuItemSO> orderItems = new List<MenuItemSO>();
        int itemCount = UnityEngine.Random.Range(1, maxItemsInOrder + 1);

        for (int i = 0; i < itemCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, possibleMenuItems.Count);
            orderItems.Add(possibleMenuItems[randomIndex]);
        }

        Order newOrder = new Order(orderItems);
        activeOrders.Add(newOrder);
        OnOrderCreated?.Invoke(newOrder);

        Debug.Log("New order created: " + newOrder.orderID);
    }

    // This method will be called by the player/gameplay logic
    public void DeliverOrder(string orderId, List<MenuItemSO> deliveredItems)
    {
        Order orderToDeliver = activeOrders.Find(order => order.orderID == orderId);

        if (orderToDeliver == null)
        {
            Debug.LogWarning("Attempted to deliver an order that doesn't exist: " + orderId);
            return;
        }

        if (deliveredItems.Count == orderToDeliver.items.Count)
        {
            Debug.Log("Order " + orderId + " delivered successfully!");
            levelSystem?.AddExperience(50); // Award 50 XP
            OnOrderCompleted?.Invoke(orderId);
            activeOrders.Remove(orderToDeliver);
        }
        else
        {
            Debug.Log("Order " + orderId + " delivery failed. Incorrect items.");
            levelSystem?.AddExperience(-10); // Penalty of 10 XP
            OnOrderFailed?.Invoke(orderId);
        }
    }
} 