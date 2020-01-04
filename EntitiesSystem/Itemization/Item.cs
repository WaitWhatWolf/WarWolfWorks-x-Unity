using UnityEngine;
using WarWolfWorks.Debugging;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Itemization
{
    /// <summary>
    /// Base class for Itemization in the WWWLibrary.
    /// </summary>
	public abstract class Item : ScriptableObject
	{
        [SerializeField][Rename("Name")]
        private string itemName;
        /// <summary>
        /// Name of the item.
        /// </summary>
        public virtual string Name => itemName;

        [SerializeField][TextArea]
        private string description;
        /// <summary>
        /// Description of the item. (for In-Game use)
        /// </summary>
        public virtual string Description => description;

        [SerializeField]
        private Sprite sprite;
        /// <summary>
        /// The item's UI.
        /// </summary>
        public virtual Sprite Sprite => sprite;

        /// <summary>
        /// Rarity of the item in integer value; You can use a custom enum value and cast it as an integer.
        /// </summary>
        public abstract int Rarity { get; }

        [SerializeField]
        private int id;
        /// <summary>
        /// Unique ID of this item.
        /// </summary>
        public int ID => id;

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
	}
}