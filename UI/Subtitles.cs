using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WarWolfWorks.UI.MenusSystem;
using WarWolfWorks.Utility;

namespace WarWolfWorks.UI
{
    /// <summary>
    /// Class used to display text on a canvas; 
    /// Uses <see cref="Subtitle"/> which has a countdown that displays a certain color from a gradient based on the countdown.
    /// </summary>
	public class Subtitles : Menu
    {
        /// <summary>
        /// All currently displayed subtitles.
        /// </summary>
        public List<Subtitle> AllSubtitles { get; } = new List<Subtitle>();

        /// <summary>
        /// Pushes a <see cref="Subtitle"/> on display.
        /// </summary>
        /// <param name="subtitle"></param>
        public void New(Subtitle subtitle)
        {
            GameObject g = new GameObject("Subtitle_" + (AllSubtitles.Count + 1));
            TextMeshProUGUI ugui = g.AddComponent<TextMeshProUGUI>();
            ugui.text = subtitle.Content;
            ugui.rectTransform.SetParent(Holder);
            ugui.rectTransform.localScale = Vector3.one;
            ugui.rectTransform.SetAnchoredUI(subtitle.AnchorPosition);
            ugui.fontSizeMin = 1;
            ugui.fontSizeMax = 1000;
            ugui.enableAutoSizing = true;

            subtitle.Text = ugui;
            subtitle.Init();
            AllSubtitles.Add(subtitle);
        }

        private void Update()
        {
            for(int i = 0; i < AllSubtitles.Count; i++)
            {
                if (!AllSubtitles[i].Loops && AllSubtitles[i].Progress01 < 0)
                {
                    Destroy(AllSubtitles[i].Text.gameObject);
                    AllSubtitles.RemoveAt(i);
                    continue;
                }

                AllSubtitles[i].Text.color = AllSubtitles[i].Gradient.Evaluate(1 - AllSubtitles[i].Progress01);
                AllSubtitles[i].Text.text = AllSubtitles[i].Content;
                AllSubtitles[i].Update();
            }
        }
    }
}