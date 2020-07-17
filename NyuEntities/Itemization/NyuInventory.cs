using System;
using WarWolfWorks.Utility;
using UnityEngine;
using WarWolfWorks.Interfaces;
using System.Collections.Generic;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.Itemization
{
    /// <summary>
    /// Advanced Fixed-Size Inventory for your Fixed-Size Inventory needs.
    /// </summary>
    public abstract class NyuInventory<T> : NyuComponent, IInventory<T>, INyuPreAwake where T : NyuItem
	{
        [FormerlySerializedAs("inventorySize"), SerializeField]
        private int s_InventorySize;
        /// <summary>
        /// The max capacity of the inventory. (Settable only through the inspector)
        /// </summary>
        public int InventorySize => s_InventorySize;

        private T[] ns_Items;
        /// <summary>
        /// Returns the count of all inventory slots assigned with an Item.
        /// </summary>
        public int ItemsCount => Array.FindAll(ns_Items, i => i != null).Length;
        /// <summary>
        /// Gets the first slot index that doesn't contain an item.
        /// </summary>
        public int FirstEmptyIndex() => ns_Items.GetEmptyIndex();
        /// <summary>
        /// Gets the first slot index that doesn't contain an item starting from specified index.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public int FirstEmptyIndex(int from) => ns_Items.GetEmptyIndex(from);
        /// <summary>
        /// Gets the first slot index that doesn't contain an item starting from specified index with specific count.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int FirstEmptyIndex(int from, int count) => ns_Items.GetEmptyIndex(from, count);
        /// <summary>
        /// Gets the first slot index that within given range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public int FirstEmptyIndex(IntRange range) => ns_Items.GetEmptyIndex(range);
        /// <summary>
        /// Returns all items held by this inventory.
        /// </summary>
        /// <param name="clone">If true, it will return a list of cloned Items, instead of the real ones.</param>
        /// <returns></returns>
        public IEnumerable<T> GetAllItems(bool clone = false) => clone ? (T[])ns_Items.Clone() : ns_Items;
        /// <summary>
        /// Gets the first item inside the Inventory with the given name.
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public T GetItem(string itemName) => ns_Items.Find(it => it.Name.Equals(itemName));
        /// <summary>
        /// Gets the first item inside the Inventory with the given ID.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public T GetItem(int itemID) => ns_Items.Find(it => it.GetID() == itemID);
        /// <summary>
        /// Gets an Item through custom predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public T GetItem(Predicate<T> match) => ns_Items.Find(match);
        
        /// <summary>
        /// Gets the index of a given item inside the inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItemIndex(T item) => ns_Items.FindIndex(it => item == it);

        /// <summary>
        /// Invoked when an item is successfully added to the inventory.
        /// </summary>
        public event Action<T> OnItemAdded;
        /// <summary>
        /// Invoked when an item is successfully removed from the inventory. Int indicates it's index in the inventory.
        /// </summary>
        public event Action<T, int> OnItemRemoved;

        /// <summary>
        /// Returns the item under specified index.
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="IndexOutOfRangeException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="InvalidCastException"/>
        /// <returns></returns>
        public T this[int key] { get => ns_Items[key]; }

        private bool InventoryInitiated { get; set; } = false;

        void INyuPreAwake.NyuPreAwake()
        {
            if (!InventoryInitiated)
            {
                ns_Items = new T[InventorySize];
                InventoryInitiated = true;
            }
        }

        /// <summary>
        /// Adds an Item to the first empty slot.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddItem(T item)
        {
            if (item == null)
                return false;

            int emptyIndex = ns_Items.GetEmptyIndex();
            if (emptyIndex == -1)
                return false;

            ns_Items[emptyIndex] = item;
            OnItemAdded?.Invoke(item);
            item.ManipulateFromInventory(this, true);

            return true;
        }

        /// <summary>
        /// Adds an Item to the specified index.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toIndex"></param>
        /// <returns></returns>
        public bool AddItem(T item, int toIndex)
        {
            if (ns_Items[toIndex] || toIndex >= ns_Items.Length || item == null)
                return false;

            ns_Items[toIndex] = item;
            OnItemAdded?.Invoke(item);
            item.ManipulateFromInventory(this, true);

            return true;
        }
        /// <summary>
        /// Adds an item to specified index regardless of if the index already has an item.
        /// If the an Item existed at specified index, it will be assigned to replacedItem.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toIndex"></param>
        /// <param name="replacedItem"></param>
        /// <returns></returns>
        public bool AddItem(T item, int toIndex, out T replacedItem)
        {
            if (toIndex < 0 || toIndex >= ns_Items.Length || item == null)
            {
                replacedItem = null;
                return false;
            }

            RemoveItem(toIndex, out replacedItem);
            ns_Items[toIndex] = item;
            OnItemAdded?.Invoke(item);
            item.ManipulateFromInventory(this, true);

            return true;
        }

        /// <summary>
        /// Removes the specified item from Inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveItem(T item)
        {
            int index = ns_Items.FindIndex(it => item == it);

            if (index == -1) 
                return false;

            T toRemove = ns_Items[index];

            if (!toRemove)
                return false;

            ns_Items[index] = null;
            OnItemRemoved?.Invoke(toRemove, index);
            toRemove.ManipulateFromInventory(this, false);

            return toRemove;
        }

        /// <summary>
        /// Removes an Item inside the inventory under given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveItem(int index)
        {
            T toRemove = ns_Items[index];

            if (!toRemove)
                return false;

            ns_Items[index] = null;
            OnItemRemoved?.Invoke(toRemove, index);
            toRemove.ManipulateFromInventory(this, false);

            return toRemove;
        }

        /// <summary>
        /// Removes an item from this inventory based on index and returns it in out.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveItem(int index, out T item)
        {
            T toRemove = ns_Items[index];

            if (!toRemove)
            {
                item = null;
                return false;
            }

            ns_Items[index] = null;
            OnItemRemoved?.Invoke(toRemove, index);
            toRemove.ManipulateFromInventory(this, false);
            item = toRemove;
            return true;
        }
    }
}