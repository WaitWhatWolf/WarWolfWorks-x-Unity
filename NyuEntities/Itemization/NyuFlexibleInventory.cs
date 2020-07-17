using System;
using System.Collections.Generic;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.NyuEntities.Itemization
{
    /// <summary>
    /// Advanced Non-Fixed-Size Inventory for your Non-Fixed-Size Inventory needs.
    /// </summary>
    public abstract class NyuFlexibleInventory<T> : NyuComponent, IInventory<T> where T : NyuItem
    {
        private readonly List<T> ns_Items = new List<T>();
        /// <summary>
        /// Returns the item at the current index; In case of an exception, returns null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T this[int key]
        {
            get
            {
                try { return ns_Items[key]; }
                catch { return null; }
            }
        }

        /// <summary>
        /// Returns the amount of items currently inside the inventory.
        /// </summary>
        public int ItemsCount => ns_Items.Count;

        /// <summary>
        /// Invoked when an item is successfully added to the inventory.
        /// </summary>
        public event Action<T> OnItemAdded;
        /// <summary>
        /// Invoked when an item is successfully removed from the inventory.
        /// </summary>
        public event Action<T, int> OnItemRemoved;

        /// <summary>
        /// Adds an item to this inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddItem(T item)
        {
            if (item == null)
                return false;

            try
            {
                ns_Items.Add(item);
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
        public IEnumerable<T> GetAllItems(bool clone = false)
        {
            return clone ? new List<T>(ns_Items) : ns_Items;
        }

        /// <summary>
        /// Returns the first item found under given name.
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public T GetItem(string itemName)
        {
            return ns_Items.Find(i => i.Name == itemName);
        }

        /// <summary>
        /// Returns the first item under given ID.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public T GetItem(int itemID)
        {
            return ns_Items.Find(i => i.GetID() == itemID);
        }

        /// <summary>
        /// Returns an item based on given match.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public T GetItem(Predicate<T> match)
        {
            return ns_Items.Find(match);
        }

        /// <summary>
        /// Returns the index of an item currently inside the inventory; If no item was found, it will return -1.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItemIndex(T item)
        {
            return ns_Items.FindIndex(i => i == item);
        }

        /// <summary>
        /// Removes an item from this inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveItem(T item)
        {
            try
            {
                int index = GetItemIndex(item);
                bool removed = ns_Items.Remove(item);
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
                if (ns_Items[index] != null)
                {
                    T toRemove = ns_Items[index];
                    bool Removed = ns_Items.Remove(toRemove);
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
        public bool RemoveItem(int index, out T item)
        {
            try
            {
                if (ns_Items[index] != null)
                {
                    T toRemove = ns_Items[index];
                    bool Removed = ns_Items.Remove(toRemove);
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
