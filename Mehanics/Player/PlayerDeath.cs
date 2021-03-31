using System;
using System.Collections;
using Core;
using Core.Data;
using Core.UI;
using LionStudios;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

namespace Mehanics.Player
{
    public enum DeathState
    {
        Alive,
        Dead,
        AlmostDead
    }

    public class PlayerDeath : MonoBehaviour
    {
        [Range(1, 200)] [SerializeField] private float lifeTime = 100f;
        [Range(0.5f, 10f)] [SerializeField] private float bulletDecrease = 1f;
        [SerializeField] private GameObject screenCrashPrefab;
        [SerializeField] private ParticleSystem death;

        [SerializeField] private GameObject waterPrefab;
        [SerializeField] private float waterFillingSpeed;
        // private GameObject _waterTheme;

        private PauseManager _pauseManager;
        private const int AdTurn = 5;
        private DeathState _lifeState;
        private GameObject _water;
        private Camera _camera;

        public float LifeTime => lifeTime;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Awake()
        {
            _lifeState = DeathState.Alive;
            DataController.Health = lifeTime;
            // _waterTheme = GameObject.Find("WaterTheme");

            if (SceneManager.GetActiveScene().buildIndex == 0) return;
            _pauseManager = GameObject.Find("PauseCanvas").GetComponent<PauseManager>();
        }

        private void Update()
        {
            switch (_lifeState)
            {
                case DeathState.Alive:
                {
                    DataController.Health -= Time.deltaTime;

                    if (DataController.Health <= 0) _lifeState = DeathState.AlmostDead;
                    break;
                }
                case DeathState.AlmostDead:
                {
                    _lifeState = DeathState.Dead;

                    DataController.DeathCounter++;
                    Analytics.Events.LevelFailed(SceneManager.GetActiveScene().buildIndex, DataController.StarsCount);                    

                    // ToDo Advertisement bugs
                    // if (DataController.DeathCounter % AdTurn == 0)
                    //     AdsManager.OnShowAd();

                    StartCoroutine(RestartIe());
                    break;
                }
                case DeathState.Dead:
                    _water.transform.position += Vector3.up / 100 * waterFillingSpeed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void DecreaseHealth() =>
            DataController.Health = Mathf.Clamp(DataController.Health - bulletDecrease, 0, lifeTime);

        private IEnumerator RestartIe()
        {
            // ToDo Advertisement bugs
            // if (Advertisement.isShowing)
                // yield return new WaitUntil(() => !Advertisement.isShowing);

            // GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

            var playerPosition = transform.position;
            var position = playerPosition;

            Instantiate(death, playerPosition, Quaternion.identity);

            Instantiate(screenCrashPrefab, position, Quaternion.identity);
            var cameraPosition = _camera.transform.position;
            _water = Instantiate(waterPrefab, new Vector3(cameraPosition.x, cameraPosition.y - 46f, 0),
                Quaternion.Euler(new Vector3(0, 0, 180)));
            
            // Destroy(gameObject, .1f);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            
            // Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(3f);

            _pauseManager.OnRestartGame();
        }
    }
}