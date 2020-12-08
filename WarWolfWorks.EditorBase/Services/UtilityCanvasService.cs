using System;
using WarWolfWorks.EditorBase.Interfaces;
using static WarWolfWorks.EditorBase.Constants;
using WarWolfWorks.Internal;
using UnityEditor;
using WarWolfWorks.Utility;
using UnityEngine;

namespace WarWolfWorks.EditorBase.Services
{
    internal sealed class UtilityCanvasService : IService
    {
        public string Name => ELS_UtilityCanvas;

        private Settings.UtilityCanvasType CanvasType;
        private string CanvasResourcesPath, CanvasNameLoad;

        private readonly LanguageString LS_UCLoadType =
           new LanguageString("Load Mode", ("Tryb Ładowania", SystemLanguage.Polish), ("ロードモード", SystemLanguage.Japanese));
        private readonly LanguageString LS_UCPath =
            new LanguageString("Resources Path", ("Ścieszka Zasobów", SystemLanguage.Polish), ("リソースフォルダ", SystemLanguage.Japanese));
        private readonly LanguageString LS_UCName =
            new LanguageString("Name", ("Nazwa", SystemLanguage.Polish), ("名", SystemLanguage.Japanese));

        public void OnEnable()
        {
            CanvasType = Settings.GetUtilityCanvasType();
            CanvasResourcesPath = Settings.GetUtilityCanvasResourcesPath();
            CanvasNameLoad = Settings.GetUtilityCanvasNameLoad();
        }

        public void Draw()
        {
            CanvasType = (Settings.UtilityCanvasType)EditorGUILayout.EnumPopup(LS_UCLoadType, CanvasType);
            switch (CanvasType)
            {
                case Settings.UtilityCanvasType.PREFABBED:
                    CanvasResourcesPath = EditorGUILayout.TextField(LS_UCPath, CanvasResourcesPath);
                    break;
                case Settings.UtilityCanvasType.BY_NAME_IN_SCENE:
                    CanvasNameLoad = EditorGUILayout.TextField(LS_UCName, CanvasNameLoad);
                    break;
            }
        }

        public void OnDisable()
        {
            Settings.SaveUtilityCanvasType(CanvasType);
            Settings.SaveUtilityCanvasResourcesPath(CanvasResourcesPath);
            Settings.SaveUtilityCanvasNameLoad(CanvasNameLoad);
        }
    }
}
