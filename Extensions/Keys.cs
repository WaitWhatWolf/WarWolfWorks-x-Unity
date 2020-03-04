using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarWolfWorks.Utility;

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
                    if(Getter)
                    {
                        Checked = true;
                        return Getter;
                    }
                    Getter = FindObjectOfType<Keys>();
                    if (!Getter)
                        Getter = new GameObject(KEYS_OBJECT_NAME).AddComponent<Keys>();

                    DontDestroyOnLoad(Getter);

                    List<DefaultKeys.WKey> wKeys = DefaultKeys.GetAllKeys();
                    SKeys = new Dictionary<string, KeyState>(wKeys.Count);
                    foreach (DefaultKeys.WKey key in wKeys)
                        SKeys.Add(key.Name, KeyState.None);
                }

                return Getter;
            }
        }

        /// <summary>
        /// Gets the state of a key under the given name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public KeyState this[string key] => SKeys[key];

        private static Dictionary<string, KeyState> SKeys;

        private void Update()
        {
            foreach (KeyValuePair<string, KeyState> entry in SKeys)
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