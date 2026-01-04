using UnityEngine;
using System.Collections;
using GridMind.Grid;
using GridMind.Managers;

namespace GridMind.Puzzle
{
    /// <summary>
    /// Polls after every player move to detect win state.
    /// Fires the win event and triggers UI / next-level flow.
    /// </summary>
    public class WinCondition : MonoBehaviour
    {
        public static WinCondition Instance { get; private set; }

        [SerializeField] private float winDelay = 0.6f;   // brief pause before win screen

        private bool levelComplete;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void ResetForNewLevel() => levelComplete = false;

        /// <summary>
        /// Called by PlayerController after every move completes.
        /// </summary>
        public void CheckWin()
        {
            if (levelComplete) return;

            if (BoxManager.Instance != null && BoxManager.Instance.AllBoxesOnGoals)
            {
                levelComplete = true;
                StartCoroutine(TriggerWin());
            }
        }

        private IEnumerator TriggerWin()
        {
            AudioManager.Instance?.PlayWin();
            yield return new WaitForSeconds(winDelay);

            int moves   = GameManager.Instance.MoveCount;
            int par     = GameManager.Instance.ParMoves;
            int level   = GameManager.Instance.CurrentLevel;

            SaveSystem.Instance?.SaveLevelResult(level, moves, moves <= par);
            UIManager.Instance?.ShowWinPanel(moves, par);

            Debug.Log($"[WinCondition] Level {level} complete in {moves} moves (par: {par}).");
        }
    }
}
