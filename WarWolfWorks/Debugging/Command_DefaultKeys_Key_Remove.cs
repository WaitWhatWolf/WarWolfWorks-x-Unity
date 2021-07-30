using System;
using WarWolfWorks.Attributes;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    [DeveloperCommand]
    internal sealed class Command_DefaultKeys_Key_Remove : Command
    {
        public override string Recognition { get; } = CONVAR_SETTINGS + "DEFAULTKEYS_KEY_REMOVE";

        public override string ArgumentHelper { get; } = "<string>";

        public override string Description { get; } = "Attempts to remove an existing key.";

        public override void OnPassed(string arg)
        {
            try
            {
                if (DefaultKeys.IsOptimized)
                {
                    AdvancedDebug.LogError("Cannot remove a key while optimized state is active. Deactivate the optimized state first or use DEFAULTKEYS_KEY_FORCEREMOVE");
                }
                else
                {
                    DefaultKeys.RemoveKey(arg);
                }
            }
            catch (Exception e) { AdvancedDebug.LogException(e); }
        }
    }
}
