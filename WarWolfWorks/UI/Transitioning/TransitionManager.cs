using System;
using System.Collections;
using static WarWolfWorks.Constants;
using UnityEngine;
using WarWolfWorks.Interfaces.UnityMethods;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;
using WarWolfWorks.UI.MenusSystem;

namespace WarWolfWorks.UI.Transitioning
{
    /// <summary>
    /// Class used for transitions between events.
    /// </summary>
    [AddComponentMenu(IN_ASSETMENU_WARWOLFWORKS + IN_ASSETMENU_UI + nameof(TransitionManager))]
    public sealed class TransitionManager : Menu
    {
        /// <summary>
        /// The currently active transition.
        /// </summary>
        public Transition CurrentlyActive { get; private set; }

        private Transition[] ns_Transitions = new Transition[32];

        /// <summary>
        /// This event is invoked when ActivateTransition(int ofIndex) is called.
        /// </summary>
        public event Action OnTransitionStart;
        /// <summary>
        /// This event is invoked once the transition's alpha is at 1.
        /// </summary>
        public event Action OnTransitionComplete;
        /// <summary>
        /// This event is invoked after OnTransitionComplete when transition's alpha is at 0.
        /// </summary>
        public event Action OnTransitionEnd;

        private void C_INDEX_CHECK(int index)
        {
            if (index < 0 || index >= ns_Transitions.Length)
                throw new Exception(string.Format("Cannot handle the transition under index {0} as the index given is less than 0 or higher than {1}", index, V_TRANSITIONMANAGER_TRANSITIONS_SIZE - 1));
        }

        /// <summary>
        /// Returns the transition under the specified index.s
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Transition GetTransition(int index)
        {
            C_INDEX_CHECK(index);

            return ns_Transitions[index];
        }

        /// <summary>
        /// Sets a transition to the specified index. (Note: This does not start the transition)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="transition"></param>
        /// <param name="speed"></param>
        public void SetTransition<T>(int index, T transition, float speed) where T : Transition
        {
            C_INDEX_CHECK(index);

            (transition as IParentInitiatable<TransitionManager>).Init(this);

            ns_Transitions[index] = transition;
            ns_Transitions[index].Index = index;
            ns_Transitions[index].Speed = speed;
            ns_Transitions[index].TransitionProgress = 0;

            if (transition is IAwake awake)
                awake.Awake();
        }

        /// <summary>
        /// Sets the speed of a transition.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="to"></param>
        public void SetTransitionSpeed(int index, float to)
        {
            C_INDEX_CHECK(index);

            if(ns_Transitions[index] != null)
                ns_Transitions[index].Speed = to;
        }

        /// <summary>
        /// Activates a transition of the specified index.
        /// </summary>
        /// <param name="ofIndex"></param>
        public void RunTransition(int ofIndex)
        {
            if (CurrentlyActive != null)
                return;

            C_INDEX_CHECK(ofIndex);

            CurrentlyActive = ns_Transitions[ofIndex];
            this.StartCoroutine(IE_Transition(), ref ITransitionIsRunning);
        }

        private void Update()
        {
            if (ITransitionIsRunning && CurrentlyActive is IUpdate update)
                update.Update();
        }

        private void FixedUpdate()
        {
            if (ITransitionIsRunning && CurrentlyActive is IFixedUpdate fixedUpdate)
                fixedUpdate.FixedUpdate();
        }

        private void LateUpdate()
        {
            if (ITransitionIsRunning && CurrentlyActive is ILateUpdate lateUpdate)
                lateUpdate.LateUpdate();
        }

        private static bool ITransitionIsRunning;
        private IEnumerator IE_Transition()
        {
            int indexUsed = CurrentlyActive.Index;
            AdvancedDebug.LogFormat("Starting Transition {0}...", DEBUG_LAYER_WWW_INDEX, indexUsed);
            OnTransitionStart?.Invoke();

            while (CurrentlyActive.TransitionProgress < 1)
            {
                CurrentlyActive.TransitionProgress += Time.unscaledDeltaTime * CurrentlyActive.Speed;
                yield return null;
            }

            AdvancedDebug.LogFormat("Completing Transition {0}...", DEBUG_LAYER_WWW_INDEX, indexUsed);
            OnTransitionComplete?.Invoke();
            CurrentlyActive.IsDetransitioning = true;

            while (CurrentlyActive.TransitionProgress > 0)
            {
                CurrentlyActive.TransitionProgress -= Time.unscaledDeltaTime * CurrentlyActive.Speed;
                yield return null;
            }

            AdvancedDebug.LogFormat("Ending Transition {0}...", DEBUG_LAYER_WWW_INDEX, indexUsed);
            OnTransitionEnd?.Invoke();

            if (CurrentlyActive is IOnDestroy onDestroy)
                onDestroy.OnDestroy();

            CurrentlyActive = null;

            AdvancedDebug.LogFormat("Ending Transition {0} coroutine...", DEBUG_LAYER_WWW_INDEX, indexUsed);
            this.StopCoroutine(IE_Transition(), ref ITransitionIsRunning);
        }

    }
}
