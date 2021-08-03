using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using WarWolfWorks.UI.ConsoleMenu;
using WarWolfWorks.UI.MenusSystem;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    /// <summary>
    /// Changes the color of <see cref="ConsoleUI"/>.
    /// </summary>
    public sealed class Command_Console_Change_Color : Command
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Recognition => CONVAR_UI + "CONSOLE_CHANGE_COLOR";
        
        /// <summary>
        /// Shows that it accepts a string value.
        /// </summary>
        public override string ArgumentHelper => "<string>";

        /// <summary>
        /// Displays all available colors.
        /// </summary>
        public override string Description
        {
            get
            {
                string toReturn = "Changes the color of the console; All possible colors:";
                string toAdd = string.Empty;
                IEnumerable<string> colorsNames = pv_ColorFields.Keys;
                foreach(string colorName in colorsNames)
                {
                    toAdd += ", " + colorName;
                }

                return toReturn + ' ' + toAdd.Remove(0, 2);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="arg"></param>
        public override void OnPassed(string arg)
        {
            arg = arg.ToUpper();

            ConsoleUI consoleUI = Menu.GetMenu<ConsoleUI>();

            Color foundColor = Color.clear;

            for(int i = 0; i < pv_ColorFields.Count; i++)
            {
                if(pv_ColorFields.ContainsKey(arg))
                {
                    foundColor = pv_ColorFields[arg];
                }
            }

            if(foundColor == Color.clear)
            {
                Console.Write("Color not found.", MessageType.Warning);
                return;
            }

            consoleUI.SetConsoleColor(foundColor);
        }

        private Dictionary<string, Color> pv_ColorFields = new();

        /// <summary>
        /// Base constructor.
        /// </summary>
        public Command_Console_Change_Color()
        {
            FieldInfo[] pv_ColorFieldsArray = typeof(Hooks.Colors).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance | BindingFlags.Static);

            for (int i = 0; i < pv_ColorFieldsArray.Length; i++)
            {
                object value = pv_ColorFieldsArray[i].GetValue(null);
                if (value is Color color)
                {
                    pv_ColorFields.Add(pv_ColorFieldsArray[i].Name.ToUpper(), color);
                }
            }

            pv_ColorFields.Remove("BLACK"); //NGL, this was pretty hilarious when I tried
        }
    }
}
