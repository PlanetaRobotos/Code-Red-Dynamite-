using System.Collections;
using UnityEngine;

namespace Core
{
    public static class CoroutineUtilities
    {
        /// <summary>
        /// Work even timeScale = 0
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static IEnumerator WaitForRealTime(float delay)
        {
            while (true)
            {
                var pauseEndTime = Time.realtimeSinceStartup + delay;
                while (Time.realtimeSinceStartup < pauseEndTime) yield return null;
                break;
            }
        }
    }
}