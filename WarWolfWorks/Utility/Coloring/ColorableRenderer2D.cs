using System;
using UnityEngine;
using UnityEngine.Serialization;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.Utility.Coloring
{
    /// <summary>
    /// To use with <see cref="ColorManager"/> for application of color(s) to sprite renderers.
    /// </summary>
    [RequireComponent(typeof(ColorManager))]
    public sealed class ColorableRenderer2D : MonoBehaviour, IColorable
    {
        /// <summary>
        /// Reference of all <see cref="SpriteRenderer"/> assigned to be changed.
        /// </summary>
        [SerializeField,FormerlySerializedAs("renderers")]
        public SpriteRenderer[] Renderers;

        Color IColorable.ColorApplier { set => Array.ForEach(Renderers, r => r.sharedMaterial.color = value); }

        private void Awake()
        {
            GetComponent<ColorManager>().AddColorable(this);
        }
    }
}