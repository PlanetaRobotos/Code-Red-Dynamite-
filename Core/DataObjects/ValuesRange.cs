using UnityEngine;

namespace Core.DataObjects
{
    [CreateAssetMenu(fileName = "New ValueRange", menuName = "Data/ValueRange", order = 0)]
    public class ValuesRange : ScriptableObject
    {
        [SerializeField] private float minValue;
        [SerializeField] private float maxValue;

        public float MinValue => minValue;
        public float MaxValue => maxValue;
        public float GetRandom() => Random.Range(minValue, maxValue);
    }
}