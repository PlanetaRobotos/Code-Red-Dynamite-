using Cinemachine;
using UnityEngine;

namespace Core
{
    public class VirtualCameraManager : MonoBehaviour
    {
        private Transform _target;
        private PolygonCollider2D _polygon;


        private void Awake()
        {
            _polygon = GameObject.Find("LevelBorders").GetComponent<PolygonCollider2D>();
            _target = GameObject.FindGameObjectWithTag(Tags.s_Player).transform;
            GetComponent<CinemachineVirtualCamera>().m_Follow = _target;
            GetComponent<CinemachineConfiner>().m_BoundingShape2D = _polygon;
        }
    }
}