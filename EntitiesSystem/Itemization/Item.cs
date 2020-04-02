using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Itemization
{
    /// <summary>
    /// Base class for Itemization in the WWWLibrary.
    /// </summary>
    [CompleteNoS]
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public abstract class Item : ScriptableObject, IItem
	{
        [FormerlySerializedAs("itemName"), SerializeField]
        private string s_Name;
        /// <summary>
        /// Name of the item.
        /// </summary>
        public virtual string Name => s_Name;

        [FormerlySerializedAs("description"), SerializeField, TextArea]
        private string s_Description;
        /// <summary>
        /// Description of the item. (for In-Game use)
        /// </summary>
        public virtual string Description => s_Description;

        [FormerlySerializedAs("sprite"), SerializeField]
        private Sprite s_Sprite;
        /// <summary>
        /// The item's UI.
        /// </summary>
        public virtual Sprite Sprite => s_Sprite;

        [SerializeField, HideInInspector]
        private int s_ID = -1;
        /// <summary>
        /// Unique ID of this item.
        /// </summary>
        public int GetID() => s_ID;

        internal void ManipulateFromInventory(IInventory<Item> owner, bool add)
        {
            if (add) OnAddedToInventory(owner);
            else OnRemovedFromInventory(owner);
        }

        /// <summary>
        /// Invoked when this item is added to an Inventory.
        /// </summary>
        /// <param name="inventory"></param>
        protected virtual void OnAddedToInventory(IInventory<Item> inventory)
        {

        }

        /// <summary>
        /// Invoked when this item is removed from an Inventory.
        /// </summary>
        /// <param name="inventory"></param>
        protected virtual void OnRemovedFromInventory(IInventory<Item> inventory)
        {

        }



        private static Item[] ResourceItems;
        /// <summary>
        /// Searches for all resource items by their ID.      
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Item Find(int ID)
        {
            if (ResourceItems == null)
                ResourceItems = Resources.LoadAll<Item>("/");

            foreach(Item item in ResourceItems)
            {
                if (item.GetID() == ID)
                    return item;
            }

            return null;
        }
	}
}