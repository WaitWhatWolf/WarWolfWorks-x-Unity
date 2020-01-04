using System;
using System.Collections.Generic;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Itemization
{
    /// <summary>
    /// Advanced Non-Fixed-Size Inventory for your Non-Fixed-Size Inventory needs.
    /// </summary>
    public class FlexibleInventory : EntityComponent, IInventory
    {
        private List<Item> Items = new List<Item>();
        /// <summary>
        /// Returns the item at the current index; In case of an exception, returns null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Item this[int key]
        {
            get
            {
                try { return Items[key]; }
                catch { return null; }
            }
        }

        /// <summary>
        /// Returns the amount of items currently inside the inventory.
        /// </summary>
        public int ItemsCount => Items.Count;

        /// <summary>
        /// Invoked when an item is successfully added to the inventory.
        /// </summary>
        public event Action<Item> OnItemAdded;
        /// <summary>
        /// Invoked when an item is successfully removed from the inventory.
        /// </summary>
        public event Action<Item, int> OnItemRemoved;

        /// <summary>
        /// Adds an item to this inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddItem(Item item)
        {
            if (item == null)
                return false;

            try
            {
                Items.Add(item);
                OnItemAdded?.Invoke(item);
                item.ManipulateFromInventory(this, true);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Returns all items currently inside the inventory.
        /// </summary>
        /// <param name="clone"></param>
        /// <returns></returns>
        public IEnumerable<Item> GetAllItems(bool clone = false)
        {
            return clone ? new List<Item>(Items) : Items;
        }

        /// <summary>
        /// Returns the first item found under given name.
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public Item GetItem(string itemName)
        {
            return Items.Find(i => i.Name == itemName);
        }

        /// <summary>
        /// Returns the first item under given ID.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public Item GetItem(int itemID)
        {
            return Items.Find(i => i.ID == itemID);
        }

        /// <summary>
        /// Returns an item based on given match.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public Item GetItem(Predicate<Item> match)
        {
            return Items.Find(match);
        }

        /// <summary>
        /// Returns the index of an item currently inside the inventory; If no item was found, it will return -1.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItemIndex(Item item)
        {
            return Items.FindIndex(i => i == item);
        }

        /// <summary>
        /// Removes an item from this inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveItem(Item item)
        {
            try
            {
                int index = GetItemIndex(item);
                bool removed = Items.Remove(item);
                if(removed)
                {
                    OnItemRemoved?.Invoke(item, index);
                    item.ManipulateFromInventory(this, false);
                    return true;
                }

                return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Removes an item from this inventory at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveItem(int index)
        {
            try
            {
                if (Items[index] != null)
                {
                    Item toRemove = Items[index];
                    bool Removed = Items.Remove(toRemove);
                    if (Removed)
                    {
                        OnItemRemoved?.Invoke(toRemove, index);
                        toRemove.ManipulateFromInventory(this, false);
                        return true;
                    }

                    return false;
                }
                return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Removes an item from this inventory based on index and returns it in out.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveItem(int index, out Item item)
        {
            try
            {
                if (Items[index] != null)
                {
                    Item toRemove = Items[index];
                    bool Removed = Items.Remove(toRemove);
                    if (Removed)
                    {
                        OnItemRemoved?.Invoke(toRemove, index);
                        toRemove.ManipulateFromInventory(this, false);
                        item = toRemove;
                        return true;
                    }
                }

                item = null;
                return false;
            }
            catch
            {
                item = null;
                return false;
            }
        }
    }
}
