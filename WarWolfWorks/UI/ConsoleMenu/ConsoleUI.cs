using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WarWolfWorks.Attributes;
using WarWolfWorks.Debugging;
using WarWolfWorks.Enums;
using WarWolfWorks.Interfaces;
using WarWolfWorks.UI.MenusSystem;
using WarWolfWorks.UI.MenusSystem.SlickMenu;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI.ConsoleMenu
{
    /// <summary>
    /// An UI menu used to display the <see cref="WarWolfWorks.Debugging.Console"/>.
    /// </summary>
    [CompleteNoS]
    public sealed class ConsoleUI : SlickMenu<ConsoleUI, ConsoleCell, SlickBorder>, IRefreshable
    {
        /// <summary>
        /// Color of the console.
        /// </summary>
        public Color ConsoleColor
        {
            get => s_ConsoleColor;
            set
            {
                s_ConsoleColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override Color ThemeColor => ConsoleColor;

        /// <summary>
        /// Returns a range of -1 to <see cref="pv_Cells"/> length.
        /// </summary>
        protected override IntRange CellsRange => pv_CellsRange;
        /// <summary>
        /// Returns <see cref="SlickNavigationType.Standard"/>.
        /// </summary>
        public override SlickNavigationType NavigationType => SlickNavigationType.Standard;

        /// <summary>
        /// Sets the size of the console, using anchored sizing.
        /// </summary>
        /// <param name="size"></param>
        public void SetConsoleSize(Vector4 size)
        {
            pv_Background.rectTransform.SetAnchoredUI(size);
        }

        /// <summary>
        /// Returns the size of the console in anchored size.
        /// </summary>
        /// <returns></returns>
        public Vector4 GetConsoleAnchoredSize() => pv_Background.rectTransform.GetAnchoredPosition();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Console.AddCommands(new Command_Console_Change_Size(), new Command_Console_Change_Color());

            Console.AllowCustomInput = true; //Without this, the console will be unusable.

            Console.OnTurnOn += ActivateMenu;
            Console.OnTurnOff += DeactivateMenu;
            Console.OnWrite += Event_UpdateFullText;

            #region UI Build
            GetHolder().transform.SetAsLastSibling();

            GameObject backgroundGO = new GameObject("Background");
            backgroundGO.transform.SetParent(GetHolder());
            pv_Background = backgroundGO.AddComponent<Image>();
            pv_Background.color = new Color(ConsoleColor.r * 0.1f, ConsoleColor.g * 0.1f, ConsoleColor.b * 0.1f, ConsoleColor.a);
            pv_Background.rectTransform.SetAnchoredUI(0f, 0f, 1f, 0.25f);

            pv_BackgroundBorder = backgroundGO.AddComponent<SlickBorder>();
            pv_BackgroundBorder.Color = s_ConsoleColor;
            pv_BackgroundBorder.Flags = Enums.SlickBorderFlags.All;
            pv_BackgroundBorder.Init();

            GameObject inputGO = new GameObject("InputText");
            inputGO.transform.SetParent(pv_Background.transform);
            pv_InputBackground = inputGO.AddComponent<Image>();
            pv_InputBackground.rectTransform.SetAnchoredUI(0f, 0f, 1f, 0.1f);
            pv_InputBackground.color = pv_Background.color;
            pv_InputBorder = inputGO.AddComponent<SlickBorder>();
            pv_InputBorder.Color = s_ConsoleColor;
            pv_InputBorder.Flags = Enums.SlickBorderFlags.All;
            pv_InputBorder.Init();
            pv_InputText = inputGO.AddComponent<TMP_InputField>();
            pv_InputText.image = pv_InputBackground;
            pv_InputText.image.color = pv_Background.color;
            RectTransform textArea = CreateHolder("Text Area", 0f, 0f, 1f, 1f, pv_InputBackground.rectTransform);
            RectMask2D textAreaMask = textArea.gameObject.AddComponent<RectMask2D>();
            textAreaMask.padding = new Vector4(-8, -8, -5, -5);
            pv_InputText.textViewport = textArea;
            pv_BackInputText = CreateHolder("Text", 0f, 0f, 1f, 1f, textArea).gameObject.AddComponent<TextMeshProUGUI>();
            pv_BackInputText.color = ConsoleColor;
            pv_InputText.textComponent = pv_BackInputText;
            pv_InputText.textComponent.color = ConsoleColor;
            pv_InputText.textComponent.enableAutoSizing = true;
            pv_InputText.textComponent.fontSizeMin = 12f;
            pv_InputText.textComponent.fontSizeMax = 120f;

            pv_InputText.onValueChanged.AddListener((s) => { Event_UpdateOnInputTextChanged(s); });

            RectTransform ftRect = CreateHolder("Scroll View", 0f, 0.1f, 1f, 1f, pv_Background.rectTransform);
            ftRect.gameObject.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            pv_FTRect = ftRect.gameObject.AddComponent<ScrollRect>();

            RectTransform viewport = CreateHolder("Viewport", holder: ftRect);
            viewport.gameObject.AddComponent<Image>().color = Hooks.Colors.White;
            pv_FTMask = viewport.gameObject.AddComponent<Mask>();
            pv_FTMask.showMaskGraphic = false;

            RectTransform fullTextRect = CreateHolder("FullText", holder: viewport);
            pv_FullText = fullTextRect.gameObject.AddComponent<TextMeshProUGUI>();
            pv_FullText.enableWordWrapping = true;
            pv_FullText.rectTransform.SetAnchoredUI(0f, 0f, 1f, 0f);
            pv_FullText.color = ConsoleColor;
            pv_FullText.enableAutoSizing = false;
            pv_FullText.fontSize = pv_FullTextFontSize;

            pv_FTRect.viewport = viewport;
            pv_FTRect.content = fullTextRect;
            pv_FTRect.horizontal = false;
            pv_FTRect.vertical = true;
            pv_FTRect.scrollSensitivity = 20f;
            pv_FTRect.CalculateLayoutInputVertical();

            pv_CellsRect = CreateHolder("Cells Rect", 0f, 0.1f, 1f, 1f, pv_Background.rectTransform);
            #endregion
        }

        /// <summary>
        /// Refreshes the color of the console to be up-to-date.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();

            pv_BackgroundBorder.Color = s_ConsoleColor;
            pv_BackgroundBorder.Refresh();
            pv_InputBorder.Color = s_ConsoleColor;
            pv_InputBorder.Refresh();

            pv_Background.color = new Color(ConsoleColor.r * 0.1f, ConsoleColor.g * 0.1f, ConsoleColor.b * 0.1f, ConsoleColor.a);
            pv_InputText.image.color = pv_Background.color;
            pv_InputText.textComponent.color = ConsoleColor;
            pv_FullText.color = ConsoleColor;
        }

        /// <summary>
        /// Starts a refresh checker.
        /// </summary>
        protected override void OnActivate()
            => pv_ColorUpdaterRoutine = StartCoroutine(RefreshChecker());

        /// <summary>
        /// Stops the refresh checker.
        /// </summary>
        protected override void OnDeactivate()
            => StopCoroutine(pv_ColorUpdaterRoutine);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="direction"></param>
        protected override void OnNavigationUse(Direction direction)
        {
            bool browseCondition = Index == -1 && (string.IsNullOrEmpty(pv_InputText.text) || BrowseIndex != -1);

            switch (direction)
            {
                case Direction.Up: 
                    _ = browseCondition ? ++BrowseIndex : Index++;
                    OnNavigationIndexUpdate();
                    break;
                case Direction.Down: 
                    _ = browseCondition ? --BrowseIndex : Index--;
                    OnNavigationIndexUpdate();
                    break;
                case Direction.Right:
                case Direction.Left:
                    RebuildUI();
                    break;
            }
        }

        /// <summary>
        /// Sets index to selection index if type is 0 (left mouse click).
        /// </summary>
        /// <param name="type"></param>
        protected override void OnSelectionAccepted(int type)
        {
            if(BrowseIndex != -1)
            {
                string use = pv_InputText.text;
                ResetIndicies();
                pv_InputText.text = use;
            }
            else if (Index == -1)
            {
                Console.ForceSetInput(pv_InputText.text, out _);
                Console.Write(true);
                ResetIndicies();
                pv_InputText.text = string.Empty;
            }
            else
            {
                string toUse = pv_Cells[Index].CellCommand.Recognition + '/';
                ResetIndicies();
                pv_InputText.text = toUse;
            }

            SelectInput();
            RebuildUI();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>Returns built cells.</returns>
        protected override IEnumerable<ConsoleCell> GetNavigationCells() => pv_Cells;

        /// <summary>
        /// All refresh cells are navigation cells.
        /// </summary>
        /// <returns><see cref="GetNavigationCells"/></returns>
        protected override IEnumerable<ConsoleCell> GetRefreshCells() => GetNavigationCells();

        /// <summary>
        /// Builds all cells.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ConsoleCell> BuildUI()
        {
            pv_RecognitionCommandsRaw = string.IsNullOrEmpty(Console.Input) ? new() : Console.GetClosestCommandsRecognition();
            pv_RecognitionCommands = pv_RecognitionCommandsRaw.Values.ToArray();
            pv_Cells = new ConsoleCell[pv_RecognitionCommands.Length];

            pv_CellsRange = new IntRange(-1, pv_Cells.Length);

            pv_CellsRect.SetAnchoredUI(0f, 0.1f, 1f, 0.1f + (pv_Cellheight * pv_CellsRange.Max));

            for (int i = 0; i < pv_Cells.Length; i++)
            {
                pv_Cells[i] = CreateCell(i, pv_RecognitionCommands[i]);
            }

            return pv_Cells;
        }

        /// <summary>
        /// Creates a console cell.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override ConsoleCell CreateCell(int index, params object[] args)
        {
            Command command = (Command)args[0];
            RectTransform cellRect = CreateHolder($"Cell_{command.Recognition}", 0f, (float)index / (float)pv_CellsRange.Max, 1f, (float)(index + 1) / (float)pv_CellsRange.Max, holder: pv_CellsRect);

            Image background = cellRect.gameObject.AddComponent<Image>();

            RectTransform overlayRect = CreateHolder("OverlayText", holder: cellRect);
            TextMeshProUGUI overlay = overlayRect.gameObject.AddComponent<TextMeshProUGUI>();
            overlay.raycastTarget = false;
            overlay.fontSizeMin = 1f;
            overlay.fontSizeMax = 24f;
            overlay.enableAutoSizing = true;
            overlay.alignment = TextAlignmentOptions.MidlineLeft;

            EventTrigger eventHandler = cellRect.gameObject.AddComponent<EventTrigger>();

            return new ConsoleCell(this, index, eventHandler, background, overlay, command);
        }

        /// <summary>
        /// Destroys all cells.
        /// </summary>
        protected override void DestroyUI()
        {
            if (pv_Cells == null)
                return;

            ResetIndicies();
            for (int i = 0; i < pv_Cells.Length; i++)
                Destroy(pv_Cells[i].Core.gameObject);

            pv_Cells = null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>True if uparrow is pressed down.</returns>
        protected override bool NavigatesUp() => Input.GetKeyDown(KeyCode.UpArrow);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>True if downarrow is pressed down.</returns>
        protected override bool NavigatesDown() => Input.GetKeyDown(KeyCode.DownArrow);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>Always returns false.</returns>
        protected override bool NavigatesLeft() => Input.GetKeyDown(KeyCode.LeftArrow);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>Always returns false.</returns>
        protected override bool NavigatesRight() => Input.GetKeyDown(KeyCode.RightArrow);

        /// <summary>
        /// Might do this later
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected override bool NavigateAccepts(out int type)
        {
            type = -1;
            if (Input.GetKeyDown(KeyCode.Return) || (Input.GetMouseButtonDown(0) && Index != -1))
            {
                type = 0;
                return true;
            }

            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="cell"></param>
        protected override void Event_Cell_OnPointerEnter(ConsoleCell cell)
        {
            base.Event_Cell_OnPointerEnter(cell);
            
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="cell"></param>
        protected override void Event_Cell_OnPointerExit(ConsoleCell cell)
        {
            base.Event_Cell_OnPointerExit(cell);
            
        }

        private void Event_UpdateFullText(string text)
        {
            pv_FullText.SetText(Console.FullText, true);
            
            pv_FullText.rectTransform.offsetMin = new Vector2(0f, 0f);
            pv_FullText.rectTransform.offsetMax = new Vector2(0f, pv_FullText.GetPreferredValues().y);
        }

        private void Event_UpdateOnInputTextChanged(string text)
        {
            //pv_InputText.text = pv_InputText.text.ToUpper();

            if ((Index == -1 && BrowseIndex == -1) || string.IsNullOrEmpty(text))
            {
                Console.ForceSetInput(text, out _);
                RebuildUI();
            }
        }

        private void OnNavigationIndexUpdate()
        {
            if(BrowseIndex != -1)
            {
                pv_InputText.text = Console.PreviousInputs[BrowseIndex];
            }
            else if(Index != -1)
            {
                pv_InputText.text = pv_Cells[Index].CellCommand.Recognition + '/' + pv_Cells[Index].CellCommand.ArgumentHelper;
            }
        }

        private void ResetIndicies()
        {
            Index = SelectionIndex = BrowseIndex = -1;
        }

        private IEnumerator RefreshChecker()
        {
            ResetIndicies();
            SelectInput();

            while (true)
            {
                yield return pv_ColorUpdateWaiter;

                if (pv_PreviousConsoleColor != s_ConsoleColor)
                {
                    pv_PreviousConsoleColor = s_ConsoleColor;
                    Refresh();
                }
            }
        }

        private void RebuildUI()
        {
            DestroyUI();
            ForceBuildUI();
        }

        private void SelectInput()
        {
            EventSystem.current.SetSelectedGameObject(pv_InputText.gameObject, null);
            pv_InputText.OnPointerClick(new PointerEventData(EventSystem.current));
            pv_InputText.MoveToEndOfLine(false, false);
            pv_InputText.Select();
        }

        //This is the index which changes when uparrow is pressed in the console to browse previous entries.
        private int BrowseIndex
        {
            get => pv_BrowseIndex;
            set
            {
                pv_BrowseIndex = value;
                
                if (pv_BrowseIndex < -1)
                    pv_BrowseIndex = Console.PreviousInputs.Count - 1;
                else if (pv_BrowseIndex >= Console.PreviousInputs.Count)
                    pv_BrowseIndex = -1;
            }
        }

        [SerializeField]
        private Color s_ConsoleColor = Hooks.Colors.Tangelo;
        private Color pv_PreviousConsoleColor = Hooks.Colors.Tangelo;

        private Image pv_Background;
        private SlickBorder pv_BackgroundBorder;

        private Image pv_InputBackground;
        private SlickBorder pv_InputBorder;
        private TMP_InputField pv_InputText;
        private TextMeshProUGUI pv_BackInputText;

        private ScrollRect pv_FTRect;
        private Mask pv_FTMask;
        private TextMeshProUGUI pv_FullText;

        private RectTransform pv_CellsRect;

        private readonly WaitForSecondsRealtime pv_ColorUpdateWaiter = new WaitForSecondsRealtime(0.1f);
        private Coroutine pv_ColorUpdaterRoutine;

        private SortedDictionary<int, Command> pv_RecognitionCommandsRaw;
        private Command[] pv_RecognitionCommands;
        private ConsoleCell[] pv_Cells;
        private IntRange pv_CellsRange;

        private int pv_BrowseIndex;

        private float pv_FullTextFontSize = 24f;
        //This is probably just temporary to remember
        private float pv_Cellheight = 0.075f;
    }
}
