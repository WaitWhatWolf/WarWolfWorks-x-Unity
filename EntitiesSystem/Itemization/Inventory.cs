using System;
using WarWolfWorks.Utility;
using UnityEngine;
using System.Linq;
using WarWolfWorks.Interfaces;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace WarWolfWorks.EntitiesSystem.Itemization
{
    /// <summary>
    /// Advanced Fixed-Size Inventory for your Fixed-Size Inventory needs.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public class Inventory : EntityComponent, IInventory<Item>
	{
        [FormerlySerializedAs("inventorySize"), SerializeField]
        private int s_InventorySize;
        /// <summary>
        /// The max capacity of the inventory. (Settable only through the inspector)
        /// </summary>
        public int InventorySize => s_InventorySize;

        private Item[] Items;
        /// <summary>
        /// Returns the count of all inventory slots assigned with an Item.
        /// </summary>
        public int ItemsCount => Array.FindAll(Items, i => i != null).Length;
        /// <summary>
        /// Gets the first slot index that doesn't contain an item.
        /// </summary>
        public int FirstEmptyIndex() => Items.GetEmptyIndex();
        /// <summary>
        /// Gets the first slot index that doesn't contain an item starting from specified index.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public int FirstEmptyIndex(int from) => Items.GetEmptyIndex(from);
        /// <summary>
        /// Gets the first slot index that doesn't contain an item starting from specified index with specific count.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int FirstEmptyIndex(int from, int count) => Items.GetEmptyIndex(from, count);
        /// <summary>
        /// Gets the first slot index that within given range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public int FirstEmptyIndex(IntRange range) => Items.GetEmptyIndex(range);
        /// <summary>
        /// Returns all items held by this inventory.
        /// </summary>
        /// <param name="clone">If true, it will return a list of cloned Items, instead of the real ones.</param>
        /// <returns></returns>
        public IEnumerable<Item> GetAllItems(bool clone = false) => clone ? (Item[])Items.Clone() : Items;
        /// <summary>
        /// Gets the first item inside the Inventory with the given name.
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public Item GetItem(string itemName) => Items.Find(it => it.Name.Equals(itemName));
        /// <summary>
        /// Gets the first item inside the Inventory with the given ID.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public Item GetItem(int itemID) => Items.Find(it => it.GetID() == itemID);
        /// <summary>
        /// Gets an Item through custom predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public Item GetItem(Predicate<Item> match) => Items.Find(match);
        
        /// <summary>
        /// Gets the index of a given item inside the inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItemIndex(Item item) => Items.FindIndex(it => item == it);

        /// <summary>
        /// Invoked when an item is successfully added to the inventory.
        /// </summary>
        public event Action<Item> OnItemAdded;
        /// <summary>
        /// Invoked when an item is successfully removed from the inventory. Int indicates it's index in the inventory.
        /// </summary>
        public event Action<Item, int> OnItemRemoved;
        /*/// <summary>
        /// Invoked when an item's stacks are successfully incremented; Int indicates stacks before increment.
        /// </summary>
        public event Action<Item, int> OnItemStacksAdded;
        /// <summary>
        /// Invoked when an item's stacks are successfully decremented; Int indicates stacks before decrement.
        /// </summary>
        public event Action<Item, int> OnItemStacksRemoved;*/

        /// <summary>
        /// Returns the item under specified index.
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="IndexOutOfRangeException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="InvalidCastException"/>
        /// <returns></returns>
        public Item this[int key] { get => Items[key]; }

        private bool InventoryInitiated { get; set; } = false;
        /// <summary>
        /// Initiates the inventory inside this method. (Sealed, if you want to use OnAwake, use <see cref="PostAwake"/>)
        /// </summary>
        public override sealed void OnAwake()
        {
            if (!InventoryInitiated)
            {
                Items = new Item[InventorySize];
                InventoryInitiated = true;
            }

            PostAwake();
        }

        /// <summary>
        /// Call this instead of <see cref="Awake"/>.
        /// </summary>
        protected virtual void PostAwake() { }

        /// <summary>
        /// Adds an Item to the first empty slot.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddItem(Item item)
        {
            if (item == null)
                return false;

            int emptyIndex = Items.GetEmptyIndex();
            if (emptyIndex == -1)
                return false;

            Items[emptyIndex] = item;
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
        public bool AddItem(Item item, int toIndex)
        {
            if (Items[toIndex] || toIndex >= Items.Length || item == null)
                return false;

            Items[toIndex] = item;
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
        public bool AddItem(Item item, int toIndex, out Item replacedItem)
        {
            if (toIndex < 0 || toIndex >= Items.Length || item == null)
            {
                replacedItem = null;
                return false;
            }

            replacedItem = Items[toIndex];
            Items[toIndex] = item;
            OnItemAdded?.Invoke(item);
            item.ManipulateFromInventory(this, true);

            return true;
        }

        /// <summary>
        /// Removes the specified item from Inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveItem(Item item)
        {
            int index = Items.FindIndex(it => item == it);

            if (index == -1) 
                return false;

            Item toRemove = Items[index];

            if (!toRemove)
                return false;

            Items[index] = null;
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
            Item toRemove = Items[index];

            if (!toRemove)
                return false;

            Items[index] = null;
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
        public bool RemoveItem(int index, out Item item)
        {
            Item toRemove = Items[index];

            if (!toRemove)
            {
                item = null;
                return false;
            }

            Items[index] = null;
            OnItemRemoved?.Invoke(toRemove, index);
            toRemove.ManipulateFromInventory(this, false);
            item = toRemove;
            return true;
        }
    }
}