using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public static Inventory Instance { get; private set; }

    public event EventHandler OnItemListChanged;

    private List<Item> itemList;
    private Action<Item> useItemAction;

    public Inventory(Action<Item> useItemAction)
    {
        this.useItemAction = useItemAction;
        itemList = new List<Item>();

        //AddItem(new Item { itemType = Item.ItemType.BasicSword, amount = 1 });
        //AddItem(new Item { itemType = Item.ItemType.Potion, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.Table, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.WeaponHanger, amount = 1 });
    }

    public void AddItem(Item item)
    {
        if (item.isStackable())
        {
            bool itemAlreadyInInventory = false;
            foreach (Item inventoryItem in itemList) {
                if(inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount += item.amount;
                    itemAlreadyInInventory = true;
                }
            }
            if (!itemAlreadyInInventory)
            {
                itemList.Add(item);
            }
        } 
        else
        {
            itemList.Add(item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveItem(Item item)
    {
        if (item.isStackable())
        {
            Item itemInInventory = null;
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount--;
                    itemInInventory = inventoryItem;
                }
            }
            if (itemInInventory != null && itemInInventory.amount <= 0)
            {
                itemList.Remove(itemInInventory);
            }
        }
        else
        {
            itemList.Remove(item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UseItem(Item item)
    {
        useItemAction(item);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public int GetAmount(Item.ItemType findingItemType)
    {
        int a = 0;
        foreach (Item item in itemList)
        {
            if (item.itemType == findingItemType)
            {
                a = item.amount;
            }
        }
        return a;
    }
}
