using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mehanics.Objects
{
    /// <summary>
    /// Set to object in scene. Generation of blocks and shapes
    /// </summary>
    public class BlocksGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject[] blockPrefabs;
        private List<Transform> _targets = new List<Transform>();
        [SerializeField] private float minDelay = 0.5f;
        [SerializeField] private float maxDelay = 5f;
        [SerializeField] private float minScale = 0.2f;
        [SerializeField] private float maxScale = 3f;

        // [SerializeField] private PolygonCollider2D borders;

        private bool _canCreate = true;

        private void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
                _targets.Add(transform.GetChild(i));
        }

        private void LateUpdate()
        {
            if (!_canCreate) return;
            StartCoroutine(CreateBlockIe());
        }
        
        /// <summary>
        /// Creation of blocks
        /// </summary>
        /// <returns></returns>
        private IEnumerator CreateBlockIe()
        {
            _canCreate = false;
            var newBlock = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)],
                _targets[Random.Range(0, _targets.Count)].position,
                Quaternion.identity);
            var rand = Random.Range(minScale, maxScale);
            newBlock.transform.localScale = Vector3.one * rand;
            Destroy(newBlock, Random.Range(6f, 10f));
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            _canCreate = true;
        }
    }
}