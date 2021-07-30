using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    internal sealed class Command_DefaultKeys_Key_Change_Value : Command
    {
        public override string Recognition { get; } = CONVAR_SETTINGS + "DEFAULTKEYS_KEY_CHANGE_VALUE";

        public override string Description { get; } = "Modifies an existing key's value.";

        public override string ArgumentHelper { get; } = "<string> <string>";

        public override void OnPassed(string arg)
        {
            try
            {
                if (DefaultKeys.IsOptimized)
                {
                    AdvancedDebug.LogError("Cannot change a key's value while optimized state is active. Deactivate the optimized state first or use DEFAULTKEYS_KEY_FORCECHANGE_VALUE");
                }
                else
                {
                    string[] args = arg.Split(' ');
                    DefaultKeys.WKey key = DefaultKeys.GetAllKeys().Find(k => k.Name.ToUpper() == args[0]);
                    DefaultKeys.ChangeKeyValue(key.Name, Hooks.Parse<KeyCode>(args[1]));
                }
            }
            catch (Exception e) { AdvancedDebug.LogException(e); }
        }
    }
}
