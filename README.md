# 🧩 GridMind

> A clean, extensible **2D puzzle game** built in Unity — push boxes onto goals, beat par, master every level.

![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-black?logo=unity)
![License](https://img.shields.io/badge/license-MIT-green)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Mac%20%7C%20WebGL-blue)

---

## 🎮 Gameplay

GridMind is a **Sokoban-style** puzzle game. You control a character on a tile grid and push boxes onto highlighted goal tiles. The twist: every level has a **par move count** — can you solve it efficiently?

```
┌──────────────┐
│  # # # # # # │   # = Wall
│  # . . . . # │   . = Floor
│  # . □ . . # │   □ = Box
│  # . . ★ . # │   ★ = Goal
│  # . . . . # │   P = Player
│  # # # # # # │
└──────────────┘
```

**Controls**

| Action       | Keyboard          | Mobile     |
|--------------|-------------------|------------|
| Move         | WASD / Arrow Keys | Swipe      |
| Pause        | Escape            | Pause btn  |
| Restart      | R                 | Restart btn|

---

## 🏗️ Project Structure

```
GridMind/
├── Assets/
│   ├── Scripts/
│   │   ├── Grid/
│   │   │   ├── GridManager.cs          ← Grid logic & coordinate system
│   │   │   ├── TileController.cs       ← Individual tile behaviour
│   │   │   └── BoxManager.cs           ← Tracks all boxes, win detection helper
│   │   ├── Player/
│   │   │   ├── PlayerController.cs     ← Input (keyboard + swipe) & movement
│   │   │   └── BoxController.cs        ← Pushable box, goal detection
│   │   ├── Puzzle/
│   │   │   ├── LevelLoader.cs          ← Loads JSON levels from Resources/
│   │   │   └── WinCondition.cs         ← Post-move win check & event fire
│   │   ├── Managers/
│   │   │   ├── GameManager.cs          ← State machine, level flow, move count
│   │   │   ├── SaveSystem.cs           ← PlayerPrefs save (best moves, par, unlock)
│   │   │   └── AudioManager.cs         ← Music & SFX with volume persistence
│   │   └── UI/
│   │       ├── UIManager.cs            ← HUD, win panel, pause panel
│   │       └── MainMenuController.cs   ← Menu nav, level select, settings
│   └── Resources/
│       └── Levels/
│           ├── level_01.json           ← "First Steps"   — 1 box, par 4
│           ├── level_02.json           ← "Double Trouble" — 2 boxes, par 10
│           └── level_03.json           ← "The Gauntlet"  — 3 boxes, par 20
├── .gitignore
└── README.md
```

---

## 🚀 Getting Started

### Requirements
- **Unity 2022.3 LTS** or newer
- **TextMeshPro** (included with Unity)
- No third-party packages required

### Open the Project
1. Clone the repo:
   ```bash
   git clone https://github.com/YOUR_USERNAME/GridMind.git
   ```
2. Open **Unity Hub** → **Open Project** → select the `GridMind/` folder.
3. Unity will import assets automatically.
4. Open `Assets/Scenes/MainMenu.unity` and hit **Play**.

### Scenes
| Scene | Purpose |
|-------|---------|
| `MainMenu` | Title screen, level select, settings |
| `Game`     | Active gameplay scene |

---

## 📦 Adding New Levels

Levels are plain JSON files in `Assets/Resources/Levels/`.

### Tile Codes
| Code | Tile  |
|------|-------|
| `0`  | Floor |
| `1`  | Wall  |
| `2`  | Goal  |

### Level JSON Schema
```json
{
  "levelName": "My Level",
  "width": 6,
  "height": 6,
  "tiles": [
    1, 1, 1, 1, 1, 1,
    1, 0, 0, 0, 0, 1,
    1, 0, 0, 0, 0, 1,
    1, 0, 0, 2, 0, 1,
    1, 0, 0, 0, 0, 1,
    1, 1, 1, 1, 1, 1
  ],
  "playerStart": [1, 1],
  "boxPositions": [[2, 2]],
  "parMoves": 4
}
```

> **Tip:** The `tiles` array is **row-major from top-left**, so `tiles[y * width + x]` = tile at (x, y).

1. Name your file `level_04.json`, `level_05.json`, etc.
2. Place it in `Assets/Resources/Levels/`.
3. Increment `totalLevels` in the `GameManager` Inspector.
4. Done — no code changes required!

---

## 🏛️ Architecture Overview

```
GameManager (DontDestroyOnLoad)
    │
    ├── LevelLoader ──► Resources/Levels/*.json
    │       │
    │       ├── GridManager   (tile grid, coordinate helpers)
    │       ├── BoxManager    (box registry, win helper)
    │       └── PlayerController (input → movement → WinCondition)
    │
    ├── SaveSystem   (PlayerPrefs, best moves, unlock chain)
    ├── AudioManager (music + SFX, volume prefs)
    └── UIManager    (HUD, win panel, pause panel)
```

All singleton managers survive scene loads. `LevelLoader` and scene-specific scripts are destroyed on scene unload.

---

## 🤝 Contributing

Pull requests are welcome!

1. Fork the repo
2. Create a feature branch: `git checkout -b feature/my-cool-thing`
3. Commit your changes: `git commit -m "Add my cool thing"`
4. Push and open a PR

### Ideas for contributions
- New levels (see "Adding New Levels" above — easiest contribution!)
- Undo move system
- Level editor UI inside the game
- Animated tile effects / particle systems
- WebGL build & itch.io integration
- Localization support

---

## 📄 License

MIT © 2025 — free to use, modify, and distribute. See [LICENSE](LICENSE) for details.

---

## 🙏 Acknowledgements

Inspired by the classic [Sokoban](https://en.wikipedia.org/wiki/Sokoban) puzzle game (1981) by Hiroyuki Imabayashi.
