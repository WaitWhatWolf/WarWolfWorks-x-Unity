using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// A more customizable Input system compared to UnityEngine.Input class.
    /// </summary>
    public static class DefaultKeys
    {
        private static readonly string KeysPath = Path.Combine(Application.streamingAssetsPath.Replace("/", "\\"), "DefaultKeys.kfidk");

        private static readonly string CategoryName = "Keys";

        //private const string EncryptionKey = "KeysUnite!";

        private static Dictionary<string, KeyCode> OptimizedKeys = new Dictionary<string, KeyCode>();

        /// <summary>
        /// Is the optimization mode currently on?
        /// </summary>
        public static bool IsOptimized
        {
            get;
            private set;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Startup()
        {
            Optimize();
            Application.quitting += Unoptimize;
        }

        /// <summary>
        /// Optimizes the DefaultKeys system to store all keys into a Dictionary, 
        /// instead of directly Stream-Reading it from the file. (Automatically triggers on <see cref="RuntimeInitializeOnLoadMethodAttribute"/>.<see cref="RuntimeInitializeLoadType.BeforeSceneLoad"/>)
        /// </summary>
        public static void Optimize()
        {
            if (!IsOptimized)
            {
                (string, KeyCode)[] allKeys = GetAllKeys();
                for (int i = 0; i < allKeys.Length; i++)
                {
                    OptimizedKeys.Add(allKeys[i].Item1, allKeys[i].Item2);
                }
                IsOptimized = true;
            }
        }

        /// <summary>
        /// Unoptimizes the DefaultKeys system to make DefaultKeys read directly from file through Stream Reading.
        /// (Recommended only to add multiple keys to DefaultKeys through code)
        /// </summary>
        public static void Unoptimize()
        {
            if (IsOptimized)
            {
                OptimizedKeys.Clear();
                IsOptimized = false;
            }
        }

        /// <summary>
        /// Updates the stored list of optimized keys to be up to date with DefaultKeys file.
        /// </summary>
        public static void Reoptimize()
        {
            Unoptimize();
            Optimize();
        }

        /// <summary>
        /// Returns a <see cref="KeyCode"/> assigned with given name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static KeyCode GetDatabaseKey(string key)
        {
            return IsOptimized ? OptimizedKeys[key] : Hooks.Parse<KeyCode>(Hooks.Streaming.Load(KeysPath, CategoryName, key));
        }

        /// <summary>
        /// Returns a list of <see cref="KeyCode"/>s assigned with the given names.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static List<KeyCode> GetDatabaseKeys(params string[] keys)
        {
            List<KeyCode> toReturn = new List<KeyCode>();
            foreach (string s in keys)
                toReturn.Add(GetDatabaseKey(s));

            return toReturn;
        }

        /// <summary>
        /// Returns true if the <see cref="KeyCode"/> under keyName is held down.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool GetKey(string keyName)
        {
            return Input.GetKey(IsOptimized ? OptimizedKeys[keyName] : GetDatabaseKey(keyName));
        }

        /// <summary>
        /// Returns true if either is false and all given keys are held, or when either is true and any of the given keys is held.
        /// </summary>
        /// <param name="either"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool GetKey(bool either, params string[] keys)
        {
            foreach (string s in keys)
            {
                if (either && GetKey(s)) return true;
                else if (!either && !GetKey(s)) return false;
            }
            return !either;
        }

        /// <summary>
        /// Returns true if the <see cref="KeyCode"/> under keyName is lifted.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool GetKeyUp(string keyName)
        {
            return Input.GetKeyUp(IsOptimized ? OptimizedKeys[keyName] : GetDatabaseKey(keyName));
        }

        /// <summary>
        /// Returns true if either is false and all given keys are lifted, or when either is true and any of the given keys is lifted.
        /// </summary>
        /// <param name="either"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool GetKeyUp(bool either, params string[] keys)
        {
            foreach (string s in keys)
            {
                if (either && GetKeyUp(s)) return true;
                else if (!either && !GetKeyUp(s)) return false;
            }
            return !either;
        }

        /// <summary>
        /// Returns true if the <see cref="KeyCode"/> under keyName is pressed.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool GetKeyDown(string keyName)
        {
            return Input.GetKeyDown(IsOptimized ? OptimizedKeys[keyName] : GetDatabaseKey(keyName));
        }

        /// <summary>
        /// Returns true if either is false and all given keys are pressed, or when either is true and any of the given keys is pressed.
        /// </summary>
        /// <param name="either"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool GetKeyDown(bool either, params string[] keys)
        {
            foreach (string s in keys)
            {
                if (either && GetKeyDown(s)) return true;
                else if (!either && !GetKeyDown(s)) return false;
            }
            return !either;
        }

        /// <summary>
        /// Returns a <see cref="ValueTuple"/> array of all keys stored inside DefaultKeys. (If optimization is active, it will return all keys in Dictionary, otherwise returns directly from the file.)
        /// </summary>
        /// <returns></returns>
        public static (string, KeyCode)[] GetAllKeys()
        {
            List<(string, KeyCode)> list = new List<(string, KeyCode)>();
            if (IsOptimized)
            {
                string[] array = OptimizedKeys.Keys.ToArray();
                for (int i = 0; i < OptimizedKeys.Count; i++)
                {
                    list.Add((array[i], OptimizedKeys[array[i]]));
                }
            }
            else
            {
                string[] allVariablesFromCategory = Hooks.Streaming.LoadAll(KeysPath, CategoryName, true).ToArray();
                string[] array2 = new string[allVariablesFromCategory.Length];
                KeyCode[] array3 = new KeyCode[allVariablesFromCategory.Length];
                for (int j = 0; j < allVariablesFromCategory.Length; j++)
                {
                    string[] array4 = allVariablesFromCategory[j].Split(new char[1]
                    {
                    '='
                    });
                    array2[j] = array4[0];
                    array3[j] = Hooks.Parse<KeyCode>(array4[1]);
                    list.Add((array2[j], array3[j]));
                }
            }
            return list.ToArray();
        }

        private static bool IsOptimizedCheck()
        {
            if (IsOptimized)
            {
                AdvancedDebug.LogWarning("Cannot add a key while optimization mode is active! Use DefaultKeys.Unoptimize() and try again.", AdvancedDebug.WWWInfoLayerIndex);
            }
            return IsOptimized;
        }

        /// <summary>
        /// Returns true if key under keyName exists inside DefaultKeys. (If optimized, tries to find it inside the Dictionary, otherwise reads it directly from the file)
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool KeyExists(string keyName)
        {
            if (!IsOptimized)
            {
                return Hooks.Streaming.Load(KeysPath, CategoryName, keyName) != string.Empty;
            }
            return OptimizedKeys.ContainsKey(keyName);
        }

        /// <summary>
        /// Adds a key to the database.
        /// </summary>
        /// <param name="key"></param>
        public static void AddKey((string, KeyCode) key)
        {
            if (!IsOptimizedCheck())
            {
                try
                {
                    Hooks.Streaming.CreateFolder(KeysPath);
                    Hooks.Streaming.Save(KeysPath, CategoryName, key.Item1, key.Item2.ToString());
                    Debug.Log($"{key} Key was successfully saved in: {KeysPath}");
                }
                catch (Exception ex)
                {
                    AdvancedDebug.Log($"A problem occured trying to save {key} key, please try again. (Error: {ex.Message})", AdvancedDebug.ExceptionLayerIndex);
                }
            }
        }

        /// <summary>
        /// Forces DefaultKeys to add key into database even if optimization mode is active.
        /// </summary>
        /// <param name="key"></param>
        public static void ForceAddKey((string, KeyCode) key)
        {
            InternalKeymethodChanger(key, isAdd: true);
        }

        private static void InternalKeymethodChanger((string, KeyCode) key, bool isAdd)
        {
            bool isOptimized = IsOptimized;
            if (isOptimized)
            {
                Unoptimize();
            }
            if (isAdd)
            {
                AddKey(key);
            }
            else
            {
                ChangeKey(key);
            }
            if (isOptimized)
            {
                Optimize();
            }
        }

        /// <summary>
        /// Changes key's value under key.Item1 to key.Item2's value.
        /// </summary>
        /// <param name="key"></param>
        public static void ChangeKey((string, KeyCode) key)
        {
            if (!IsOptimizedCheck())
            {
                KeyCode keyCode = Hooks.Parse<KeyCode>(Hooks.Streaming.Load(KeysPath, CategoryName, key.Item1));
                if (keyCode != key.Item2)
                {
                    Hooks.Streaming.Save(KeysPath, CategoryName, key.Item1, key.Item2.ToString());
                    AdvancedDebug.Log($"{key.Item1} was successfully changed from {keyCode} to {key.Item2}", AdvancedDebug.WWWInfoLayerIndex);
                }
            }
        }

        /// <summary>
        /// Forces DefaultKeys to change key from database even if optimization mode is active.
        /// </summary>
        /// <param name="key"></param>
        public static void ForceChangeKey((string, KeyCode) key)
        {
            InternalKeymethodChanger(key, isAdd: false);
        }

        /// <summary>
        /// Removes a key from the database.
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveKey(string key)
        {
            if (!IsOptimizedCheck())
            {
                Hooks.Streaming.Remove(KeysPath, CategoryName, key);
            }
        }

        /// <summary>
        /// Forces DefaultKeys to remove key from database even if optimization mode is active.
        /// </summary>
        /// <param name="key"></param>
        public static void ForceRemoveKey(string key)
        {
            bool isOptimized = IsOptimized;
            if (isOptimized)
            {
                Unoptimize();
            }
            RemoveKey(key);
            if (isOptimized)
            {
                Optimize();
            }
        }
    }
}