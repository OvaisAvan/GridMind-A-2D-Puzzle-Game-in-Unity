using UnityEngine;
using System.Collections.Generic;
using GridMind.Player;

namespace GridMind.Grid
{
    /// <summary>
    /// Singleton that tracks every BoxController in the active level.
    /// Provides fast lookup by grid position.
    /// </summary>
    public class BoxManager : MonoBehaviour
    {
        public static BoxManager Instance { get; private set; }

        private readonly List<BoxController> boxes = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Register(BoxController box)
        {
            if (!boxes.Contains(box)) boxes.Add(box);
        }

        public void Unregister(BoxController box) => boxes.Remove(box);

        public BoxController GetBoxAt(Vector2Int pos) =>
            boxes.Find(b => b.GridPos == pos);

        public int TotalBoxes       => boxes.Count;
        public int BoxesOnGoal      => boxes.FindAll(b => b.IsOnGoal).Count;
        public bool AllBoxesOnGoals => TotalBoxes > 0 && BoxesOnGoal == TotalBoxes;

        public void Clear()
        {
            boxes.Clear();
        }
    }
}
