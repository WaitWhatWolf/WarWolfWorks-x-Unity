using System.Text;
using UnityEngine;
using UnityEngine.UI;
using WarWolfWorks.Utility;
using TMPro;

namespace WarWolfWorks.UI
{
    using Debugging;
    using WarWolfWorks.UI.MenusSystem;

    /// <summary>
    /// UI display of the <see cref="WWWConsole"/>.
    /// </summary>
    public sealed class ConsoleMenu : Menu
    {
        private float ScrollSensitivity => WWWConsole.ScrollSpeed;

        [SerializeField]
        private TextMeshProUGUI consoleContentText;

        [SerializeField]
        private TextMeshProUGUI consoleInputText;

        [SerializeField]
#pragma warning disable IDE0051
        private bool CustomConsoleLayout;
#pragma warning restore IDE0051

        /// <summary>
        /// Text of the console's input.
        /// </summary>
        public TextMeshProUGUI ConsoleInputText => consoleContentText;

        /// <summary>
        /// Sets the console up.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            WWWConsole.OnConsoleTextInsert += UpdateConsoleText;
            WWWConsole.OnCommandConfirmed += UpdateConsoleUI;
            UpdateTextSize();
        }

        private void UpdateConsoleUI(Command arg1, string arg2)
        {
            switch(arg1.Name)
            {
                case nameof(WWWConsole.s_console_font_size):
                    consoleContentText.fontSize = WWWConsole.FontSize;
                    break;
                case nameof(WWWConsole.s_console_size):
                    Holder.SetAnchoredUI(WWWConsole.AnchoredPosition);
                    break;
            }
        }

        /// <summary>
        /// Updates the size of the console and the scroll position.
        /// </summary>
        protected override void OnActivate()
        {
            ScrollConsoleContent(up: true);
            ScrollConsoleContent(up: false);
            UpdateTextSize();
        }

        private const string ConsoleKeyName = "Console";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#pragma warning disable IDE0051
        private static void CheckForConsole()
#pragma warning restore IDE0051
        {
            if (!DefaultKeys.KeyExists(ConsoleKeyName))
            {
                DefaultKeys.ForceAddKey(ConsoleKeyName, KeyCode.Tilde);
            }

            ConsoleMenu consoleMenu = UnityEngine.Object.FindObjectOfType<ConsoleMenu>();
            Canvas canvas = consoleMenu?.GetComponent<Canvas>();
            CanvasScaler canvasScaler = consoleMenu?.GetComponent<CanvasScaler>();
            if (consoleMenu == null)
            {
                Color color = new Color(0f, 0.6f, 0f, 1f);
                GameObject gameObject = new GameObject("ConsoleCanvas");
                consoleMenu = gameObject.AddComponent<ConsoleMenu>();
                canvas = gameObject.AddComponent<Canvas>();
                canvasScaler = gameObject.AddComponent<CanvasScaler>();
                GameObject gameObject2 = new GameObject("ConsoleBG");
                consoleMenu.Holder = gameObject2.AddComponent<RectTransform>();
                gameObject2.AddComponent<Image>().color = color;
                RectTransform rectTransform = (RectTransform)gameObject2.transform;
                rectTransform.SetParent(gameObject.transform);
                rectTransform.SetAnchoredUI(new Vector4(0f, 0.45f, 0.55f, 1f));
                GameObject gameObject3 = new GameObject("ConsoleBGInside");
                gameObject3.AddComponent<Image>().color = Color.black;
                RectTransform rectTransform2 = (RectTransform)gameObject3.transform;
                rectTransform2.SetParent(rectTransform);
                rectTransform2.SetAnchoredUI(new Vector4(0f, 0.025f, 0.99f, 1f));
                GameObject gameObject4 = new GameObject("ConsoleInputSeparator");
                gameObject4.AddComponent<Image>().color = color;
                RectTransform rectTransform3 = (RectTransform)gameObject4.transform;
                rectTransform3.SetParent(rectTransform);
                rectTransform3.SetAnchoredUI(new Vector4(0f, 0.175f, 1f, 0.2f));
                GameObject gameObject5 = new GameObject("ConsoleInputText");
                consoleMenu.consoleInputText = gameObject5.AddComponent<TextMeshProUGUI>();
                consoleMenu.consoleInputText.alignment = TextAlignmentOptions.MidlineLeft;
                consoleMenu.consoleInputText.richText = false;
                RectTransform rectTransform4 = (RectTransform)gameObject5.transform;
                rectTransform4.SetParent(rectTransform);
                rectTransform4.SetAnchoredUI(new Vector4(0f, 0.025f, 0.99f, 0.175f));
                GameObject gameObject6 = new GameObject("ConsoleContentMask");
                RectTransform rectTransform5 = gameObject6.AddComponent<RectTransform>();
                rectTransform5.SetParent(rectTransform);
                rectTransform5.SetAnchoredUI(new Vector4(0f, 0.2f, 0.99f, 1f));
                gameObject6.AddComponent<RectMask2D>();
                GameObject gameObject7 = new GameObject("ConsoleContentText");
                RectTransform rectTransform6 = gameObject7.AddComponent<RectTransform>();
                rectTransform6.SetParent(rectTransform5);
                rectTransform6.SetAnchoredUI(new Vector4(0f, 0f, 1f, 1f));
                consoleMenu.consoleContentText = gameObject7.AddComponent<TextMeshProUGUI>();
                consoleMenu.consoleContentText.alignment = TextAlignmentOptions.BottomLeft;
                consoleMenu.consoleContentText.enableWordWrapping = true;
                consoleMenu.consoleContentText.overflowMode = TextOverflowModes.Overflow;
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = true;
            canvas.sortingOrder = 32767;
            canvas.sortingLayerID = SortingLayer.layers[SortingLayer.layers.Length - 1].id;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.scaleFactor = 1f;
            canvasScaler.referencePixelsPerUnit = 100f;
            consoleMenu.DeactivateMenu();
        }

        private void UpdateTextSize()
        {
            if (consoleContentText)
            {
                consoleContentText.autoSizeTextContainer = false;
                consoleContentText.fontSize = Screen.currentResolution.height / 50;
            }
        }

        private void UpdateConsoleText(string text)
        {
            consoleContentText.text += text;
        }

        private void Update()
        {
            if (DefaultKeys.GetKeyDown(ConsoleKeyName))
            {
                if (IsActive)
                {
                    DeactivateMenu();
                }
                else
                {
                    ActivateMenu();
                }
            }
            if (!IsActive)
            {
                return;
            }
            if (Input.anyKey && WWWConsole.InsertInput(Hooks.GetKeyStroke(1)))
            {
                StringBuilder stringBuilder = new StringBuilder(WWWConsole.GetCurrentInput());
                stringBuilder.Insert(WWWConsole.InsertOffsetIndex, '|');
                consoleInputText.text = stringBuilder.ToString();
            }
            float axis = Input.GetAxis("Mouse ScrollWheel");
            if (axis < 0f)
            {
                ScrollConsoleContent(up: true);
            }
            else if (axis > 0f)
            {
                ScrollConsoleContent(up: false);
            }
            if (WWWConsole.PreviousInputs.Count >= 1 && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)))
            {
                string a = WWWConsole.GetCurrentInput().ToString();
                if (a != string.Empty)
                {
                    int num = (!Input.GetKeyDown(KeyCode.UpArrow)) ? 1 : (-1);
                    WWWConsole.InputFieldIndex += num;
                }
                string text = WWWConsole.PreviousInputs[WWWConsole.InputFieldIndex].ToString();
                WWWConsole.SetInputText(text);
                consoleInputText.text = text;
            }
        }

        private void ScrollConsoleContent(bool up)
        {
            float num = up ? ScrollSensitivity : (0f - ScrollSensitivity);
            RectTransform component = consoleContentText.GetComponent<RectTransform>();
            component.anchorMin = new Vector2(component.anchorMin.x, component.anchorMin.y + num);
            component.anchorMax = new Vector2(component.anchorMax.x, component.anchorMax.y + num);
            //Vector2 vector2 = component.offsetMin = (component.offsetMax = Vector2.zero);
        }
    }
}