using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    public interface IGetter<T> where T : MonoBehaviour
    {
        T Drawer { get; }
    }
}
