using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimpleInventory : MonoBehaviour
{
    public static SimpleInventory Instance;

    [Header("Settings")]
    public int maxSlots = 20;

    private List<InventoryItem> items = new List<InventoryItem>();

    // Event khi inventory thay đổi
    public System.Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadInventory();
    }

    // Thêm item
    public bool AddItem(ShopItem item)
    {
        // Kiểm tra xem item đã có chưa (stack)
        InventoryItem existingItem = items.FirstOrDefault(i => i.itemID == item.itemID);

        if (existingItem != null)
        {
            existingItem.quantity++;
        }
        else
        {
            // Kiểm tra chỗ trống
            if (items.Count >= maxSlots)
            {
                Debug.Log("Inventory đầy!");
                return false;
            }

            items.Add(new InventoryItem(item, 1));
        }

        OnInventoryChanged?.Invoke();
        SaveInventory();
        return true;
    }

    // Xóa item
    public bool RemoveItem(string itemID, int quantity = 1)
    {
        InventoryItem item = items.FirstOrDefault(i => i.itemID == itemID);

        if (item == null) return false;

        item.quantity -= quantity;

        if (item.quantity <= 0)
        {
            items.Remove(item);
        }

        OnInventoryChanged?.Invoke();
        SaveInventory();
        return true;
    }

    // Lấy danh sách items
    public List<InventoryItem> GetItems()
    {
        return new List<InventoryItem>(items);
    }

    // Kiểm tra có item không
    public bool HasItem(string itemID)
    {
        return items.Any(i => i.itemID == itemID);
    }

    // Lấy số lượng item
    public int GetItemQuantity(string itemID)
    {
        InventoryItem item = items.FirstOrDefault(i => i.itemID == itemID);
        return item?.quantity ?? 0;
    }

    // Lưu inventory
    private void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData();

        foreach (var item in items)
        {
            saveData.itemIDs.Add(item.itemID);
            saveData.quantities.Add(item.quantity);
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("Inventory", json);
        PlayerPrefs.Save();
    }

    // Load inventory
    private void LoadInventory()
    {
        string json = PlayerPrefs.GetString("Inventory", "");

        if (string.IsNullOrEmpty(json)) return;

        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        items.Clear();

        for (int i = 0; i < saveData.itemIDs.Count; i++)
        {
            // Cần rebuild item từ ShopData
            // Tạm thời tạo item trống, sẽ rebuild sau
            items.Add(new InventoryItem(saveData.itemIDs[i], saveData.quantities[i]));
        }

        OnInventoryChanged?.Invoke();
    }

    // Rebuild items từ ShopData (gọi sau khi ShopData load)
    public void RebuildItemsFromData(ShopData shopData)
    {
        if (shopData == null) return;

        foreach (var invItem in items)
        {
            ShopItemData data = shopData.GetItemByID(invItem.itemID);
            if (data != null)
            {
                invItem.item = data.ToShopItem();
            }
        }
    }

    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();
        SaveInventory();
    }
}

// Class lưu item trong inventory
[System.Serializable]
public class InventoryItem
{
    public string itemID;
    public ShopItem item;
    public int quantity;

    public InventoryItem(ShopItem shopItem, int qty)
    {
        itemID = shopItem.itemID;
        item = shopItem;
        quantity = qty;
    }

    // Constructor cho load
    public InventoryItem(string id, int qty)
    {
        itemID = id;
        quantity = qty;
    }
}

// Class để save/load
[System.Serializable]
public class InventorySaveData
{
    public List<string> itemIDs = new List<string>();
    public List<int> quantities = new List<int>();
}