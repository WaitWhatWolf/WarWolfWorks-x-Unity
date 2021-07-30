using System;
using System.Collections.Generic;
using System.Text;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    internal sealed class Command_DefaultKeys_OptimizedState_Change : Command
    {
        public override string Recognition { get; } = CONVAR_SETTINGS + "DEFAULTKEYS_OPTIMIZEDSTATE_CHANGE";

        public override string Description { get; } = "Enables/Disables the optimized state of DefaultKeys.";

        public override string ArgumentHelper { get; } = "<bool>";

        public override void OnPassed(string arg)
        {
            try
            {
                bool optimize = Convert.ToBoolean(arg);
                if (optimize) DefaultKeys.Optimize();
                else DefaultKeys.Unoptimize();
            }
            catch (Exception e) { e.LogException(); }
        }
    }
}
