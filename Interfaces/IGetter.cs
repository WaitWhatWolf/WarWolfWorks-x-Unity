using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    public interface IGetter
    {
        string 名前 { get; }
        MonoBehaviour Drawer { get; }
    }
}