using System;
using System.Collections.Generic;
using UnityEngine;

namespace WarWolfWorks.UI.MenusSystem
{
    /// <summary>
    /// Core class for handling Menus; All Menus have a Holder which determine a large proportion of the Menu's behaviour (like IsActive),
    /// It is best practice to put a menu script on the Canvas GameObject, and put the Holder as child of the Canvas.
    /// </summary>
    public abstract class Menu : MonoBehaviour
    {
        /// <summary>
        /// All menus that are currently in the scene.
        /// </summary>
        private static List<Menu> AllMenus = new List<Menu>();

        /// <summary>
        /// The parent transform which holds all elements of this menu.
        /// </summary>
        [SerializeField]
        protected RectTransform Holder;

        /// <summary>
        /// Triggers when this menu is activated.
        /// </summary>
        public event Action OnActivated;
        /// <summary>
        /// Triggers when this menu is deactivated.
        /// </summary>
        public event Action OnDeactivated;

        /// <summary>
        /// Determines if the menu is currently active. (Pointer to Holder.gameObject.activeInHierarchy)
        /// </summary>
        public bool IsActive { get => Holder.gameObject.activeInHierarchy; private set => Holder.gameObject.SetActive(value); }

        /// <summary>
        /// Returns the Holder of this Menu.
        /// </summary>
        /// <returns></returns>
        public RectTransform GetHolder() => Holder;

        /// <summary>
        /// Returns a menu of the given T type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetMenu<T>() where T : Menu
        {
            return (T)AllMenus.Find(m => m is T);
        }

        /// <summary>
        /// Returns a menu of given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Menu GetMenu(Type type)
        {
            return AllMenus.Find(m => m.GetType() == type);
        }

        /// <summary>
        /// Invoked when this menu is activated. (Invoked before <see cref="OnActivated"/>)
        /// </summary>
        protected virtual void OnActivate() { }
        /// <summary>
        /// Invoked when this menu is deactivated. (Invoked before <see cref="OnDeactivated"/>)
        /// </summary>
        protected virtual void OnDeactivate() { }

        /// <summary>
        /// Activates this menu.
        /// </summary>
        public void ActivateMenu()
        {
            if (IsActive)
                return;

            IsActive = true;
            OnActivate();
            OnActivated?.Invoke();
        }

        /// <summary>
        /// Deactivates this menu.
        /// </summary>
        public void DeactivateMenu()
        {
            if (!IsActive)
                return;

            IsActive = false;
            OnDeactivate();
            OnDeactivated?.Invoke();
        }
        
        /// <summary>
        /// Activates the menu of given T type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void ActivateMenu<T>() where T : Menu
        {
            T menu = GetMenu<T>();
            menu.ActivateMenu();
        }

        /// <summary>
        /// Deactivates the menu of given T type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void DeactivateMenu<T>() where T : Menu
        {
            T menu = GetMenu<T>();
            menu.DeactivateMenu();
        }

        /// <summary>
        /// Deactivates all menus.
        /// </summary>
        public static void DeactivateAllMenus()
        {
            for(int i = 0; i < AllMenus.Count; i++)
            {
                AllMenus[i].DeactivateMenu();
            }
        }


        /// <summary>
        /// Adds this menu to the <see cref="AllMenus"/> list; When overriding, make sure to include base.Awake().
        /// </summary>
        protected virtual void Awake()
        {
            AllMenus.Add(this);
        }

        /// <summary>
        /// Removes this menu from <see cref="AllMenus"/> list.
        /// </summary>
       ~Menu() => AllMenus.Remove(this);
    }
}
