using System;
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
        private const KeyCode UpKey = KeyCode.UpArrow;

        private const KeyCode DownKey = KeyCode.DownArrow;

        private float ScrollSensitivity = 0.1f;

        [SerializeField]
        private TextMeshProUGUI consoleContentText;

        [SerializeField]
        private TextMeshProUGUI consoleInputText;

        [SerializeField]
        private bool CustomConsoleLayout;

        private int inptFldIndx = 0;

        private int InputFieldIndex
        {
            get
            {
                return inptFldIndx;
            }
            set
            {
                inptFldIndx = value;
                if (inptFldIndx < 0)
                {
                    inptFldIndx = 0;
                }
                if (inptFldIndx >= WWWConsole.PreviousInputs.Count)
                {
                    inptFldIndx = WWWConsole.PreviousInputs.Count - 1;
                }
            }
        }

        public TextMeshProUGUI ConsoleInputText => consoleContentText;

        protected override void Awake()
        {
            base.Awake();

            WWWConsole.OnConsoleTextInsert += UpdateConsoleText;
            WWWConsole.OnConfirmInput += ConsoleSetter;
            UpdateTextSize();
        }

        protected override void OnActivate()
        {
            ScrollConsoleContent(up: true);
            ScrollConsoleContent(up: false);
            UpdateTextSize();
        }

        private const string ConsoleKeyName = "Console";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CheckForConsole()
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
            if ((bool)consoleContentText)
            {
                consoleContentText.autoSizeTextContainer = false;
                consoleContentText.fontSize = Screen.currentResolution.height / 50;
            }
        }

        private void ConsoleSetter(StringBuilder sb)
        {
            InputFieldIndex = WWWConsole.PreviousInputs.Count - 1;
            try
            {
                string text = sb.ToString();
                if (text == "ConsoleHelp")
                {
                    WWWConsole.WriteCommandHelper("console");
                    WWWConsole.InsertTextToConsole("\nAllows you to change the size of the console menu. (0 to 1)\nConsole_Set_Size = [SizeX]x[SizeY]");
                    WWWConsole.InsertTextToConsole("\nSets the scroll sensivity of the console.\nConsole_Set_ScrollSpeed = [speed]");
                }
                else if (text.Contains("Console_Set_Size"))
                {
                    string text2 = sb.ToString().Split(new char[1]
                    {
                    '='
                    })[1];
                    string[] array = text2.Split(new char[1]
                    {
                    'x'
                    });
                    float x = Convert.ToSingle(array[0]);
                    float num = Convert.ToSingle(array[1]);
                    Holder.anchorMin = new Vector2(0f, 1f - num);
                    Holder.anchorMax = new Vector2(x, 1f);
                    Vector2 vector2 = Holder.offsetMin = (Holder.offsetMax = Vector2.zero);
                }
                else if (text.Contains("Console_Set_ScrollSpeed"))
                {
                    ScrollSensitivity = Convert.ToSingle(text.Split(new char[1]
                    {
                    '='
                    })[1]);
                }
            }
            catch (Exception e)
            {
                WWWConsole.DebugException(e, "ConsoleHelp");
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
                    InputFieldIndex += num;
                }
                string text = WWWConsole.PreviousInputs[InputFieldIndex].ToString();
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
            Vector2 vector2 = component.offsetMin = (component.offsetMax = Vector2.zero);
        }
    }
}