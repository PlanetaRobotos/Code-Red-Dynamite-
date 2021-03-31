using System;
using System.Linq;
using UnityEngine;

namespace Core.Utilities
{
    public static class Tools
    {
        /// <summary>
        /// For each null-value serialized field log error in Unity Console
        /// </summary>
        /// <param name="assembly">MonoBehaviour or ScriptableObject class instance</param>
        /// <example>
        /// public class MyClass : MonoBehaviour {
        /// #if UNITY_EDITOR
        ///     private void OnValidate() {
        ///         Tools.ValidateFieldsForNull(this);
        ///     }
        /// #endif
        /// }
        /// </example>
        public static void ValidateFieldsForNull<T>(T assembly)
        {
            string className = assembly.ToString();
            foreach (var fieldInfo in Reflection.GetAllNullValueFields(assembly))
                Debug.LogWarning($"\"{className}\": FIELD \"{fieldInfo.Name}\" IS NOT DEFINED!");
        }

        public static float RoundValue(float value, int tolerance) => (float) Math.Round(value, tolerance);


        public static Color CombineColors(params Color[] colors)
        {
            float r = colors.Sum(clr => clr.r);
            float g = colors.Sum(clr => clr.g);
            float b = colors.Sum(clr => clr.b);
            float a = colors.Sum(clr => clr.a);
            float len = colors.Length;
            return new Color(r / len, g / len, b / len, a / len);
        }


        public static void SmartRoundToInt(this ref Vector2 vec, float threshold = 0) =>
            vec.Set(vec.x > threshold ? 1 : vec.x < -threshold ? -1 : 0,
                vec.y > threshold ? 1 : vec.y < -threshold ? -1 : 0);
    }
}