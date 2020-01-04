using System;
using UnityEngine;

namespace WarWolfWorks.Utility.Pooling
{
    public interface IPool
    {
        string 名前 { get; set; }
        void StartPool(string 名, int size);
        void EndPool();
    }
}
