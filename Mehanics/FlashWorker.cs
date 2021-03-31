using System;
using System.Collections;
using Core.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mehanics
{
    /// <summary>
    /// Bluer effect
    /// </summary>
    public class FlashWorker : MonoBehaviour
    {
        private GameObject _flash;
        private Animator _flashAnim;
        [SerializeField] private GameObject flashCanvasPrefab;
        [SerializeField] private float checkDelay = 6f;
        
        private static readonly int PlayerFinish = Animator.StringToHash("Finish");

        private IEnumerator Start()
        {
            _flash = Instantiate(flashCanvasPrefab, transform.position, Quaternion.identity);
            _flashAnim = _flash.GetComponentInChildren<Animator>();

            yield return new WaitForSeconds(checkDelay + 1f);

            _flashAnim.SetTrigger(PlayerFinish);
            yield return new WaitForSeconds(.7f);

            var nextLevel = DataController.LevelToLoad;
            // Debug.Log(nextLevel);
            var asyncOperation = SceneManager.LoadSceneAsync(nextLevel > DataController.MaxUnitLevel
                ? 0 : nextLevel);
            while (!asyncOperation.isDone) yield return null;
        }
    }
}