# Ashfall (Unity)

A third-person wave-based survival prototype set in a post-disaster environment. Manage Health, Hunger, and Oxygen while surviving against zombies and lasting as many in-game days as possible.

## How to Run
1. Open the project in Unity Hub
2. Open the "Ashfall_boot" scene
3. Press Play

## Controls
- WASD: Move
- Mouse: Look / Camera
- Space: Jump
- Left Shift: Sprint

## Core Features
- Player survival stats: Health, Hunger, Oxygen
- Hunger/Oxygen drain over time. Health damage when either hits zero
- Pickups that restore Hunger, Oxygen, Ammo, & Health
- Zombie AI patrol + chase + attack
- Day (Wave) progression + difficulty scaling
- Death UI and restart flow

## Credits / Assets
### 3D Assets
- SYNTY Studios (POLYGON asset packs) — character/environment/props

### Animations
- Base locomotion for player and zombies were sourced from SYNTY
- Mixamo.com for PistolIdle, RifleIdle, & RifleFire animations

### Audio
- Mixkit (sound effects used in the project)
- Pixabay (sound effects used in the project)
- Freesound (sound effects used in the project)

## Acknowledgements (Debugging Help)
In addition to the class lecture slides, the following online resources were crucial in debugging and troubleshooting:

- Unity Documentation & Unity Learn  
  https://docs.unity3d.com  
  https://learn.unity.com  

- Stack Overflow & Game Dev StackExchange  
  https://stackoverflow.com  
  https://gamedev.stackexchange.com  

- Community discussions on Reddit (r/Unity3D)  
  https://www.reddit.com/r/Unity3D/

- Educational YouTube channels including:
  - Brackeys — https://www.youtube.com/@Brackeys
  - Sebastian Lague — https://www.youtube.com/@SebastianLague
  - Code Monkey — https://www.youtube.com/@CodeMonkeyUnity

- ChatGPT (OpenAI) — provided debugging assistance for:
  - Interpreting Unity console errors (e.g., NullReferenceException and missing references)
  - Verifying script-to-prefab and inspector wiring during troubleshooting

These resources provided guidance on UI synchronization issues, AudioSource setup, third-person camera alignment, and general Unity error debugging.
