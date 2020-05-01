using System;
using System.Collections.Generic;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Interface for implementing an inventory.
    /// </summary>
    public interface IInventory<T> where T : IItem
    {
        /// <summary>
        /// Returns the item at given index.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T this[int key] { get; }
        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool AddItem(T item);
        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool RemoveItem(T item);
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
        bool RemoveItem(int index, out T item);
        /*/// <summary>
        /// Removes stacks from an item; If removed amount makes that item go to 0, it will also remove it.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        bool RemoveStacks(T item, int amount);
        /// <summary>
        /// Removes stacks from an item and returns the successfully removed amount; 
        /// If removed amount makes that item go to 0, it will also remove it.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <param name="removed"></param>
        /// <returns></returns>
        bool RemoveStacks(T item, int amount, out int removed);
        /// <summary>
        /// Removes stacks from an item specified by it's residing index; If removed amount makes that item go to 0, it will also remove it.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        bool RemoveStacks(int index, int amount);
        /// <summary>
        /// Removes stacks from an item specified by it's residing index and returns the successfully removed amount; 
        /// If removed amount makes that item go to 0, it will also remove it.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="amount"></param>
        /// <param name="removed"></param>
        /// <returns></returns>
        bool RemoveStacks(int index, int amount, out int removed);*/
        /// <summary>
        /// Returns the first item under the given name.
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        T GetItem(string itemName);
        /// <summary>
        /// Returns the first item under the given index.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        T GetItem(int itemID);
        /// <summary>
        /// Returns an item based on match given.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        T GetItem(Predicate<T> match);
        /// <summary>
        /// Returns the index of the item given inside the inventory; If no item was found, it will return -1.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int GetItemIndex(T item);
        /// <summary>
        /// Invoked when an item is successfully added to the inventory.
        /// </summary>
        event Action<T> OnItemAdded;
        /// <summary>
        /// Invoked when an item is successfully removed from the inventory.
        /// </summary>
        event Action<T, int> OnItemRemoved;
        /*/// <summary>
        /// Invoked when an item's stacks get added; Int is stacks before addition.
        /// </summary>
        event Action<T, int> OnItemStacksAdded;
        /// <summary>
        /// Invoked when an item's stacks get removed; Int is stacks before reduction.
        /// </summary>
        event Action<T, int> OnItemStacksRemoved;*/
        /// <summary>
        /// Returns all items inside the inventory.
        /// </summary>
        /// <param name="clone"></param>
        /// <returns></returns>
        IEnumerable<T> GetAllItems(bool clone = false);
        /// <summary>
        /// Returns the amount of items currently inside the inventory.
        /// </summary>
        int ItemsCount { get; }
    }
}
