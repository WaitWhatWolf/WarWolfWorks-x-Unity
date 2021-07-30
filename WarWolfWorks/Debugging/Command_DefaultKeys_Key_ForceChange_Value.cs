using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    internal sealed class Command_DefaultKeys_Key_ForceChange_Value : Command
    {
        public override string Recognition { get; } = CONVAR_SETTINGS + "DEFAULTKEYS_KEY_FORCECHANGE_VALUE";

        public override string Description { get; } = "Modifies an existing key's value regardless of optimized state.";

        public override string ArgumentHelper { get; } = "<string> <string>";

        public override void OnPassed(string arg)
        {
            try
            {
                string[] args = arg.Split(' ');
                DefaultKeys.WKey key = DefaultKeys.GetAllKeys().Find(k => k.Name.ToUpper() == args[0]);
                DefaultKeys.ForceChangeKeyValue(key.Name, Hooks.Parse<KeyCode>(args[1]));
            }
            catch (Exception e) { AdvancedDebug.LogException(e); }
        }
    }
}
