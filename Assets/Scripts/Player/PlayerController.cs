using UnityEngine;
using System.Collections;
using GridMind.Grid;
using GridMind.Managers;
using GridMind.Puzzle;

namespace GridMind.Player
{
    /// <summary>
    /// Handles player input and translates it into grid-based movement.
    /// Supports keyboard (WASD / Arrow keys) and touch swipe input.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float swipeThreshold = 50f;

        private Vector2Int currentGridPos;
        private bool isMoving;
        private Animator animator;

        // Touch tracking
        private Vector2 touchStartPos;
        private bool trackingSwipe;

        // Animation hashes
        private static readonly int HashMoveX = Animator.StringToHash("MoveX");
        private static readonly int HashMoveY = Animator.StringToHash("MoveY");
        private static readonly int HashIsMoving = Animator.StringToHash("IsMoving");

        private void Awake() => animator = GetComponent<Animator>();

        private void Start()
        {
            currentGridPos = GridManager.Instance.WorldToGrid(transform.position);
            GridManager.Instance.SetPlayerPosition(currentGridPos);
        }

        private void Update()
        {
            if (isMoving || GameManager.Instance.CurrentState != GameState.Playing) return;

            Vector2Int dir = GetInputDirection();
            if (dir != Vector2Int.zero)
                TryMove(dir);
        }

        // ── Input ─────────────────────────────────────────────────────────────

        private Vector2Int GetInputDirection()
        {
            // Keyboard
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))    return Vector2Int.up;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))  return Vector2Int.down;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  return Vector2Int.left;
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) return Vector2Int.right;

            // Touch / Swipe
            return GetSwipeDirection();
        }

        private Vector2Int GetSwipeDirection()
        {
            if (Input.touchCount == 0) return Vector2Int.zero;

            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos  = touch.position;
                    trackingSwipe  = true;
                    break;

                case TouchPhase.Ended when trackingSwipe:
                    trackingSwipe = false;
                    Vector2 delta = touch.position - touchStartPos;
                    if (delta.magnitude < swipeThreshold) break;
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                        return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
                    else
                        return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
            }
            return Vector2Int.zero;
        }

        // ── Movement ──────────────────────────────────────────────────────────

        private void TryMove(Vector2Int direction)
        {
            Vector2Int targetPos = currentGridPos + direction;

            // Check for box push
            BoxController box = BoxManager.Instance?.GetBoxAt(targetPos);
            if (box != null)
            {
                Vector2Int boxTarget = targetPos + direction;
                if (!GridManager.Instance.IsWalkable(boxTarget) || BoxManager.Instance.GetBoxAt(boxTarget) != null)
                    return; // Can't push
                box.PushTo(boxTarget);
            }

            if (!GridManager.Instance.IsWalkable(targetPos)) return;

            GameManager.Instance.RegisterMove();
            StartCoroutine(MoveCoroutine(targetPos, direction));
        }

        private IEnumerator MoveCoroutine(Vector2Int targetGridPos, Vector2Int dir)
        {
            isMoving = true;
            animator.SetBool(HashIsMoving, true);
            animator.SetFloat(HashMoveX, dir.x);
            animator.SetFloat(HashMoveY, dir.y);

            Vector3 startWorld  = transform.position;
            Vector3 targetWorld = GridManager.Instance.GridToWorld(targetGridPos);
            float elapsed = 0f;
            float duration = 1f / moveSpeed;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startWorld, targetWorld, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = targetWorld;
            currentGridPos = targetGridPos;
            GridManager.Instance.SetPlayerPosition(currentGridPos);

            animator.SetBool(HashIsMoving, false);
            isMoving = false;

            AudioManager.Instance?.PlayStep();
            WinCondition.Instance?.CheckWin();
        }

        public void Teleport(Vector2Int gridPos)
        {
            currentGridPos     = gridPos;
            transform.position = GridManager.Instance.GridToWorld(gridPos);
            GridManager.Instance.SetPlayerPosition(gridPos);
        }
    }
}
