using System;
using System.Collections.Generic;
using System.Text;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    internal sealed class Command_DefaultKeys_Apply : Command
    {
        public override string Recognition { get; } = CONVAR_SETTINGS + "DEFAULTKEYS_APPLY";

        public override string Description { get; } = "Any changes made using DefaultKeys commands are reset upon restarting the game; This command saves changes to the disk so that any change made to DefaultKeys persists.";

        public override string ArgumentHelper { get; } = string.Empty;

        public override void OnPassed(string arg)
        {
            DefaultKeys.Apply();
        }
    }
}
