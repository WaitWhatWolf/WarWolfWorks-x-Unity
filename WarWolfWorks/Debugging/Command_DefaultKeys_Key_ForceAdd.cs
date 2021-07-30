using System;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    [DeveloperCommand]
    internal sealed class Command_DefaultKeys_Key_ForceAdd : Command
    {
        public override string Recognition { get; } = CONVAR_SETTINGS + "DEFAULTKEYS_KEY_FORCEADD";

        public override string ArgumentHelper { get; } = "<string> <string>";

        public override string Description { get; } = "Adds a key regardless of optimized state.";

        public override void OnPassed(string arg)
        {
            try
            {
                string[] args = arg.Split(' ');
                DefaultKeys.ForceAddKey(args[0], Hooks.Parse<KeyCode>(args[1]));
            }
            catch (Exception e) { AdvancedDebug.LogException(e); }
        }
    }
}
