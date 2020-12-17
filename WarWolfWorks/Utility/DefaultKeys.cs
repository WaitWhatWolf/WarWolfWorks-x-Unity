using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using WarWolfWorks.IO.CTS;
using static WarWolfWorks.WWWResources;

[assembly: InternalsVisibleTo("WarWolfWorks.EditorBase")]
namespace WarWolfWorks.Utility
{
    /// <summary>
    /// A more customizable Input system compared to the <see cref="UnityEngine.Input"/> class.
    /// </summary>
    public static class DefaultKeys
    {
        /// <summary>
        /// Struct used to store keys and identify them by name.
        /// </summary>
        public struct WKey
        {
            /// <summary>
            /// Name of the <see cref="WKey"/>.
            /// </summary>
            public string Name;
            /// <summary>
            /// Value of the <see cref="WKey"/>.
            /// </summary>
            public KeyCode Key;

            /// <summary>
            /// Base constructor of the <see cref="WKey"/>.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="key"></param>
            public WKey(string name, KeyCode key)
            {
                Name = name;
                Key = key;
            }

            /// <summary>
            /// Constructor based off of <see cref="Hooks.Streaming"/> save lines.
            /// </summary>
            /// <param name="variable"></param>
            internal WKey(Variable variable)
            {
                Name = variable.Name;
                Key = Hooks.Parse<KeyCode>(variable.Value);
            }
        }

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
                List<WKey> keys = GetAllKeys();
                for(int i = 0; i < keys.Count; i++)
                {
                    OptimizedKeys.Add(keys[i].Name, keys[i].Key);
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
            return IsOptimized ? OptimizedKeys[key] : Hooks.Parse<KeyCode>(CTS_DefaultKeys[key]);
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
        public static List<WKey> GetAllKeys()
        {
            List<WKey> list = new List<WKey>();
            if (IsOptimized)
            {
                List<KeyValuePair<string, KeyCode>> temp = OptimizedKeys.ToList();
                for(int i = 0; i < temp.Count; i++)
                {
                    list.Add(new WKey(temp[i].Key, temp[i].Value));
                }
            }
            else
            {
                IReadOnlyList<Variable> variables = CTS_DefaultKeys.GetAllLines();
                foreach (Variable variable in variables)
                    list.Add(new WKey(variable));
            }
            return list;
        }

        private static bool IsOptimizedCheck()
        {
            if (IsOptimized)
            {
                AdvancedDebug.LogWarning("Cannot add a key while optimization mode is active! Use DefaultKeys.Unoptimize() and try again.", DEBUG_LAYER_WWW_INDEX);
            }
            return IsOptimized;
        }

        /// <summary>
        /// Returns true if key under keyName exists inside <see cref="DefaultKeys"/>.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool KeyExists(string keyName)
        {
            if (!IsOptimized)
            {
                return !string.IsNullOrEmpty(CTS_DefaultKeys[keyName]);
            }
            return OptimizedKeys.ContainsKey(keyName);
        }

        /// <summary>
        /// Adds a key to the database; If a key under <see cref="WKey.Name"/> already exists, it will only change that key's value with <see cref="WKey.Key"/>.
        /// </summary>
        /// <param name="key"></param>
        public static void AddKey(WKey key)
            => AddKey(key.Name, key.Key);

        internal static void DebugKeyAddition(string name, KeyCode value)
            => Debug.Log($"{name} Key was successfully saved as {value}!");

        /// <summary>
        /// Adds a key to the database; If a key under the given name already exists, it will only change that key's value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void AddKey(string name, KeyCode value)
        {
            if (!IsOptimizedCheck())
            {
                try
                {
                    Hooks.Streaming.CreateFolder(SV_Path_DefaultKeys);
                    CTS_DefaultKeys[name] = value.ToString();
                    CTS_DefaultKeys.Apply();
                    DebugKeyAddition(name, value);
                }
                catch (Exception ex)
                {
                    AdvancedDebug.Log($"A problem occured trying to save {name} key. (Error: {ex.Message})", DEBUG_LAYER_EXCEPTIONS_INDEX);
                }
            }
        }

        /// <summary>
        /// Forces <see cref="DefaultKeys"/> to add/change a key inside the database even if optimization mode is active.
        /// </summary>
        /// <param name="key"></param>
        public static void ForceAddKey(WKey key)
            => ForceAddKey(key.Name, key.Key);

        /// <summary>
        /// Forces <see cref="DefaultKeys"/> to add/change a key inside the database even if optimization mode is active.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        public static void ForceAddKey(string name, KeyCode key)
        {
            bool wasOptimized = IsOptimized;
            if (wasOptimized)
            {
                Unoptimize();
            }

            AddKey(name, key);

            if (wasOptimized)
            {
                Optimize();
            }
        }

        /// <summary>
        /// Changes a key's name.
        /// </summary>
        /// <param name="from">Current name of the key.</param>
        /// <param name="to">New name of the key.</param>
        public static bool ChangeKeyName(string from, string to)
        {
            if (!IsOptimizedCheck() && CTS_DefaultKeys.Rename(from, to))
            {
                AdvancedDebug.Log($"{from}'s name was successfully changed to {to}", DEBUG_LAYER_WWW_INDEX);
                return true;
            }

            AdvancedDebug.LogWarning($"{from}'s name couldn't be changed! Make sure a key under the given name exists and the new key name is not the same.", DEBUG_LAYER_WWW_INDEX);
            return false;
        }

        /// <summary>
        /// Removes a key from the database.
        /// </summary>
        /// <param name="key"></param>
        public static bool RemoveKey(string key)
        {
            if (!IsOptimizedCheck())
            {
                return CTS_DefaultKeys.Remove(key);
            }

            return false;
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

        /// <summary>
        /// Applies all changes made to <see cref="DefaultKeys"/> to it's save file (CTS).
        /// </summary>
        public static void Apply()
        {
            CTS_DefaultKeys.Apply();
        }
    }
}