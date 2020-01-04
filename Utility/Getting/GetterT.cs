using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.Utility.Getting
{
    public static class Getter<T> where T : MonoBehaviour
    {
        private static List<IGetter<T>> AllGetterObjects = new List<IGetter<T>>();

        public static T Draw()
        {
            return AllGetterObjects.Find(getter => getter.Drawer is T)?.Drawer;
        }

        public static void AddDraw(IGetter<T> draw)
        {
            AllGetterObjects.RemoveAll(g => g == null);
            if(Draw() != null)
            {
                AdvancedDebug.LogWarning($"A IGetter of type {draw.GetType().Name} already exists! Make sure there are no duplicates of the object in the scene.", AdvancedDebug.WWWInfoLayerIndex);
                return;
            }
            AllGetterObjects.Add(draw);
        }

        public static void RemoveDraw(IGetter<T> draw)
        {
            AllGetterObjects.Remove(draw);
        }

    }
}
