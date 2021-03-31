using System.Collections;
using Core;
using Core.Audio;
using Core.Data;
using Core.UI;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using AudioType = Core.Audio.AudioType;

namespace Mehanics.Player
{
    /// <summary>
    /// Work with All collisions 
    /// </summary>
    public class PlayerCollisions : MonoBehaviour
    {
        private AudioManager _audioManager;
        private AbilitiesUi _abilitiesUi;
        // private PauseManager _pauseManager;
        private PlayerDeath _playerDeath;

        [SerializeField] private ParticleSystem deathParticle;
        [SerializeField] private ParticleSystem groundCollParticle;
        [SerializeField] private ParticleSystem ropeCollParticle;

        // [SerializeField] private float hitColorIntensity = 0.7f;
        // private PostProcessProfile _postProcessingProfile;


        private void Start()
        {
            // if(SceneManager.GetActiveScene().buildIndex != 0)
            //     _pauseManager = GameObject.Find("PauseCanvas").GetComponent<PauseManager>();
            _audioManager = GameObject.FindWithTag(Tags.s_Core).GetComponent<AudioManager>();
            _audioManager.PlayAudio(AudioType.SfxPlayerAppear);

            if (SceneManager.GetActiveScene().buildIndex == 0) return;
            _abilitiesUi = GameObject.Find("InputCanvas").GetComponent<AbilitiesUi>();

            _playerDeath = GetComponent<PlayerDeath>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Tags.s_Finish))
            {
                _audioManager.PlayAudio(AudioType.SfxPlayerFinish);
                DeathParticle();
            }
            else if (other.CompareTag(Tags.s_Gravity))
                _audioManager.PlayAudio(AudioType.SfxPlayerGravity);
            else if (other.CompareTag(Tags.s_Teleport))
                _audioManager.PlayAudio(AudioType.SfxPlayerTeleport);
            else if (other.CompareTag(Tags.s_Star))
            {
                DataController.StarsCount++;
                Destroy(other.gameObject, .01f);
                _audioManager.PlayAudio(AudioType.SfxPlayerStar);
            }
        }

        private void DeathParticle()
        {
            var newParticle = Instantiate(deathParticle, transform.position, Quaternion.identity);
            Destroy(newParticle.gameObject, deathParticle.main.startLifetimeMultiplier);
            gameObject.SetActive(false);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(Tags.s_Ground))
            {
                _audioManager.PlayAudio(AudioType.SfxPlayerGround);

                foreach (var contact in other.contacts)
                {
                    var hitPoint = contact.point;
                    var newParticle = Instantiate(groundCollParticle, new Vector3(hitPoint.x, hitPoint.y, 0),
                        Quaternion.Euler(-90, 0, 0));
                    Destroy(newParticle.gameObject, groundCollParticle.main.startLifetimeMultiplier);
                }
            }
            else if (other.gameObject.CompareTag(Tags.s_Rope))
            {
                _audioManager.PlayAudio(AudioType.SfxPlayerRope);

                foreach (var contact in other.contacts)
                {
                    var hitPoint = contact.point;
                    var newParticle = Instantiate(ropeCollParticle, new Vector3(hitPoint.x, hitPoint.y, 0),
                        Quaternion.Euler(-90, 0, 0));
                    Destroy(newParticle.gameObject, ropeCollParticle.main.startLifetimeMultiplier);
                }
            }
            else if (other.gameObject.CompareTag(Tags.s_Bullet))
            {
                _audioManager.PlayAudio(AudioType.SfxPlayerBullet);

                if(SceneManager.GetActiveScene().buildIndex == 0) return;

                _playerDeath.DecreaseHealth();
                
                
                // DeathParticle();
                
            }
            else if (other.gameObject.CompareTag(Tags.s_Block))
                _audioManager.PlayAudio(AudioType.SfxPlayerBlock);
            else if (other.gameObject.CompareTag(Tags.s_Mill))
                _audioManager.PlayAudio(AudioType.SfxPlayerMill);
        }

        // private IEnumerator RestartWhenNotShowingAdIe()
        // {
        //     yield return new WaitUntil(() => !Advertisement.isShowing);
        //     _pauseManager.OnRestartGame();
        // }
    }
}