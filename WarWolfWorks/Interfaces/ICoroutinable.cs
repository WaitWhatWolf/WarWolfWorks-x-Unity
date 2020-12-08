using System.Collections;
using WarWolfWorks.Utility;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Determines a component which uses a pointer to 
    /// <see cref="Hooks.StartCoroutine(UnityEngine.MonoBehaviour, IEnumerator, ref bool)"/> and 
    /// <see cref="Hooks.StopCoroutine(UnityEngine.MonoBehaviour, IEnumerator, ref bool)"/>.
    /// </summary>
    public interface ICoroutinable
    {
        /// <summary>
        /// Should point to <see cref="Hooks.StartCoroutine(UnityEngine.MonoBehaviour, IEnumerator, ref bool)"/>.
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="verifier"></param>
        void StartCoroutine(IEnumerator routine, ref bool verifier);
        /// <summary>
        /// Should point to <see cref="Hooks.StopCoroutine(UnityEngine.MonoBehaviour, IEnumerator, ref bool)"/>.
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="verifier"></param>
        void StopCoroutine(IEnumerator routine, ref bool verifier);

    }
}
