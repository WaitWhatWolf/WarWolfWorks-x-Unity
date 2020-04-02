using UnityEngine;
using WarWolfWorks.EditorBase.Interfaces;
using WarWolfWorks.Utility;
using static WarWolfWorks.EditorBase.Constants;
using WarWolfWorks.Internal;
using UnityEditor;
using System;

namespace WarWolfWorks.EditorBase.Services
{
    internal sealed class MiscService : IService
    {
        public string Name => ELS_Misc;

        private readonly LanguageString LS_OLanguage =
            new LanguageString("Language", ("Język", SystemLanguage.Polish), ("言語", SystemLanguage.Japanese));

        private readonly LanguageString LS_OStacking =
            new LanguageString("Default Stacking Display Type", 
                ("Domyślnie wyświetlany typ Stacking", SystemLanguage.Polish), 
                ("デフォルトのスタッキング表示タイプ", SystemLanguage.Japanese));

        private readonly LanguageString LS_OAffections =
            new LanguageString("Default Affections Display Type", 
                ("Domyślnie wyświetlany typ Affections", SystemLanguage.Polish), 
                ("デフォルトのアフェクシィオン表示タイプ", SystemLanguage.Japanese));

        private SystemLanguage Language;
        private string StackingName;
        private string AffectionsName;

        public void OnEnable()
        {
            Language = Settings.LibraryLanguage;
            StackingName = Settings.DefaultStackingType?.ToString();
            AffectionsName = Settings.DefaultAffectionsType?.ToString();
        }

        public void Draw()
        {
            Language = (SystemLanguage)EditorGUILayout.EnumPopup(LS_OLanguage, Language);
            if (Language != Settings.LibraryLanguage)
                Settings.LibraryLanguage = Language; //Saved automatically.

            DisplayType(ref StackingName, false);
            DisplayType(ref AffectionsName, true);
        }

        private void DisplayType(ref string @for, bool forAffections)
        {
            @for = EditorGUILayout.TextField(forAffections ? LS_OAffections : LS_OStacking, @for);
            try
            {
                Type tmpType = Hooks.ParseType(@for);
                Type toChange = forAffections ? Settings.DefaultAffectionsType : Settings.DefaultStackingType;
                if (toChange == null || (tmpType != null && tmpType.IsSubclassOf(typeof(Enum))))
                {
                    if (tmpType != Settings.DefaultAffectionsType)
                    {
                        if (!forAffections)
                            Settings.DefaultStackingType = tmpType;
                        else
                            Settings.DefaultAffectionsType = tmpType;
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Couldn't parse the type! Make sure you include the namespace and that" +
                        " the type given is an Enum.", UnityEditor.MessageType.Warning);
                }
            }
            catch
            {

            }
        }

        public void OnDisable() { }
    }
}
