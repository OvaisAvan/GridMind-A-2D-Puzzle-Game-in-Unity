// Partial extension for GameManager — add this method to GameManager.cs
// or keep as a separate static helper accessed from MainMenuController.

namespace GridMind.Managers
{
    public partial class GameManager
    {
        /// <summary>
        /// Used by the level-select screen to jump to a specific level.
        /// </summary>
        public void CurrentLevelOverride(int levelIndex)
        {
            CurrentLevel = levelIndex;
        }
    }
}
