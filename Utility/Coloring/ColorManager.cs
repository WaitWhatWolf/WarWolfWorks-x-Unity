using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.Utility.Coloring
{
    /// <summary>
    /// Class used to manage colors. It works on a systen similar to layers, where there is 8 lists of <see cref="CMColor"/>s,
    /// where 8 is always displayed as it is of the lowest depth. 
    /// </summary>
    public sealed class ColorManager : MonoBehaviour
    {
        private const int MaxLayers = 8;

        [FormerlySerializedAs("baseColor"), SerializeField]
        private Color s_BaseColor;
        /// <summary>
        /// The base color to apply.
        /// </summary>
        public Color BaseColor
        {
            get => s_BaseColor;
            set => s_BaseColor = value;
        }

        private Dictionary<int, List<CMColor>> Colors = new Dictionary<int, List<CMColor>>(MaxLayers);
        private List<IColorable> Colorables = new List<IColorable>();
        /// <summary>s
        /// Returns all <see cref="IColorable"/> elements affected by this <see cref="ColorManager"/>.
        /// </summary>
        public IColorable[] GetColorables() => Colorables.ToArray();

        /// <summary>
        /// Adds a <see cref="IColorable"/> to colorables affected by this <see cref="ColorManager"/>.
        /// </summary>
        /// <param name="colorable"></param>
        public void AddColorable(IColorable colorable) => Colorables.Add(colorable);

        /// <summary>
        /// Removes specified <see cref="IColorable"/> from colorables affected by this <see cref="ColorManager"/>.
        /// </summary>
        /// <param name="colorable"></param>
        /// <returns></returns>
        public bool RemoveColorable(IColorable colorable) => Colorables.Remove(colorable);

        /// <summary>
        /// The color that will be applied to all Colorables.
        /// </summary>
        public Color FinalColor { get; private set; }

        private void Awake()
        {
            for(int i = 0; i < MaxLayers; i++)
            {
                Colors.Add(i, new List<CMColor>());
            }
        }

        private void Update()
        {
            try
            {
                Color finalDestination = BaseColor;
                bool evaluedColor = false;
                for (int i = Colors.Count - 1; i >= 0; i--)
                {
                    if (!evaluedColor && Colors[i].Count > 0)
                    {
                        finalDestination = ApplyUpdateColor(i == 0 ? BaseColor : Color.clear, i);//ApplyUpdateColor(i == 0 ? BaseColor : Color.clear, Colors[i], i);
                        evaluedColor = true;
                    }
                    for (int j = 0; j < Colors[i].Count; j++)
                    {
                        ColorBehavior beh = (ColorBehavior)Colors[i][j];
                        if (beh == ColorBehavior.NoDurationCountdown)
                            continue;

                        Colors[i][j].CurrentDuration -= Time.deltaTime;//Mathf.Clamp(Colors[i][j].CurrentDuration - Time.deltaTime, 0, Colors[i][j].CurrentDuration);
                        if (Colors[i][j].CurrentDuration <= 0 && beh == ColorBehavior.RemoveOnDurationEnd)
                        {
                            Colors[i].Remove(Colors[i][j]);
                        }
                    }
                }

                FinalColor = finalDestination;

                foreach (IColorable ic in Colorables)
                {
                    ic.ColorApplier = FinalColor;
                }
            }
            catch(Exception e)
            {
                AdvancedDebug.LogWarning($"Couldn't Apply color, {e}", AdvancedDebug.DEBUG_LAYER_EXCEPTIONS_INDEX);
                return;
            }
        }

        private Color GetColorByApplication(CMColor color, int layer)
        {
            switch((ColorApplication)color)
            {
                default: return Color.clear;
                case ColorApplication.FlatAdd:
                    return color;
                case ColorApplication.FlatRemove:
                    return Hooks.Colors.ToNegative(color);
                case ColorApplication.AverageAdd:
                    return (Color)color / Colors[layer].Count;
                case ColorApplication.AverageRemove:
                    return Hooks.Colors.ToNegative(color) / Colors[layer].Count;
                case ColorApplication.AscendingAdd:
                    return (Color)color * (1 - (color.CurrentDuration / color.MaxDuration));
                case ColorApplication.AscendingRemove:
                    return (Color)color * (color.CurrentDuration / color.MaxDuration);
                case ColorApplication.AscendingAverageAdd:
                    return ((Color)color / Colors[layer].Count) * (1 - (color.CurrentDuration / color.MaxDuration));
                case ColorApplication.AscendingAverageRemove:
                    return (Hooks.Colors.ToNegative(color) / Colors[layer].Count) * (color.CurrentDuration / color.MaxDuration);
            }
        }

        private Color ApplyUpdateColor(Color original, int layer)
        {
            Color toReturn = original;
            for(int i = 0; i < Colors[layer].Count; i++)
            {
                toReturn += GetColorByApplication(Colors[layer][i], layer);
            }

            return toReturn;
        }

        [ContextMenu("Add 2D Support (ColorableRenderer2D)")]
        private void AddColorableRenderer2D()
        {
            ColorableRenderer2D cr2d = gameObject.AddComponent<ColorableRenderer2D>();
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr)
            {
                cr2d.Renderers = new SpriteRenderer[] { sr };
            }
        }

        private int LayerMethodFixer(int original)
        {
            if (original > MaxLayers)
            {
                AdvancedDebug.LogWarningFormat("Layer {0} was set too high. Changing to layer {1}.", 0, original, MaxLayers);
                original = MaxLayers;
            }
            else if (original < 0)
            {
                AdvancedDebug.LogWarningFormat("Layer {0} was set too low. Changing to layer {1}.", 0, original, 0);
                original = 0;
            }

            return original;
        }

        /// <summary>
        /// Restarts the duration from the beginning for the given color.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="layer"></param>
        public bool RefreshColorDuration(CMColor color, int layer)
        {
            layer = LayerMethodFixer(layer);
            int index = Colors[layer].IndexOf(color);
            if (index == -1)
                return false;

            Colors[layer][index].CurrentDuration = color.MaxDuration;
            return true;
        }

        /// <summary>
        /// Add a color to be processed by the <see cref="ColorManager"/>.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="to"></param>
        public void AddColor(CMColor color, int to)
        {
            to = LayerMethodFixer(to);
            //color.CurrentDuration = color.MaxDuration;
            Colors[to].Add(color);
        }

        /// <summary>
        /// Removes a previously added <see cref="CMColor"/> and returns true if the color was successfully removed.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="from"></param>
        public bool RemoveColor(CMColor color, int from)
        {
            from = LayerMethodFixer(from);
            int index = Colors[from].IndexOf(color);
            if (index == -1)
                return false;
            else
            {
                Colors[from].RemoveAt(index);
                return true;
            }
        }

        /// <summary>
        /// Removes the first instance of the given color in any layer.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool RemoveColor(CMColor color)
        {
            for (int i = 0; i < Colors.Count; i++)
            {
                if (RemoveColor(color, i))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all instances of the given object from the specified layer.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public int RemoveAll(CMColor color, int from)
        {
            int toReturn = 0;
            while(RemoveColor(color, from))
            {
                toReturn++;
            }

            return toReturn;
        }

        /// <summary>
        /// Removes all instances of the given object from all layers.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public int RemoveAll(CMColor color)
        {
            int count = 0;
            for (int i = 0; i < Colors.Count; i++)
            {
                count += RemoveAll(color, i);
            }

            return count;
        }
    }
}