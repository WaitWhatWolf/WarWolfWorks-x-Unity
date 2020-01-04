using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WarWolfWorks.EntitiesSystem;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility.Coloring;

#if !WWWCLM_2
namespace WWW.Utility
{
#else
namespace WarWolfWorks.Deprecated
{ 
    [Obsolete("A new color manager exists, use WWWCLM_2 compiler symbol.")]
#endif
    public class EntityColorManager2D :
#if !WWWCLM_2
        ColorManager, IEntityComponent
#else
        MonoBehaviour
#endif
    {
#if !WWWCLM_2
        private Color SpriteMaterial
        {
            set
            {
                UsedRenderers.ForEach
                (   
                    r =>
                    {
                        if (r is SpriteRenderer)
                            ((SpriteRenderer)r).color = value;
                    }
                );
                UsedGraphics.ForEach(mg => mg.color = value);
            }
        }

        bool parentEntityVerified;
        private Entity parentEntity;
        public Entity EntityMain
        {
            get
            {
                if(!parentEntityVerified)
                {
                    parentEntity = GetComponent<Entity>();
                    parentEntityVerified = true;
                }

                return parentEntity;
            }
        }

        /// <summary>
        /// Returns the first renderer instance of type SpriteRenderer. If none are found, it returns null instead.
        /// </summary>
        public SpriteRenderer SpriteRenderer => (SpriteRenderer)UsedRenderers.Find(r => r is SpriteRenderer);

        public void OnAwake() { }
        public void OnEabled() { }
        public void OnDisabled() { }
        public void OnStart() { }
        public void OnUpdate() { }
        public void OnFixed()
        {
            CountdownColors();
            UpdateColors();
        }
        public void OnDestroyed() { }

        private void UpdateColors() => SpriteMaterial = GetFinalColor();
        public void StartCoroutine(IEnumerator routine, ref bool verifier) => Utilities.StartCoroutine(EntityMain, routine, ref verifier);
        public void StopCoroutine(IEnumerator routine, ref bool verifier) => Utilities.StopCoroutine(EntityMain, routine, ref verifier);
#endif
        }
}
