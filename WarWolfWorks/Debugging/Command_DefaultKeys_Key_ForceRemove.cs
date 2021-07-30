using System;
using WarWolfWorks.Attributes;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    [DeveloperCommand]
    internal sealed class Command_DefaultKeys_Key_ForceRemove : Command
    {
        public override string Recognition { get; } = CONVAR_SETTINGS + "DEFAULTKEYS_KEY_FORCEREMOVE";

        public override string ArgumentHelper { get; } = "<string>";

        public override string Description { get; } = "Forcefully removes a key regardless of optimized state.";

        public override void OnPassed(string arg)
        {
            try
            {
                DefaultKeys.ForceRemoveKey(arg);
            }
            catch (Exception e) { AdvancedDebug.LogException(e); }
        }
    }
}
