using UnityEngine;

namespace Mehanics.Objects
{
    public class RotationMill : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;

        private void Update() => 
            transform.Rotate(new Vector3(0, 0, rotationSpeed*Time.deltaTime));
    }
}