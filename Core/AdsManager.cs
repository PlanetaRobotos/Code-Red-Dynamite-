// using System;
// using System.Collections;
// using Core.DataObjects;
// using UnityEngine;
// using UnityEngine.Advertisements;
//
// namespace Core
// {
//     public class AdsManager : MonoBehaviour, IUnityAdsListener
//     {
//         // private const string Placement = "video";
//         private const string Placement = "rewardedVideo";
//         private const string GameId = "3988505";
//
//         [Header("Time between two ads")] [SerializeField]
//         private ValuesRange adDelayRange;
//
//         private void Start()
//         {
//             if (!Advertisement.isSupported) return;
//             Advertisement.AddListener(this);
//             Advertisement.Initialize(GameId, false);
//             // StartCoroutine(ShowAdLoopIe());
//         }
//
//         /// <summary>
//         /// When Click on button for example
//         /// </summary>
//         public static void OnShowAd()
//         {
//             if (!Advertisement.IsReady()) return;
//             Advertisement.Show(Placement);
//         }
//
//
//         /// <summary>
//         /// Old Thing
//         /// </summary>
//         /// <returns></returns>
//         private IEnumerator ShowAdLoopIe()
//         {
//             while (Advertisement.isSupported)
//             {
//                 yield return new WaitForSeconds(adDelayRange.GetRandom());
//                 if (Advertisement.IsReady())
//                     Advertisement.Show(Placement);
//                 yield return new WaitUntil(() => !Advertisement.isShowing);
//             }
//
//             Debug.Log("Advertisement isn't supported");
//             yield return null;
//         }
//
//         public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
//         {
//             switch (showResult)
//             {
//                 case ShowResult.Finished:
//                     Debug.Log("ShowResult.Finished");
//                     break;
//                 case ShowResult.Failed:
//                     Debug.Log("ShowResult.Failed");
//                     break;
//                 case ShowResult.Skipped:
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(showResult), showResult, null);
//             }
//         }
//
//         public void OnUnityAdsReady(string placementId)
//         {
//         }
//
//         public void OnUnityAdsDidError(string message)
//         {
//         }
//
//         public void OnUnityAdsDidStart(string placementId)
//         {
//         }
//     }
// }