using UnityEngine;

namespace Game_Stuff
{
    public class TextLocalizationDetail: MonoBehaviour
    {
        [SerializeField] private string key;

        public string Key => key;
    }
}