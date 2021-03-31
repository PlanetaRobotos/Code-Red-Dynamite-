using UnityEngine;

namespace Mehanics.Objects
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private ParticleSystem deathParticle;

        private void OnCollisionEnter2D(Collision2D other)
        {
            var newDeath = Instantiate(deathParticle, transform.position, Quaternion.identity);
            Destroy(gameObject, .01f);
            Destroy(newDeath, newDeath.main.startLifetimeMultiplier);
        }
    }
}