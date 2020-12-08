using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarWolfWorks.Utility;
using static WarWolfWorks.Constants;

namespace WarWolfWorks.Extensions
{
    /// <summary>
    /// An extention class for a more reusable input detection. Depends on <see cref="DefaultKeys"/> for key names.
    /// </summary>
	public sealed class Keys : MonoBehaviour
	{
        private static bool Checked;
        private static Keys Getter;
        /// <summary>
        /// Use the indexer of this to access a key's state.
        /// (Example: Keys.Get["AwesomeKey"])
        /// </summary>
        public static Keys Get
        {
            get
            {
                if(!Checked)
                {
                    UpdateServicedKeys();

                    if (Getter)
                    {
                        Checked = true;
                        return Getter;
                    }
                    Getter = FindObjectOfType<Keys>();
                    if (!Getter)
                        Getter = new GameObject(KEYS_OBJECT_NAME).AddComponent<Keys>();

                    DontDestroyOnLoad(Getter);
                }

                return Getter;
            }
        }

        /// <summary>
        /// Updates all keys used by <see cref="Keys"/>; Draws all key names and values from <see cref="DefaultKeys"/>.
        /// </summary>
        public static void UpdateServicedKeys()
        {
            List<DefaultKeys.WKey> wKeys = DefaultKeys.GetAllKeys();
            SKeys.Clear();
            foreach (DefaultKeys.WKey key in wKeys)
                SKeys.Add(key.Name, KeyState.None);

            UsedKeys = SKeys.ToArray();
        }

        /// <summary>
        /// Gets the state of a key under the given name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public KeyState this[string key] => SKeys[key];

        private static Dictionary<string, KeyState> SKeys = new Dictionary<string, KeyState>();
        private static KeyValuePair<string, KeyState>[] UsedKeys;

        private void Update()
        {
            if (!Checked)
                return;

            try
            {
                foreach (KeyValuePair<string, KeyState> entry in UsedKeys)
                {
                    if (DefaultKeys.GetKeyDown(entry.Key))
                        SKeys[entry.Key] = KeyState.Pressed;
                    else if (DefaultKeys.GetKey(entry.Key))
                        SKeys[entry.Key] = KeyState.Held;
                    else if (DefaultKeys.GetKeyUp(entry.Key))
                        SKeys[entry.Key] = KeyState.Released;
                    else SKeys[entry.Key] = KeyState.None;
                }
            }
            catch
            {
                AdvancedDebug.LogWarning("There was a problem updating Keys; Refreshing...", DEBUG_LAYER_WWW_INDEX);
                UpdateServicedKeys();
            }
        }

        private const string KEYS_OBJECT_NAME = "Keys/Input Detector";

        /// <summary>
        /// Returns <see cref="KEYS_OBJECT_NAME"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return KEYS_OBJECT_NAME;
        }
    }
}