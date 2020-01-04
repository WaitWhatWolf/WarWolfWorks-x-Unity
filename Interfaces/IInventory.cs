using System;
using System.Collections.Generic;
using WarWolfWorks.EntitiesSystem.Itemization;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Interface for implementing an inventory.
    /// </summary>
    public interface IInventory
    {
        /// <summary>
        /// Returns the item at given index.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Item this[int key] { get; }
        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool AddItem(Item item);
        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool RemoveItem(Item item);
        /// <summary>
        /// Removes an item from the inventory based on index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool RemoveItem(int index);
        /// <summary>
        /// Removes an item from inventory based on index and returns it in out.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool RemoveItem(int index, out Item item);
        /// <summary>
        /// Returns the first item under the given name.
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        Item GetItem(string itemName);
        /// <summary>
        /// Returns the first item under the given index.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        Item GetItem(int itemID);
        /// <summary>
        /// Returns an item based on match given.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        Item GetItem(Predicate<Item> match);
        /// <summary>
        /// Returns the index of the item given inside the inventory; If no item was found, it will return -1.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int GetItemIndex(Item item);
        /// <summary>
        /// Invoked when an item is successfully added to the inventory.
        /// </summary>
        event Action<Item> OnItemAdded;
        /// <summary>
        /// Invoked when an item is successfully removed from the inventory.
        /// </summary>
        event Action<Item, int> OnItemRemoved;
        /// <summary>
        /// Returns all items inside the inventory.
        /// </summary>
        /// <param name="clone"></param>
        /// <returns></returns>
        IEnumerable<Item> GetAllItems(bool clone = false);
        /// <summary>
        /// Returns the amount of items currently inside the inventory.
        /// </summary>
        int ItemsCount { get; }
    }
}
