#pragma warning disable IDE0044
#pragma warning disable 0649

using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.NyuEntities.Itemization
{
    /// <summary>
    /// Base class for Itemization in the WWWLibrary.
    /// </summary>
    [CompleteNoS]
    public abstract class NyuItem : ScriptableObject, IItem
	{
        #region Unity Serialized
        [SerializeField]
        private string s_Name;
        [SerializeField, TextArea]
        private string s_Description;
        [SerializeField]
        private Sprite s_Sprite;
        [SerializeField]
        private int s_ID = -1;
        #endregion

        /// <summary>
        /// Name of the item.
        /// </summary>
        public virtual string Name => s_Name;

        /// <summary>
        /// Description of the item. (for In-Game use)
        /// </summary>
        public virtual string Description => s_Description;

        /// <summary>
        /// The item's UI.
        /// </summary>
        public virtual Sprite Sprite => s_Sprite;

        /// <summary>
        /// Unique ID of this item.
        /// </summary>
        public int GetID() => s_ID;

        internal void ManipulateFromInventory(object owner, bool add)
        {
            if (add) OnAddedToInventory(owner);
            else OnRemovedFromInventory(owner);
        }

        /// <summary>
        /// Invoked when this item is added to an Inventory.
        /// </summary>
        /// <param name="inventory"></param>
        protected virtual void OnAddedToInventory(object inventory)
        {

        }

        /// <summary>
        /// Invoked when this item is removed from an Inventory.
        /// </summary>
        /// <param name="inventory"></param>
        protected virtual void OnRemovedFromInventory(object inventory)
        {

        }

        /// <summary>
        /// Searches for all resource items by their ID.      
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static NyuItem Find(int ID)
        {
            InitResources();

            foreach (NyuItem item in ResourceItems)
            {
                if (item.GetID() == ID)
                    return item;
            }

            return null;
        }

        private static void InitResources()
        {
            if (ResourceItems == null)
                ResourceItems = Resources.LoadAll<NyuItem>("/");
        }
        private static NyuItem[] ResourceItems;
	}
}