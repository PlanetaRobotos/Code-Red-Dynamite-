using System;
using UnityEngine;

namespace Game_Stuff
{
    public abstract class SingletonManager<TSys> : ScriptableObject where TSys : SingletonManager<TSys>
    {
        private static TSys _instance;

        public void Init()
        {
            if (!(_instance is null)) return;
            _instance = (TSys) this;
            OnInit();
        }

        protected abstract void OnInit();
    }
}