using System.Collections;
using UnityEngine;

namespace Core.UI
{
    public class TaskManager : MonoBehaviour
    {
        private static TaskManager _instance;

        private void Awake()
        {
            if (_instance != null) return;
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static void RunCoroutine(IEnumerator routine) =>
            _instance.StartCoroutine(routine);
    }
}