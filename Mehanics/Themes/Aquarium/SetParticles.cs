using System;
using Game_Stuff;
using UnityEngine;

namespace Mehanics.Themes.Aquarium
{
    public class SetParticles : MonoBehaviour
    {
        [SerializeField] private Transform bubblesParticle;
        [SerializeField] private Transform fishesParticle;
        [SerializeField] private Transform backGround;

        private LevelBorders _levelBorders;

        private void Start()
        {
            _levelBorders = FindObjectOfType<LevelBorders>();

            backGround.position = _levelBorders.GetCenter();
            backGround.localScale = new Vector3(_levelBorders.GetBorderLength() / 11.5f,
                _levelBorders.GetBorderLength("y") / 6.5f, 1f);

            InitObject(bubblesParticle);
            InitObject(fishesParticle);
        }

        /// <summary>
        /// Init particles
        /// </summary>
        /// <param name="particle"></param>
        private void InitObject(Transform particle)
        {
            var shape = particle.GetComponent<ParticleSystem>().shape;

            if (particle == bubblesParticle)
            {
                particle.position = _levelBorders.GetXCenter();
                shape.radius = _levelBorders.GetBorderLength();
            }
            else
            {
                particle.position = _levelBorders.GetYCenter();
                shape.radius = _levelBorders.GetBorderLength("y");
            }
        }
    }
}