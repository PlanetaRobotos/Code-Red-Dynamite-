using System;
using System.Linq;
using Core;
using Core.DataObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Game_Stuff
{
    public class HolesSpawn : MonoBehaviour
    {
        [SerializeField] private ValuesRange holesRange;
        [SerializeField] private ValuesRange holeScaleRange;
        [SerializeField] private GameObject holePrefab;
        [SerializeField] private float minDistance = 5f;

        private LevelBorders _levelBorders;
        private Vector2 _prevPosition = Vector2.zero;

        private void Start()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0) return;

            _levelBorders = FindObjectOfType<LevelBorders>();

            for (int i = 0; i < holesRange.GetRandom(); i++)
            {
                CreateHole();
            }
        }

        /// <summary>
        /// All loic to create a hole
        /// </summary>
        private void CreateHole()
        {
            bool canStay = false;
            Vector2 position = GetRandomPosition();

            RaycastHit2D[] hits = new RaycastHit2D[5];
            Physics2D.RaycastNonAlloc(position, Vector2.zero, hits, 5f,
                LayerMask.GetMask("Default"));

            if (hits.Any(hit => hit.collider && !hit.collider.CompareTag(Tags.s_Ground))) 
                canStay = true;

            float dist = Vector2.Distance(_prevPosition, position);
            int counter = 0;
            
            while (dist <= minDistance && counter < 100 && !canStay)
            {
                position = GetRandomPosition();
                
                Physics2D.RaycastNonAlloc(position, Vector2.zero, hits, 5f,
                    LayerMask.GetMask("Default"));

                if (hits.Any(hit => hit.collider && !hit.collider.CompareTag(Tags.s_Ground))) 
                    canStay = true;
                
                foreach (var hit2D in hits)
                {
                    if(hit2D.collider.CompareTag(Tags.s_Ground))
                        Debug.Log(hit2D.collider.tag);
                }
                
                counter++;
            }

            if (counter == 100)
                Debug.Log($"counter {counter}");

            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
            Transform newHole = Instantiate(holePrefab, position, rotation).transform;
            newHole.SetParent(transform);
            newHole.localScale = Vector3.one * holeScaleRange.GetRandom();

            _prevPosition = position;

            Vector2 GetRandomPosition() =>
                new Vector2(Random.Range(_levelBorders.MinPosition.x, _levelBorders.MaxPosition.x),
                    Random.Range(_levelBorders.MinPosition.y, _levelBorders.MaxPosition.y));
        }
    }
}