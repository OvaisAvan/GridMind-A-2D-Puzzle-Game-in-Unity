using UnityEngine;

namespace GridMind.Grid
{
    public enum TileType
    {
        Floor = 0,
        Wall  = 1,
        Goal  = 2,
        Box   = 3
    }

    /// <summary>
    /// Represents a single cell in the puzzle grid.
    /// Handles its own visual state and type metadata.
    /// </summary>
    public class TileController : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite floorSprite;
        [SerializeField] private Sprite wallSprite;
        [SerializeField] private Sprite goalSprite;
        [SerializeField] private Sprite goalFilledSprite;

        [Header("Colors")]
        [SerializeField] private Color normalColor   = Color.white;
        [SerializeField] private Color highlightColor = new Color(1f, 0.92f, 0.6f);

        public TileType TileType   { get; private set; }
        public Vector2Int GridPos  { get; private set; }
        public bool IsGoalFilled   { get; private set; }

        public void Initialise(int x, int y, TileType type)
        {
            GridPos  = new Vector2Int(x, y);
            TileType = type;
            ApplyVisuals();
        }

        private void ApplyVisuals()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.sprite = TileType switch
            {
                TileType.Wall  => wallSprite,
                TileType.Goal  => goalSprite,
                _              => floorSprite
            };
            spriteRenderer.color = normalColor;
        }

        /// <summary>
        /// Call when a box is pushed onto / off this goal tile.
        /// </summary>
        public void SetGoalFilled(bool filled)
        {
            if (TileType != TileType.Goal) return;
            IsGoalFilled = filled;
            if (spriteRenderer != null)
                spriteRenderer.sprite = filled ? goalFilledSprite : goalSprite;
        }

        public void Highlight(bool on) =>
            spriteRenderer.color = on ? highlightColor : normalColor;
    }
}
