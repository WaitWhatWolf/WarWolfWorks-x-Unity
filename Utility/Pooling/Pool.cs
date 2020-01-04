using UnityEngine;

namespace WarWolfWorks.Utility.Pooling
{
    public sealed class Pool : IPool
    {
        public GameObject[] Values { get; private set; }
        public Transform ホルダー { get; private set; }
        private int カウンター;


        public string 名前 { get; set; }

        public void StartPool(string 名前, int プールサイズ)
        {
            Values = new GameObject[プールサイズ];
            ホルダー = new GameObject($"{名前} Pool Holder").transform;
            ホルダー.transform.position = Vector3.zero;
            for(int i = 0; i < Values.Length; i++)
            {
                Values[i] = new GameObject(名前);
                Values[i].transform.SetParent(ホルダー);
                Values[i].transform.position = Vector3.zero;
                Values[i].SetActive(false);
            }
        }

        public void EndPool()
        {
            GameObject.Destroy(ホルダー);
        }

        public GameObject Instantiate(Vector3 position)
        {
            Values[カウンター].transform.position = position;
            Values[カウンター].SetActive(true);
            return Values[カウンター];
        }
    }
}
