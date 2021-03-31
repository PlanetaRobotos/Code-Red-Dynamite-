using System.Collections;
using Core;
using UnityEngine;

namespace Mehanics.Objects
{
    public class TeleportDetail : MonoBehaviour
    {
        [SerializeField] private Transform teleportPoint;
        [SerializeField] private float telepotDelay = 2f;
        private CircleCollider2D _collider;

        private void Start()
        {
            _collider = GetComponent<CircleCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.CompareTag(Tags.s_Player)) return;
            other.transform.position = teleportPoint.position;
            StartCoroutine(ColliderActivator());
        }

        private IEnumerator ColliderActivator()
        {
            _collider.enabled = false;
            yield return new WaitForSeconds(telepotDelay);
            _collider.enabled = true;
        }
    }
}