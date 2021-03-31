using System.Collections;
using Core;
using UnityEngine;

namespace Mehanics.Objects
{
    public class Gun : MonoBehaviour
    {
        private Transform _target;

        [Header("The end of the gun")] [SerializeField]
        private Transform gunEnd;

        [SerializeField] private Rigidbody2D bulletPrefab;
        [SerializeField] private float shootDelay = 1f;
        [SerializeField] private float lifeTime = 2f;
        [SerializeField] private float thrust = 7f;
        [SerializeField] private float gunRange = 10f;

        // [SerializeField] private float smoothSpeed = 1f;
        private const int Offset = 90;
        private bool _canShot = true;

        private void Start() => 
            _target = GameObject.FindWithTag(Tags.s_Player).transform;

        private void LateUpdate()
        {
            var targetPos = _target.position;
            var thisPos = transform.position;
            targetPos.x -= thisPos.x;
            targetPos.y -= thisPos.y;
            var angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + Offset));

            if (!_canShot || Vector3.Distance(_target.position, transform.position) >= gunRange) return;
            var newBullet = Instantiate(bulletPrefab, gunEnd.transform.position, Quaternion.identity);
            newBullet.AddForce((_target.position - transform.position).normalized * thrust, ForceMode2D.Impulse);
            Destroy(newBullet.gameObject, lifeTime);
            StartCoroutine(CanShotIe());
        }

        private IEnumerator CanShotIe()
        {
            _canShot = false;
            yield return new WaitForSeconds(shootDelay);
            _canShot = true;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = GizmosData.EnemyGizmosColor;
            Gizmos.DrawWireSphere(transform.position, gunRange);
        }
#endif
    }
}