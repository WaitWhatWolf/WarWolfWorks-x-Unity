using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WarWolfWorks.Debugging;
using WarWolfWorks.Enums;
using WarWolfWorks.UI.MenusSystem.SlickMenu;

namespace WarWolfWorks.UI.ConsoleMenu
{
    /// <summary>
    /// A cell element used in <see cref="ConsoleUI"/>.
    /// </summary>
    public sealed class ConsoleCell : SCText<ConsoleUI, ConsoleCell, SlickBorder>
    {
        /// <summary>
        /// The command this cell displays.
        /// </summary>
        public Command CellCommand { get; private set; }

        /// <summary>
        /// Displays the command's recognition and argument helper.
        /// </summary>
        public override string OverlayContent => $"{CellCommand.Recognition}/{CellCommand.ArgumentHelper}";

        /// <summary>
        /// Returns <see cref="SlickBorderFlags.All"/>.
        /// </summary>
        public override SlickBorderFlags BorderFlags => SlickBorderFlags.All;

        /// <summary>
        /// Base <see cref="ConsoleCell"/> constructor.
        /// </summary>
        public ConsoleCell(ConsoleUI parent, int index, EventTrigger eventHandler, Image background, TextMeshProUGUI text, Command command) : base(parent, index, eventHandler, background, text)
        {
            CellCommand = command;
        }
    }
}
