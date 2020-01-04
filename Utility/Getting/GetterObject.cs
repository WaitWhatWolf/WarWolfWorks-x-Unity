using UnityEngine;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.Utility.Getting
{
	public class GetterObject : MonoBehaviour, IGetter
	{
        [SerializeField]
        private string Name;
        public string 名前 => Name;

        public MonoBehaviour Drawer => this;

        private void Awake()
        {
            if (Name == null) Name = gameObject.name;
            Getter.AddGetter(this);

            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}