using System;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    [DeveloperCommand]
    internal sealed class Command_DefaultKeys_Key_Add : Command
    {
        public override string Recognition { get; } = CONVAR_SETTINGS + "DEFAULTKEYS_KEY_ADD";

        public override string ArgumentHelper { get; } = "<string> <string>";

        public override string Description { get; } = "Attempts to add a key; If a key under the given name already exists, it's value will be changed instead.";

        public override void OnPassed(string arg)
        {
            try
            {
                if (DefaultKeys.IsOptimized)
                {
                    AdvancedDebug.LogError("Cannot add a key while optimized state is active. Deactivate the optimized state first or use DEFAULTKEYS_KEY_FORCEADD");
                }
                else
                {
                    string[] args = arg.Split(' ');
                    DefaultKeys.AddKey(args[0], Hooks.Parse<KeyCode>(args[1]));
                }
            }
            catch (Exception e) { AdvancedDebug.LogException(e); }
        }
    }
}
