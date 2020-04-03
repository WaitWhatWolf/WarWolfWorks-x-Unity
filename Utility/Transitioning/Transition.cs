using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.UnityMethods;

namespace WarWolfWorks.Utility.Transitioning
{
    /// <summary>
    /// Base class for implementing a transition inside the <see cref="TransitionManager"/>.
    /// (Supported interfaces: <see cref="IAwake"/>, <see cref="IStart"/>, <see cref="IUpdate"/>, <see cref="IFixedUpdate"/>, <see cref="ILateUpdate"/>,
    /// <see cref="IOnDestroy"/>)
    /// (Note: <see cref="IAwake"/> is called when this <see cref="Transition"/> is added to a <see cref="TransitionManager"/>,
    /// while <see cref="IStart"/> is called when this <see cref="Transition"/> is started.)
    /// </summary>
    public abstract class Transition : IParentInitiatable<TransitionManager>
    {
        /// <summary>
        /// The initiated state of this <see cref="Transition"/>. (<see cref="IParentInitiatable{T}"/> implementation)
        /// </summary>
        public bool Initiated { get; private set; }

        /// <summary>
        /// The <see cref="TransitionManager"/> parent.
        /// </summary>
        public TransitionManager Parent { get; private set; }

        /// <summary>
        /// The progress of the transition; 0 is start, 1 is end.
        /// </summary>
        public float TransitionProgress { get; internal set; }

        /// <summary>
        /// Returns true if the <see cref="TransitionProgress"/> has hit 1, and goes back to 0 to simulate a fade-out.
        /// </summary>
        public bool IsDetransitioning { get; internal set; }

        /// <summary>
        /// The speed at which this transition is going. (Set through <see cref="TransitionManager.SetTransition{T}(int, T, float)"/>).
        /// </summary>
        public float Speed { get; internal set; }

        /// <summary>
        /// The index in which this <see cref="Transition"/> resides in it's parent.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// Initiates this transition.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        bool IParentInitiatable<TransitionManager>.Init(TransitionManager parent)
        {
            if (Initiated)
                return false;

            Parent = parent;
            return true;
        }
    }
}
