using System;
using WarWolfWorks.Attributes;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    [DeveloperCommand]
    internal sealed class Command_DefaultKeys_Key_Change_Name : Command
    {
        public override string Recognition { get; } = CONVAR_SETTINGS + "DEFAULTKEYS_KEY_CHANGE_NAME";

        public override string Description { get; } = "Modifies an existing key's name.";

        public override string ArgumentHelper { get; } = "<string> <string>";

        public override void OnPassed(string arg)
        {
            try
            {
                string[] args = arg.Split(' ');
                DefaultKeys.WKey key = DefaultKeys.GetAllKeys().Find(k => k.Name.ToUpper() == args[0]);
                DefaultKeys.ChangeKeyName(key.Name, args[1]);
            }
            catch (Exception e) { AdvancedDebug.LogException(e); }
        }
    }
}
