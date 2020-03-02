using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Itemization
{
    /// <summary>
    /// Base class for Itemization in the WWWLibrary.
    /// </summary>
	public abstract class Item : ScriptableObject
	{
        [FormerlySerializedAs("itemName")]
        [SerializeField]
        private string s_Name;
        /// <summary>
        /// Name of the item.
        /// </summary>
        public virtual string Name => s_Name;

        [FormerlySerializedAs("description")]
        [SerializeField][TextArea]
        private string s_Description;
        /// <summary>
        /// Description of the item. (for In-Game use)
        /// </summary>
        public virtual string Description => s_Description;

        [FormerlySerializedAs("sprite")]
        [SerializeField]
        private Sprite s_Sprite;
        /// <summary>
        /// The item's UI.
        /// </summary>
        public virtual Sprite Sprite => s_Sprite;

        /// <summary>
        /// Rarity of the item in integer value; You can use a custom enum value and cast it as an integer.
        /// </summary>
        public abstract int Rarity { get; }

        [SerializeField][HideInInspector]
        private int s_ID = -1;
        /// <summary>
        /// Unique ID of this item.
        /// </summary>
        public int ID => s_ID;

        internal void ManipulateFromInventory(IInventory owner, bool add)
        {
            if (add) OnAddedToInventory(owner);
            else OnRemovedFromInventory(owner);
        }

        /// <summary>
        /// Invoked when this item is added to an Inventory.
        /// </summary>
        /// <param name="inventory"></param>
        protected virtual void OnAddedToInventory(IInventory inventory)
        {

        }

        /// <summary>
        /// Invoked when this item is removed from an Inventory.
        /// </summary>
        /// <param name="inventory"></param>
        protected virtual void OnRemovedFromInventory(IInventory inventory)
        {

        }

        private static Item[] ResourceItems;
        /// <summary>
        /// Searches for all resource items by their <see cref="ID"/>      
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Item Find(int ID)
        {
            if (ResourceItems == null)
                ResourceItems = Resources.LoadAll<Item>("/");

            foreach(Item item in ResourceItems)
            {
                if (item.ID == ID)
                    return item;
            }

            return null;
        }
	}
}