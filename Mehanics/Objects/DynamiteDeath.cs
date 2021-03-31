using System;
using System.Collections;
using UnityEngine;

namespace Mehanics.Objects
{
    public class DynamiteDeath : MonoBehaviour
    {
        [SerializeField] private ParticleSystem deathParticle;
        [SerializeField] private float deathDelay;

        [HideInInspector] public Rigidbody2D rb;
        [SerializeField] private float force;
        [SerializeField] private float xOffset;
        [SerializeField] private float rotationSpeed = 10f;

        private IEnumerator Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.AddForce(new Vector2(xOffset, force), ForceMode2D.Impulse);

            yield return new WaitForSeconds(deathDelay);

            Instantiate(deathParticle, transform.position, Quaternion.identity);
            Destroy(gameObject, .01f);
        }

        private void FixedUpdate()
        {
            // Debug.Log(transform.rotation.z);
            transform.Rotate(new Vector3(0, 0, rotationSpeed*Time.deltaTime));
        }
    }
}