using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace WarWolfWorks.Threading
{
    /// <summary>
    /// Contains various utilities to make your life easier with Unity's partial non-support of multithreading.
    /// </summary>
    public sealed class ThreadingUtilities : MonoBehaviour
    {
        private static ThreadingUtilities runner;
        private static bool runRunner;
        private static ThreadingUtilities Current
        {
            get
            {
                if (!runRunner)
                {
                    runner = new GameObject("(WWWLibrary)Threader").AddComponent<ThreadingUtilities>();
                    runRunner = true;
                }
                return runner;
            }
        }


        private List<Action> actions = new List<Action>();

        /// <summary>
        /// Queues an action to the main thread.
        /// </summary>
        /// <param name="action"></param>
        public static void QueueOnMainThread(Action action)
        {
            lock (Current.actions)
            {
                Current.actions.Add(action);
            }
        }

        /// <summary>
        /// Runs an action on a separate thread.
        /// </summary>
        /// <param name="a"></param>
        public static void RunAsync(Action a)
        {
            var t = new Thread(RunAction);
            t.Priority = System.Threading.ThreadPriority.Normal;
            t.Start(a);
        }

        private static void RunAction(object action)
        {
            ((Action)action)();
        }


        Action[] toBeRun = new Action[1000];

        void Update()
        {
            try
            {
                var actions = 0;
                //Process the non-delayed actions
                lock (this.actions)
                {
                    for (var i = 0; i < this.actions.Count; i++)
                    {
                        toBeRun[actions++] = this.actions[i];
                        if (actions == 999)
                            break;
                    }
                    this.actions.Clear();
                }
                for (var i = 0; i < actions; i++)
                {
                    var a = toBeRun[i];
                    try
                    {
                        a();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Queued Exception: " + e.ToString());
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogError("UnityThreader Error " + e.ToString());
            }
        }
    }
}
