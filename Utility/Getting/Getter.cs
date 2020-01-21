using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.Utility.Getting
{
	public static class Getter
	{
        private static List<IGetter> AllGetterObjects = new List<IGetter>();

        public static MonoBehaviour GetGetter(string name)
            => AllGetterObjects.Find(ig => ig.名前 == name)?.Drawer;

        public static void AddGetter(IGetter g)
        {
            AllGetterObjects.RemoveAll(ig => ig == null);
            if (GetGetter(g.名前))
            {
                AdvancedDebug.LogWarning($"IGetter under the name {g.名前} already exists! Please use another name.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);
                return;
            }
            AllGetterObjects.Add(g);
        }

        public static void RemoveGetter(IGetter g)
        {
            AllGetterObjects.Remove(g);
        }
	}
}