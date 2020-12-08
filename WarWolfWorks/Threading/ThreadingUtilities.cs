using System;
using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.Threading
{
    /// <summary>
    /// Contains various utilities to make your life easier with Unity's partial non-support of multithreading.
    /// </summary>
    public sealed class ThreadingUtilities : MonoSingleton<ThreadingUtilities>
    {
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
