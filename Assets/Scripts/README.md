# Ashfall (Unity)

A third-person survival prototype set in a post-disaster environment. Manage Health, Hunger, and Oxygen while surviving against zombies and lasting as many in-game days as possible.

## How to Run
1. Open the project in Unity Hub.
2. Open the main gameplay scene (example: `Ashfall_City` or your assigned gameplay scene).
3. Press Play.

> Note: If your project uses a Boot scene that routes into the main scene, you can also press Play from `Boot`.

## Controls
- WASD: Move
- Mouse: Look / Camera
- Space: Jump
- Left Shift: Sprint
- Left Ctrl: Slide (while sprinting, if enabled)

## Core Features
- Player survival stats: Health, Hunger, Oxygen
- Hunger/Oxygen drain over time; health damage when either hits zero
- Pickups that restore Hunger or Oxygen
- Zombie AI chase + attack
- Day progression + difficulty scaling
- Death UI and restart flow

## Credits / Assets
### 3D Assets
- SYNTY Studios (POLYGON asset packs) — character/environment/props

### Audio
- Mixkit (sound effects used in the project)
- Pixabay (sound effects used in the project)

## Acknowledgements (Debugging Help)
- ChatGPT (GPT-5.2 Thinking) — assisted with debugging and troubleshooting issues such as:
  - UI stat bars / slider max-value and fill behavior problems (health/hunger/oxygen display syncing) 
  - Audio hookup issues (AudioSource references and missing variable/field errors in scripts)
  - Third-person controller + camera alignment issues (movement direction vs camera facing)
  - General Unity console error triage (NullReferenceException, missing references, prefab/script setup)
