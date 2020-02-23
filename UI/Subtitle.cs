using UnityEngine;
using TMPro;

namespace WarWolfWorks.UI
{
    /// <summary>
    /// Used with <see cref="Subtitles"/> to display text on the screen.
    /// </summary>
    [System.Serializable]
    public sealed class Subtitle
    {
#pragma warning disable 0649
#pragma warning disable IDE0044

        /// <summary>
        /// The <see cref="TextMeshProUGUI"/> object assigned to this <see cref="Subtitle"/>.
        /// </summary>
        [HideInInspector]
        public TextMeshProUGUI Text;

        [SerializeField][TextArea]
        private string content;
        /// <summary>
        /// Currently displayed text.
        /// </summary>
        public string Content => content;

        [SerializeField]
        private float countdown;
        /// <summary>
        /// The assigned countdown.
        /// </summary>
        public float Countdown => countdown;
        /// <summary>
        /// The current countdown of this <see cref="Subtitle"/>.
        /// </summary>
        public float CurrentCountdown { get; private set; }
        [SerializeField]
        private bool loops;
        /// <summary>
        /// If true, <see cref="Subtitles"/> will put <see cref="CurrentCountdown"/> back to <see cref="Countdown"/> instead of removing it.
        /// </summary>
        public bool Loops => loops;

        [SerializeField]
        private Gradient gradient;
        /// <summary>
        /// The gradient assigned to this <see cref="Subtitle"/>. <see cref="Text"/>'s color is assigned with <see cref="Gradient.Evaluate(float)"/>.
        /// </summary>
        public Gradient Gradient => gradient;

        [SerializeField]
        private Vector4 anchorPosition;
        /// <summary>
        /// The anchored position <see cref="Text"/> is set to.
        /// </summary>
        public Vector4 AnchorPosition => anchorPosition;

        /// <summary>
        /// Returns <see cref="CurrentCountdown"/> / <see cref="Countdown"/>.
        /// </summary>
        public float Progress01 => CurrentCountdown / Countdown;

        internal void Init()
        {
            CurrentCountdown = Countdown;
        }

        internal void Update()
        {
            CurrentCountdown -= Time.deltaTime;
            if (Loops && CurrentCountdown <= 0)
                CurrentCountdown = Countdown;
        }

        /// <summary>
        /// Resets the <see cref="Subtitle"/>'s countdown and assigns it with new content and a new gradient.
        /// </summary>
        /// <param name="newContent"></param>
        /// <param name="newGradient"></param>
        public void Reset(string newContent, Gradient newGradient)
        {
            content = newContent;
            CurrentCountdown = Countdown;
            gradient = newGradient;
        }

        /// <summary>
        /// Creates a new <see cref="Subtitle"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="anchors"></param>
        /// <param name="countdown"></param>
        /// <param name="gradient"></param>
        /// <param name="loops"></param>
        public Subtitle(string content, Vector4 anchors, float countdown, Gradient gradient, bool loops)
        {
            this.content = content;
            anchorPosition = anchors;
            this.countdown = countdown;
            this.gradient = gradient;
            this.loops = loops;
        }

        /// <summary>
        /// Creates a copy of another <see cref="Subtitle"/>; Copies everything except <see cref="Text"/>.
        /// </summary>
        /// <param name="copy"></param>
        public Subtitle(Subtitle copy)
        {
            content = copy.content;
            Text = null;
            anchorPosition = copy.anchorPosition;
            countdown = copy.countdown;
            CurrentCountdown = copy.Countdown;
            gradient = copy.gradient;
            loops = copy.loops;
        }
    }
}