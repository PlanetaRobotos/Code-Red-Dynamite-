using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mehanics.Player
{
    public class MainMenuPlayerProperties : MonoBehaviour
    {
        [Header("Time between resets player position")] [SerializeField]
        [Range(10, 150)] private float resetDelay = 20f;

        [SerializeField] private Vector2 spawnPosition;

        [HideInInspector] public Rigidbody2D rb;

        private bool _shouldReset = true;
        private const float MinVelocity = 0.0001f;

        private void Start() =>
            rb = GetComponent<Rigidbody2D>();

        private void Update()
        {
            var velocity = rb.velocity;
            if (_shouldReset)
                StartCoroutine(ResetPLayerPositionIe());
            if (Mathf.Abs(velocity.x) <= MinVelocity ||
                Mathf.Abs(velocity.y) <= MinVelocity)
                rb.AddForce(new Vector2(Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)) * Random.Range(0.4f, 7f), ForceMode2D.Impulse);
        }

        private IEnumerator ResetPLayerPositionIe()
        {
            _shouldReset = false;
            yield return new WaitForSeconds(resetDelay);
            transform.position = spawnPosition;
            _shouldReset = true;
        }
    }
}