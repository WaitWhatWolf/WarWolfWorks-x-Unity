using System.Collections;
using TMPro;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Utility;
using static WarWolfWorks.UI.GUIViewport.GUI;

namespace WarWolfWorks.UI
{
    /// <summary>
    /// Used with <see cref="GUIViewport"/> to display a text in world position.
    /// </summary>
    [System.Serializable]
    public sealed class WorldText : GUIViewport.GUI
    {
#pragma warning disable 0649
#pragma warning disable IDE0044

        /// <summary>
        /// The <see cref="TextMeshProUGUI"/> object assigned to this <see cref="Subtitle"/>.
        /// </summary>
        public TextMeshProUGUI TextGraphic => CoreGraphic as TextMeshProUGUI;

        [SerializeField, NoS]
        private Vector2 s_AnchoredSize;
        [SerializeField, NoS]
        private Vector3 s_Offset;
        [SerializeField, NoS]
        private Transform s_Followed;
        [SerializeField, NoS]
        private string s_Content;
        [SerializeField, NoS]
        private Gradient s_TextColor;
        [SerializeField, NoS]
        private float s_Countdown;
        [SerializeField, NoS]
        private bool s_Loops;

        /// <summary>
        /// The text to be displayed on screen.
        /// </summary>
        public string Content => s_Content;

        /// <summary>
        /// The anchored size of this <see cref="Subtitle"/>.
        /// </summary>
        public override Vector2 AnchoredSize => s_AnchoredSize;

        /// <summary>
        /// The position at which the text displays.
        /// </summary>
        public override Vector3 Position { get => (s_Followed ? s_Followed.position : Vector3.zero) + s_Offset; protected set => s_Followed.position = value; }

        /// <summary>
        /// The color of the text. (Uses <see cref="Gradient.Evaluate(float)"/> to set the color of the text)
        /// </summary>
        public Gradient TextColor { get => s_TextColor; set => s_TextColor = value; }

        private float CurrentCountdown = 0;
        /// <summary>
        /// Returns the current countdown.
        /// </summary>
        /// <returns></returns>
        public float GetCurrentCountdown() => CurrentCountdown;

        /// <summary>
        /// The countdown that the text counts down from.
        /// </summary>
        public float Countdown { get => s_Countdown; private set => s_Countdown = value; }

        /// <summary>
        /// If true, the <see cref="Subtitle"/> will not get removed when countdown reaches 0; Instead, it will reset the countdown.
        /// </summary>
        public bool Loops { get; private set; }

        /// <summary>
        /// <see cref="WorldText"/> uses <see cref="GUIType.WorldPosToViewport"/>.
        /// </summary>
        public override GUIType Type => GUIType.WorldPosToViewport;

        /// <summary>
        /// Resets the <see cref="Subtitle"/>'s countdown and assigns it with new content and a new gradient.
        /// </summary>
        /// <param name="newContent"></param>
        /// <param name="newGradient"></param>
        public void Reset(string newContent, Gradient newGradient)
        {
            s_Content = newContent;
            CurrentCountdown = Countdown;
            s_TextColor = newGradient;
        }

        /// <summary>
        /// Sets the cooldown and starts a coroutine to calculate the countdown.
        /// </summary>
        protected override void OnInit()
        {
            GameObject g = new GameObject("Subtitle_" + ID);
            TextMeshProUGUI ugui = g.AddComponent<TextMeshProUGUI>();
            ugui.text = s_Content;
            ugui.rectTransform.localScale = Vector3.one;
            ugui.fontSizeMin = 1;
            ugui.fontSizeMax = 1000;
            ugui.enableAutoSizing = true;
            ugui.raycastTarget = false;

            CoreGraphic = ugui;

            CurrentCountdown = Countdown;
            Parent.StartCoroutine(IUpdate(), ref IUpdateIsRunning);
        }

        private bool IUpdateIsRunning;
        private IEnumerator IUpdate()
        {
            while (IUpdateIsRunning)
            {
                CurrentCountdown -= Time.deltaTime;
                if (CurrentCountdown <= 0)
                {
                    if (s_Loops)
                        CurrentCountdown = Countdown;
                    else Parent.Remove(this);
                }

                TextGraphic.color = s_TextColor.Evaluate(1 - (CurrentCountdown / s_Countdown));

                yield return null;
            }
        }

        /// <summary>
        /// Destroys the <see cref="TextMeshProUGUI"/> used.
        /// </summary>
        public override void Dispose()
        {
            Object.Destroy(TextGraphic.gameObject);
            Parent.StopCoroutine(IUpdate(), ref IUpdateIsRunning);

            Deinit();
        }

        /// <summary>
        /// Sets the anchored position of the text.
        /// </summary>
        /// <param name="to"></param>
        public void SetFollowed(Transform to)
        {
            s_Followed = to;
        }

        /// <summary>
        /// Sets the anchored size of the text.
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Vector2 size)
        {
            s_AnchoredSize = size;
        }

        /// <summary>
        /// Creates a new <see cref="Subtitle"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="followed"></param>
        /// <param name="offset"></param>
        /// <param name="anchoredSize"></param>
        /// <param name="countdown"></param>
        /// <param name="textColor"></param>
        /// <param name="loops"></param>
        public WorldText(string content, Transform followed, Vector3 offset, Vector2 anchoredSize, float countdown, Gradient textColor, bool loops)
        {
            s_Content = content;
            s_Followed = followed;
            s_Offset = offset;
            s_AnchoredSize = anchoredSize;
            s_Countdown = countdown;
            s_TextColor = textColor;
            s_Loops = loops;
        }

        /// <summary>
        /// Creates a copy of another <see cref="Subtitle"/>; Copies everything except <see cref="TextGraphic"/>.
        /// </summary>
        /// <param name="copy"></param>
        public WorldText(WorldText copy)
        {
            s_Content = copy.s_Content;
            CoreGraphic = null;
            s_Followed = copy.s_Followed;
            s_Offset = copy.s_Offset;
            s_AnchoredSize = copy.s_AnchoredSize;
            s_Countdown = copy.s_Countdown;
            CurrentCountdown = copy.Countdown;
            s_TextColor = copy.s_TextColor;
            s_Loops = copy.s_Loops;
        }
    }
}
