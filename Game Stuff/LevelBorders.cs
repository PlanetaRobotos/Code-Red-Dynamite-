using UnityEditor;
using UnityEngine;

namespace Game_Stuff
{
    public class LevelBorders : MonoBehaviour
    {
        [Header("Borders for polygon")] [SerializeField]
        private Vector2 minPosition;

        public Vector2 MinPosition => minPosition;

        [SerializeField] private Vector2 maxPosition;

        public Vector2 MaxPosition => maxPosition;

        private PolygonCollider2D _polygon;

        private void SetPolygonBorders()
        {
            _polygon = GetComponent<PolygonCollider2D>();
            var points = new Vector2[4];

            points[0] = minPosition;
            points[1] = new Vector2(minPosition.x, maxPosition.y);
            points[2] = maxPosition;
            points[3] = new Vector2(maxPosition.x, minPosition.y);
            _polygon.points = points;
        }

        #region Classic Methods

        public float GetBorderLength(string side = "x")
            => Vector2.Distance(minPosition,
                side is "x" ? new Vector2(maxPosition.x, minPosition.y) : new Vector2(minPosition.x, maxPosition.y))/2f;

        public Vector2 GetCenter() =>
            new Vector2((minPosition.x + maxPosition.x) / 2, (minPosition.y + maxPosition.y) / 2);

        public Vector2 GetXCenter() =>
            new Vector2((minPosition.x + maxPosition.x) / 2, minPosition.y);

        public Vector2 GetYCenter(string side = "left") =>
            new Vector2(side is "left" ? minPosition.x : maxPosition.x, (minPosition.y + maxPosition.y) / 2);

        #endregion


#if UNITY_EDITOR
        [CustomEditor(typeof(LevelBorders))]
        internal class RoomEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                GUILayout.Space(10);
                var room = (LevelBorders) target;
                if (GUILayout.Button("Set Polygon Borders"))
                    room.SetPolygonBorders();
            }
        }
#endif
    }
}