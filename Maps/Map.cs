using UnityEngine;

namespace WarWolfWorks.Maps
{
    [CreateAssetMenu(fileName = "new Map", menuName = "Map", order = 0)]
    public class Map : ScriptableObject
    {
        public string MapName;

        public MapComponent[] MapComponents;

        public override bool Equals(object other)
        {
            return (other as Map).MapName == MapName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
