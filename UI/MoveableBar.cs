using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI
{
    public class MoveableBar : MonoBehaviour
    {
        public Image MovableBar;
        public RectTransform UIToMove;

        /// <summary>
        /// Rect min and max anchores stored in a vector4. Sorted as follows: anchorMin.x, anchorMin.y, anchorMax.x, anchorMax.y.
        /// </summary>
        public Vector4 OriginalRectSize { get; private set; }

        /// <summary>
        /// Returns current rect size.
        /// </summary>
        public Vector4 CurrentRectSize => UIToMove != null ? new Vector4(UIToMove.anchorMin.x, UIToMove.anchorMin.y, UIToMove.anchorMax.x, UIToMove.anchorMax.y) : Vector4.zero;

        /// <summary>
        /// Last position at which anchors were before the window was released.
        /// </summary>
        private Vector4 LastRectSize { get; set; }

        /// <summary>
        /// Position at which the mouse is relative to the bar.
        /// </summary>
        private Vector2 MouseOffset { get; set; }

        /// <summary>
        /// Returns true if the window is currently moved.
        /// </summary>
        public bool IsOnBar { get; private set; }

        public bool LimitedToScreenView;

        public bool CanMoveUI()
        {
            return IsOnBar;
        }

        private void Awake()
        {
            SetRectDefaultSizes();
            SetUnityEventToBar();
        }

        /// <summary>
        /// Sets <see cref="OriginalRectSize"/> and <see cref="LastRectSize"/> to <see cref="CurrentRectSize"/>.
        /// </summary>
        private void SetRectDefaultSizes()
        {
            OriginalRectSize = LastRectSize = CurrentRectSize;
        }

        private void SetUnityEventToBar()
        {
            //Looks for an EventTrigger component, if there isn't any, a new one will be created.
            EventTrigger Uevent = MovableBar.GetComponent<EventTrigger>() ?? MovableBar.gameObject.AddComponent<EventTrigger>();

            //Sets an EventTrigger for PointerDown to activate SetOnBarTrue method.
            EventTrigger.Entry entry = new EventTrigger.Entry() { eventID = EventTriggerType.PointerDown };
            entry.callback.AddListener((data) => { SetOnBarTrue((PointerEventData)data); });

            //Sets an EventTrigger for PointerUp to activate SetOnBarFalse method.
            EventTrigger.Entry entry2 = new EventTrigger.Entry() { eventID = EventTriggerType.PointerUp };
            entry2.callback.AddListener((data) => { SetOnBarFalse((PointerEventData)data); });

            Uevent.triggers.Add(entry);
            Uevent.triggers.Add(entry2);
        }

        private void SetOnBarTrue(PointerEventData data)
        {
            IsOnBar = true;
            //Saves at what offset the mouse was when dragging the menu relative to it.
            MouseOffset = Input.mousePosition / new Vector2(Screen.width, Screen.height);

            AdvancedDebug.Log("Cursor is moving " + name, 5);
        }

        private void SetOnBarFalse(PointerEventData data)
        {
            IsOnBar = false;
            LastRectSize = CurrentRectSize;

            SetWindowOffBounds();

            AdvancedDebug.Log("Cursor stopped moving " + name, 5);
        }

        private void SetWindowOffBounds()
        {
            if (!LimitedToScreenView)
                return;

            //Checks if any side of the window is outside the screen.
            bool isBoundsLeft = UIToMove.anchorMin.x < 0;
            bool isBoundsRight = UIToMove.anchorMax.x > 1;
            bool isBoundsUp = UIToMove.anchorMax.y > 1;
            bool isBoundsDown = UIToMove.anchorMin.y < 0;

            //Will be used to set anchorMin.
            float xToGiveMin = UIToMove.anchorMin.x;
            float yToGiveMin = UIToMove.anchorMin.y;

            //Will be used to set anchorMax.
            float xToGiveMax = UIToMove.anchorMax.x;
            float yToGiveMax = UIToMove.anchorMax.y;

            if (isBoundsRight)
            {
                float diff = (xToGiveMax - 1);
                xToGiveMax -= diff;
                xToGiveMin -= diff;
            }
            else if (isBoundsLeft)
            {
                float diff = xToGiveMin.ToPositive();
                xToGiveMin = 0;
                xToGiveMax += diff;
            }

            if (isBoundsUp)
            {
                float diff = (yToGiveMax - 1);
                yToGiveMax -= diff;
                yToGiveMin -= diff;
            }
            else if (isBoundsDown)
            {
                float diff = yToGiveMin.ToPositive();
                yToGiveMin = 0;
                yToGiveMax += diff;
            }

            //Sets anchors.
            UIToMove.anchorMin = new Vector2(xToGiveMin, yToGiveMin);
            UIToMove.anchorMax = new Vector2(xToGiveMax, yToGiveMax);
            UIToMove.offsetMin = UIToMove.offsetMax = Vector2.zero;

            //Sets LastRectSize to this current one so it does not resume from the position it was in before.
            LastRectSize = CurrentRectSize;
        }

        private void OnGUI()
        {
            MoveMenu();
        }

        private void MoveMenu()
        {
            if (!CanMoveUI())
                return;

            //Retrieves mouse position in 0-1 position.
            Vector2 mousePos = Hooks.Cursor.MousePosInPercent;

            //Sets anchor min and max.
            UIToMove.anchorMin = new Vector2(mousePos.x + LastRectSize.x - MouseOffset.x, mousePos.y + LastRectSize.y - MouseOffset.y);
            UIToMove.anchorMax = new Vector2(mousePos.x + LastRectSize.z - MouseOffset.x, mousePos.y + LastRectSize.w - MouseOffset.y);

            //Sets offset to 0 (The UI will be filling the whole anchor window).
            UIToMove.offsetMin = UIToMove.offsetMax = Vector2.zero;
        }
    }
}

