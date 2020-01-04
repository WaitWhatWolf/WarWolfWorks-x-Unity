namespace WarWolfWorks.Maps
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using WarWolfWorks.Internal;
    using WarWolfWorks.Utility;

    public static class MapManager
    {
        internal struct MMMap
        {
            internal Map Map
            {
                get;
                set;
            }

            private List<MapObject> objects;
            internal List<MapObject> Objects
            {
                get
                {
                    return objects;
                }
                set
                {
                    objects = value;
                }
            }

            internal void RemoveNull()
            {
                Objects.RemoveAll(obj => obj.Object == null);
            }
        }

        internal class MapObject
        {
            internal GameObject Object;
            internal bool DestroyOnUnload;
            internal bool Disablable;

            internal MapObject(GameObject obj, bool destroy, bool disablable)
            {
                Object = obj;
                DestroyOnUnload = destroy;
                Disablable = disablable;
            }

        }

        private enum LoadType
        {
            awake,
            waitFrames,
            none
        }

        private static readonly string SavePath = Path.Combine(Application.streamingAssetsPath.Replace("/", "\\"), "Maps.kfidk");

        private static readonly string CategoryName = "MapsConfig";

        private const string EncryptionKey = "MapsUnite!";

        private static bool LoadsStartingMap;

        public static Map StartingMap;

        private static RuntimeInitializeLoadType StartingSceneLoadType;

        private static string[] NameFixer => new string[2]
        {
        "MPF_",
        "(Clone)"
        };

        internal static List<MMMap> CurrentMaps
        {
            get;
            private set;
        } = new List<MMMap>();


        public static event Action<Map> OnMapLoaded;

        public static event Action<Map> OnMapUnloaded;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StarterASL()
        {
            if (StartingSceneLoadType == RuntimeInitializeLoadType.AfterSceneLoad)
            {
                LoadBaseVars();
                LoadStarterMap();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StarterBSL()
        {
            if (StartingSceneLoadType == RuntimeInitializeLoadType.BeforeSceneLoad)
            {
                LoadBaseVars();
                LoadStarterMap();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void StarterAAL()
        {
            if (StartingSceneLoadType == RuntimeInitializeLoadType.AfterAssembliesLoaded)
            {
                LoadBaseVars();
                LoadStarterMap();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void StarterBSS()
        {
            if (StartingSceneLoadType == RuntimeInitializeLoadType.BeforeSplashScreen)
            {
                LoadBaseVars();
                LoadStarterMap();
            }
        }

        private static void LoadBaseVars()
        {
            try
            {
                StartingMap = Resources.Load<Map>(Hooks.Streaming.Load(SavePath, CategoryName, "StartingMapPath"));
                string variableFromCategory = Hooks.Streaming.Load(SavePath, CategoryName, "LoadsStartingMap");
                LoadsStartingMap = Convert.ToBoolean(variableFromCategory);
                StartingSceneLoadType = Hooks.Parse<RuntimeInitializeLoadType>(Hooks.Streaming.Load(SavePath, CategoryName, "SSLT"));
            }
            catch
            {
                if (StartingMap == null)
                    AdvancedDebug.LogException(new StartingMapIncorrectLocationException());
                else AdvancedDebug.LogException(new WWWException("Couldn't load start-up variables of MapManager. Make sure all input in WWWLibrary/Map Manager is correct."));
            }
        }

        private static void LoadStarterMap()
        {
            if (LoadsStartingMap)
            {
                LoadMap(StartingMap);
            }
        }

        [Obsolete("This is currenlty not working. (WIP)")]
        public static void LoadMapAsync(Action<MapLoadProgress> progressCatch, Map map)
        {
            GameObject g = new GameObject("WWWAsyncLoader");
            AsyncLoader al = g.AddComponent<AsyncLoader>();
            al.StartCoroutine(MapLoader(al, map, progressCatch));
        }

        [Obsolete("This is currenlty not working.")]
        private static IEnumerator<MapLoadProgress> MapLoader(AsyncLoader loader, Map map, Action<MapLoadProgress> catcher)
        {
            yield return null;
            
            /*MapLoadProgress mlp = new MapLoadProgress();
            if (map == null)
            {
                loader.StopCoroutine(MapLoader(loader, map, catcher));
            }
            if (map == StartingMap)
            {
                mlp.Loading = "Destroying Un-needed assets...";
                for (int i = 0; i < Components.Count; i++)
                {
                    mlp.Loading = $"Destroying {Components[i].CurrentMMGO.GameObject.name}...";
                    UnityEngine.Object.Destroy(Components[i].CurrentMMGO.GameObject);
                    mlp.CurrentProgress += 10 / Components.Count;
                    catcher?.Invoke(mlp);
                    yield return null;
                }

                mlp.Loading = "Clearing Maps...";
                CurrentMaps.Clear();
                yield return null;

                mlp.Loading = "Clearing Components...";
                Components.Clear();
                yield return null;

                mlp.Loading = "Resetting Callbacks...";
                OnMapLoaded = OnMapUnloaded = null;
                MMGOCounter = 0;
            }
            Map map2 = UnityEngine.Object.Instantiate(map);
            mlp.Loading = "Cloning Map Components...";
            map2.MapComponents = (map2.MapComponents.Clone() as MapComponent[]);
            int num = 0;
            int mMGOCounter = MMGOCounter;
            for (int i = 0; i < map2.MapComponents.Length; i++)
            {
                MapComponent item = map2.MapComponents[i];
                mlp.Loading = $"Instantiating {item.ToLoad} to ID:{MMGOCounter}...";
                item.CurrentMMGO = new MapComponent.MMGO
                {
                    GameObject = UnityEngine.Object.Instantiate(item.ToLoad, item.UsesCustomPosition ? item.CustomPosition : item.ToLoad.transform.position, item.UsesCustomRotation ? Quaternion.Euler(item.CustomRotation) : item.ToLoad.transform.rotation),
                    Index = MMGOCounter
                };
                if (item.FixName)
                {
                    item.CurrentMMGO.GameObject.name = item.CurrentMMGO.GameObject.name.RemoveArrayFromString(NameFixer);
                }
                num++;
                MMGOCounter++;
                Components.Add(item);
                yield return null;
            }

            mlp.Loading = "Creating MMMap...";
            MMMap mMMap = default;
            mMMap.Map = map2;
            mMMap.IndexCount = num;
            mMMap.StartingIndex = mMGOCounter;
            yield return null;
            mlp.Loading = "Cloning MMMap...";
            MMMap item2 = mMMap;
            yield return null;

            mlp.Loading = "Adding MMMap to database...";
            CurrentMaps.Add(item2);
            yield return null;

            OnMapLoaded?.Invoke(map2);
            mlp.Loading = "Loading complete!";*/
        }

        public static void LoadMap(Map map)
        {
            try
            {
                if (map == null)
                {
                    return;
                }
                if (map == StartingMap)
                {
                    ResetManager(true);
                }
                Map map2 = UnityEngine.Object.Instantiate(map);
                map2.MapComponents = (map2.MapComponents.Clone() as MapComponent[]);

                List<MapObject> objects = new List<MapObject>(map2.MapComponents.Length);

                for (int i = 0; i < map2.MapComponents.Length; i++)
                {
                    MapComponent mc = map2.MapComponents[i];
                    GameObject g = UnityEngine.Object.Instantiate(mc.ToLoad,
                        mc.UsesCustomPosition ? mc.CustomPosition : mc.ToLoad.transform.position,
                        Quaternion.Euler(mc.UsesCustomRotation ? mc.CustomRotation : mc.ToLoad.transform.eulerAngles));

                    NameFixer.ForEach(s => g.name.Replace(s, string.Empty));
                    objects.Add(new MapObject(g, mc.DestroyOnUnload, mc.Disablable));
                }

                CurrentMaps.Add(new MMMap() { Map = map2, Objects = objects });
                OnMapLoaded?.Invoke(map2);
            }
            catch
            {
                AdvancedDebug.LogException(new WWWException($"{map.MapName} could not be loaded"));
            }
        }

        public static void EnableMap(Map map) => EnableMap(map.MapName);
        public static void EnableMap(string mapName)
            => SetActiveMapState(mapName, true);

        public static void EnableAllMaps()
            => CurrentMaps.ForEach(m => SetActiveMapState(m, true));

        public static void DisableMap(Map map) => DisableMap(map.MapName);
        /// <summary>
        /// Disables all objects instantiated by specified map.
        /// </summary>
        /// <param name="mapName"></param>
        public static void DisableMap(string mapName)
            => SetActiveMapState(mapName, false);

        /// <summary>
        /// Disables all objects which were instantiated by maps.
        /// </summary>
        public static void DisableAllMaps()
            => CurrentMaps.ForEach(m => SetActiveMapState(m, false));

        private static void SetActiveMapState(string mapName, bool to)
        {
            MMMap used = GetMapByName(mapName);
            SetActiveMapState(used, to);
        }

        private static void SetActiveMapState(MMMap used, bool to)
        {
            used.RemoveNull();
            used.Objects.ForEach(g => { if (g.Disablable) g.Object.SetActive(to); });
        }

        public static void UnloadAllMaps()
        {
            for (short num = 0; num < CurrentMaps.Count; num++)
            {
                RemoveSelectedMap(CurrentMaps[num]);
            }
        }

        internal static MMMap GetMapByName(string mapName) => CurrentMaps.Find(t => t.Map.MapName == mapName);

        public static void UnloadMap(Map map) => UnloadMap(map.MapName);
        public static void UnloadMap(string mapName)
        {
            try
            {
                RemoveSelectedMap(GetMapByName(mapName));
            }
            catch
            {
                AdvancedDebug.LogException(new WWWException("Make sure the map passed is not null."));
            }
        }

        private static void RemoveSelectedMap(MMMap map)
        {
            try
            {
                map.RemoveNull();
                for (int i = 0; i < map.Objects.Count; i++)
                {
                    if (map.Objects[i].DestroyOnUnload)
                    {
                        UnityEngine.Object.Destroy(map.Objects[i].Object);
                    }

                }
                OnMapUnloaded?.Invoke(map.Map);
                CurrentMaps.Remove(map);
            }
            catch
            {
                AdvancedDebug.LogException(new WWWException($"Couldn't unload {map.Map.MapName}"));
            }
        }

        /// <summary>
        /// Resets all variables of the MapManager.
        /// </summary>
        /// <param name="forceDestroyAll">If true, all objects spawned by the manager will be destroyed.</param>
        public static void ResetManager(bool forceDestroyAll)
        {
            if (forceDestroyAll)
            {
                UnloadAllMaps();
            }
            CurrentMaps.Clear();
            OnMapLoaded = OnMapUnloaded = null;
        }
    }

}
