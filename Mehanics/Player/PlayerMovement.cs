using UnityEngine;

namespace Mehanics.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Borders for both axis for player velocity")] [SerializeField]
        private float maxVelocity;

        // [Header("Player have physicMaterial2D. It's for Bounciness property")]
        // [Tooltip("Also player can go through ground if he have big velocity")]
        // [SerializeField]
        // private ValuesRange playerBouncinessBorders;

        [HideInInspector] public Rigidbody2D rb;
        // private Collider2D _collider;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            // _collider = GetComponent<PolygonCollider2D>();
            // _collider.sharedMaterial.bounciness = playerBouncinessBorders.MaxValue;
        }

        // private void Update()
        // {
        //     var velocity = rb.velocity;
        //     if (Mathf.Abs(velocity.x) >= maxVelocity || Mathf.Abs(velocity.y) >= maxVelocity)
        //     {
        //         // Debug.Log(velocity);
        //         _collider.sharedMaterial.bounciness = playerBouncinessBorders.MinValue;
        //         rb.velocity = new Vector2(Mathf.Clamp(velocity.x, -maxVelocity, maxVelocity),
        //             Mathf.Clamp(velocity.y, -maxVelocity, maxVelocity));
        //         // Debug.Log("changed: " + rb.velocity);
        //     }
        //     else
        //         _collider.sharedMaterial.bounciness = playerBouncinessBorders.MaxValue;
        // }
    
        private void Update()
        {
            var velocity = rb.velocity;
            if (!(Mathf.Abs(velocity.x) >= maxVelocity) && !(Mathf.Abs(velocity.y) >= maxVelocity)) return;
            rb.velocity = new Vector2(Mathf.Clamp(velocity.x, -maxVelocity, maxVelocity),
                Mathf.Clamp(velocity.y, -maxVelocity, maxVelocity));
        }
    }
}