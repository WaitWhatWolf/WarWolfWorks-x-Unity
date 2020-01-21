using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Class used for transitions between events.
    /// </summary>
    public class TransitionManager : MonoBehaviour
    {
        private static TransitionManager Instance;
        
        private static Image TransitionImage;

        private static List<(int index, Sprite sprite, Color color, float speed)> TransitionSettings = new List<(int index, Sprite sprite, Color color, float speed)>();

        private static Thread TransitorSetter = new Thread(new ThreadStart(InitializeTransitionManager));

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeThread()
        {
            GameObject g = new GameObject("TransitionImage");
            Instance = g.AddComponent<TransitionManager>();
            TransitionImage = g.AddComponent<Image>();
            TransitionImage.color = Color.clear;
            TransitionImage.raycastTarget = false;
            if (!Hooks.UtilityCanvas) TransitorSetter.Start();
            else InitializeTransitionManager();
        }

        private static void InitializeTransitionManager()
        {
            while (true)
            {
                if (Hooks.UtilityCanvas)
                {
                    TransitionImage.rectTransform.SetParent(Hooks.UtilityCanvas.transform);
                    TransitionImage.rectTransform.SetAnchoredUI(new Vector4(0, 0, 1, 1));
                    TransitionImage.rectTransform.SetAsLastSibling();
                    break;
                }
            }

            if(TransitorSetter.IsAlive) TransitorSetter.Abort();
        }

        /// <summary>
        /// This event is invoked when ActivateTransition(int ofIndex) is called.
        /// </summary>
        public static event Action<int> OnTransitionStart;
        /// <summary>
        /// This event is invoked once the transition's alpha is at 1.
        /// </summary>
        public static event Action<int> OnTransitionComplete;
        /// <summary>
        /// This event is invoked after OnTransitionComplete when transition's alpha is at 0.
        /// </summary>
        public static event Action<int> OnTransitionEnd;

        /// <summary>
        /// Sets a transition to the specified index.
        /// </summary>
        /// <param name="ofIndex"></param>
        /// <param name="spriteUsed"></param>
        /// <param name="color"></param>
        /// <param name="speed"></param>
        public static void SetTransition(int ofIndex, Sprite spriteUsed, Color color, float speed)
        {
            (int, Sprite, Color, float) toUse = (ofIndex, spriteUsed, color, speed);
            int ExistingIndex = TransitionSettings.FindIndex(t => t.index == ofIndex);
            if (ExistingIndex == -1)
                TransitionSettings.Add(toUse);
            else
            {
                TransitionSettings[ExistingIndex] = toUse;
            }
        }

        /// <summary>
        /// Activates a transition of the specified index.
        /// </summary>
        /// <param name="ofIndex"></param>
        public static void ActivateTransition(int ofIndex)
        {
            (int index, Sprite sprite, Color color, float speed) use = TransitionSettings.Find(t => t.index == ofIndex);

            if(use == default)
            {
                throw new NotImplementedException("Cannot start transition of index {ofIndex} as it's setting were not set! Use TransitionManager.SetTransition()");
            }

            TransitionImage.color = new Color(use.color.r, use.color.g, use.color.b, 0);
            TransitionImage.sprite = use.sprite;
            Instance.StartCoroutine(ITransition(ofIndex, use.speed), ref ITransitionIsRunning);
        }

        /// <summary>
        /// Gets the progress of the currently played transition.
        /// </summary>
        public static float CurrentTransitionProgress { get; private set; }

        private static bool ITransitionIsRunning;
        private static IEnumerator ITransition(int ofIndex, float speed)
        {
            AdvancedDebug.LogFormat("Starting Transition {0}...", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, ofIndex);
            OnTransitionStart?.Invoke(ofIndex);

            while (TransitionImage.color.a < 1)
            {
                TransitionImage.color = new Color(TransitionImage.color.r, TransitionImage.color.g, TransitionImage.color.b, TransitionImage.color.a + (Time.unscaledDeltaTime * speed));
                CurrentTransitionProgress = TransitionImage.color.a / 2;
                yield return null;
            }

            AdvancedDebug.LogFormat("Completing Transition {0}...", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, ofIndex);
            OnTransitionComplete?.Invoke(ofIndex);

            while(TransitionImage.color.a > 0)
            {
                TransitionImage.color = new Color(TransitionImage.color.r, TransitionImage.color.g, TransitionImage.color.b, TransitionImage.color.a - (Time.unscaledDeltaTime * speed));
                CurrentTransitionProgress = 0.5f + (TransitionImage.color.a / 2);
                yield return null;
            }

            AdvancedDebug.LogFormat("Ending Transition {0}...", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, ofIndex);
            OnTransitionEnd?.Invoke(ofIndex);

            CurrentTransitionProgress = 0;
            AdvancedDebug.LogFormat("Ending Transition {0} thread...", AdvancedDebug.DEBUG_LAYER_WWW_INDEX, ofIndex);
            Instance.StopCoroutine(ITransition(ofIndex, speed), ref ITransitionIsRunning);
        }

    }
}
