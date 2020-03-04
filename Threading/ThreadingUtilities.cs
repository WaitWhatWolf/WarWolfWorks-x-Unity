using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.Threading
{
    /// <summary>
    /// Contains various utilities to make your life easier with Unity's partial non-support of multithreading.
    /// </summary>
    public sealed class ThreadingUtilities : Singleton<ThreadingUtilities>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            AdvancedDebug.LogFormat("{0} is taking care of multi-threading, make sure not to delete it if you are using ThreadingUtilities!",
                AdvancedDebug.DEBUG_LAYER_WWW_INDEX, Instance);
        }

        private static List<Action> AllActions = new List<Action>();

        /// <summary>
        /// Queues an action to the main thread.
        /// </summary>
        /// <param name="action"></param>
        public static void QueueOnMainThread(Action action)
        {
            lock (AllActions)
            {
                AllActions.Add(action);
            }
        }

        private Action[] PerformedActions = new Action[1000];

        private void Update()
        {
            lock (AllActions)
            {
                for (int i = 0; i < AllActions.Count; i++)
                {
                    AllActions[i]();
                }
                AllActions.Clear();
            }
        }

        /// <summary>
        /// Returns "WarWolfWorks Threader".
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "WarWolfWorks Threader";
        }
    }
}
