using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WarWolfWorks.Interfaces;
using WarWolfWorks.UI.MenusSystem;
using WarWolfWorks.Utility;
using static WarWolfWorks.Constants;

namespace WarWolfWorks.UI
{
    /// <summary>
    /// The manager class of the markers system.
    /// </summary>
    [AddComponentMenu(IN_ASSETMENU_WARWOLFWORKS + IN_ASSETMENU_UI + IN_ASSETMENU_MENUSYSTEM + nameof(GUIViewport))]
    public class GUIViewport : Menu, IParentInitiatable<Camera>, IDeinitiatable
    {
        /// <summary>
        /// Used with <see cref="GUIViewport"/> to display a <see cref="Graphic"/> on screen through ViewportPoint.
        /// </summary>
        [Serializable]
        public abstract class GUI : IParentInitiatable<GUIViewport>, IDisposable
        {
            /// <summary>
            /// How the <see cref="Position"/> is used.
            /// </summary>
            public enum GUIType
            {
                /// <summary>
                /// <see cref="Position"/> is a world position, and <see cref="GUIViewport"/> uses <see cref="Camera.WorldToViewportPoint(Vector3)"/>
                /// to set to to an achored position.
                /// </summary>
                WorldPosToViewport,
                /// <summary>
                /// The <see cref="Position"/> is the anchored position. (0-1)
                /// </summary>
                AsAnchors,
            }

            /// <summary>
            /// Name of this <see cref="GUI"/>; This is required to be different from other <see cref="GUI"/> elements
            /// as it will not be identifiable otherwise, and <see cref="New(GUI, out int)"/> will return false.
            /// </summary>
            public int ID { get; internal set; }

            /// <summary>
            /// Core graphic of the GUI.
            /// </summary>
            public Graphic CoreGraphic { get; protected set; }

            /// <summary>
            /// Is this <see cref="GUI"/> initiated?
            /// </summary>
            public bool Initiated { get; private set; }

            /// <summary>
            /// <see cref="GUIViewport"/> parent; Set after <see cref="New(GUI, out int)"/> was successfully called.
            /// </summary>
            public GUIViewport Parent { get; private set; }

            /// <summary>
            /// Returns true if the <see cref="CoreGraphic"/> is visible on it's parent canvas.
            /// </summary>
            public bool Visible { get; internal set; }

            bool IParentInitiatable<GUIViewport>.Init(GUIViewport parent)
            {
                if (Initiated || parent == null)
                    return false;

                Initiated = true;
                Parent = parent;
                OnInit();

                return true;
            }

            /// <summary>
            /// Deinitializes this <see cref="GUI"/>, making it Initiable again.
            /// </summary>
            protected void Deinit()
            {
                Initiated = false;
                Parent = null;
            }

            /// <summary>
            /// The size of the <see cref="CoreGraphic"/> on screen.
            /// </summary>
            public abstract Vector2 AnchoredSize { get; }
            /// <summary>
            /// Position at which <see cref="CoreGraphic"/> displays itself.
            /// </summary>
            public abstract Vector3 Position { get; protected set; }
            /// <summary>
            /// Called the parent successfully sets this <see cref="GUI"/> through <see cref="New(GUI, out int)"/>.
            /// </summary>
            protected abstract void OnInit();
            /// <summary>
            /// Called the parent successfully removes this <see cref="GUI"/> through <see cref="Remove(GUI)"/>.
            /// </summary>
            public abstract void Dispose();
            /// <summary>
            /// How this <see cref="GUI"/>'s <see cref="Position"/> is used by <see cref="GUIViewport"/>.
            /// </summary>
            public abstract GUIType Type { get; }
        }

        /// <summary>
        /// All markers currently active.
        /// </summary>
        private List<GUI> AllGUI { get; set; } = new List<GUI>();

        /// <summary>
        /// The amount of <see cref="GUI"/> elements that are active inside this <see cref="GUIViewport"/>.
        /// </summary>
        public int GUICount => AllGUI.Count;

        /// <summary>
        /// Is the <see cref="GUIViewport"/> menu initiated?
        /// </summary>
        public bool Initiated { get; private set; }

        /// <summary>
        /// The camera used to display GUI elements on screen.
        /// </summary>
        public Camera Parent { get; private set; }

        /// <summary>
        /// Adds a new <see cref="GUI"/> and returns it's given ID.
        /// </summary>
        /// <param name="gui"></param>
        /// <param name="guiID"></param>
        public void New(GUI gui, out int guiID)
        {
            guiID = -1;

            if (AllGUI.FindIndex(c => gui.ID == c.ID) != -1)
                return;

            if ((gui as IParentInitiatable<GUIViewport>).Init(this))
            {
                gui.CoreGraphic.rectTransform.SetParent(Holder);
                AllGUI.Add(gui);
                int toGive = 0;
                while(AllGUI.FindIndex(g => g.ID == toGive) != -1)
                {
                    toGive = UnityEngine.Random.Range(GUICount, GUICount + 10);
                }

                gui.ID = guiID = toGive;
            }
        }

        /// <summary>
        /// Adds a new <see cref="GUI"/>.
        /// </summary>
        /// <param name="gui"></param>
        public void New(GUI gui)
        {
            if (AllGUI.FindIndex(c => gui.ID == c.ID) != -1)
                return;

            if ((gui as IParentInitiatable<GUIViewport>).Init(this))
            {
                gui.CoreGraphic.rectTransform.SetParent(Holder);
                AllGUI.Add(gui);
                int toGive = 0;
                while (AllGUI.FindIndex(g => g.ID == toGive) != -1)
                {
                    toGive = UnityEngine.Random.Range(GUICount, GUICount + 10);
                }

                gui.ID = toGive;
            }
        }

        private void Update()
        {
            if (Initiated)
            {
                for (int i = 0; i < AllGUI.Count; i++)
                {
                    Vector3 center;
                    bool Visible;

                    switch (AllGUI[i].Type)
                    {
                        default:
                            center = Parent.WorldToViewportPoint(AllGUI[i].Position);
                            Visible = center.z > 0;
                            break;
                        case GUI.GUIType.AsAnchors:
                            center = AllGUI[i].Position;
                            Visible = Hooks.Rendering.IsVisible(AllGUI[i].CoreGraphic.rectTransform);
                            break;
                    }


                    if (Visible)
                    {
                        AllGUI[i].Visible = true;
                        Vector3 ActSize = AllGUI[i].AnchoredSize * 0.5f;

                        AllGUI[i].CoreGraphic.rectTransform.SetAnchoredUI(
                        center.x - ActSize.x, center.y - ActSize.y,
                        center.x + ActSize.x, center.y + ActSize.y);
                    }
                    else
                    {
                        AllGUI[i].Visible = false;
                        AllGUI[i].CoreGraphic.rectTransform.SetAnchoredUI(Vector4.zero);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a GUI element by ID.
        /// </summary>
        /// <param name="id"></param>
        public GUI GetGUI(int id)
        {
            int index = AllGUI.FindIndex(c => c.ID == id);
            if (index == -1)
                return null;

            return AllGUI[index];
        }

        /// <summary>
        /// Removes a GUI element.
        /// </summary>
        /// <param name="gui"></param>
        /// <returns></returns>
        public bool Remove(GUI gui)
        {
            if (gui == null)
                return false;

            int index = AllGUI.FindIndex(m => m.ID == gui.ID);
            if (index != -1)
            {
                AllGUI[index].Dispose();
                AllGUI.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Initiates the <see cref="GUIViewport"/>.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool Init(Camera parent)
        {
            if (Initiated || parent == null)
                return false;

            Parent = parent;

            Initiated = true;
            return true;
        }

        /// <summary>
        /// Deinitiates this <see cref="GUIViewport"/>.
        /// </summary>
        /// <returns></returns>
        public bool Deinit()
        {
            if (!Initiated)
                return false;

            Initiated = false;
            Parent = null;
            return true;
        }
    }
}
