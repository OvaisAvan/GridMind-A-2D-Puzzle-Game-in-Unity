using UnityEngine;
using System.Collections;
using GridMind.Grid;
using GridMind.Managers;

namespace GridMind.Player
{
    /// <summary>
    /// A pushable box. Moves to target grid position when pushed,
    /// notifies goal tiles of its presence.
    /// </summary>
    public class BoxController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color normalColor = new Color(0.8f, 0.55f, 0.25f);
        [SerializeField] private Color onGoalColor = new Color(0.3f, 0.85f, 0.4f);

        public Vector2Int GridPos { get; private set; }
        public bool IsOnGoal     { get; private set; }

        private void Start()
        {
            GridPos = GridManager.Instance.WorldToGrid(transform.position);
            BoxManager.Instance?.Register(this);
            CheckGoal();
        }

        public void PushTo(Vector2Int target)
        {
            // Unmark previous goal
            TileController prev = GridManager.Instance.GetTile(GridPos);
            if (prev != null && prev.TileType == TileType.Goal)
            {
                prev.SetGoalFilled(false);
                IsOnGoal = false;
            }

            GridPos = target;
            StartCoroutine(SlideCoroutine(GridManager.Instance.GridToWorld(target)));
        }

        private IEnumerator SlideCoroutine(Vector3 targetWorld)
        {
            Vector3 start    = transform.position;
            float   elapsed  = 0f;
            float   duration = 1f / moveSpeed;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(start, targetWorld, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = targetWorld;
            CheckGoal();
            AudioManager.Instance?.PlayBoxLand();
        }

        private void CheckGoal()
        {
            TileController tile = GridManager.Instance.GetTile(GridPos);
            if (tile != null && tile.TileType == TileType.Goal)
            {
                tile.SetGoalFilled(true);
                IsOnGoal = true;
                spriteRenderer.color = onGoalColor;
            }
            else
            {
                spriteRenderer.color = normalColor;
            }
        }
    }
}
