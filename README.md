# 🌆🧟 Ashfall  
**A Third-Person Survival Roguelite built in Unity**


Ashfall is a **systems-driven third-person survival game** set in a collapsing post-apocalyptic city. The player must survive as long as possible while **hunger, oxygen, enemies, and environmental hazards** relentlessly scale with time.

This project emphasizes **clean gameplay architecture**, **modular systems**, and **scalable difficulty** over scripted encounters.

---
## 🎥 Demo
https://www.youtube.com/watch?v=abi_9CmqB2g

---

## 🎮 Core Gameplay Loop

1. Spawn into the city  
2. Explore to find **food, oxygen, and weapons**  
3. Survive zombie attacks while managing resources  
4. Days pass → **difficulty increases**  
5. Player dies → run summary → restart from Day 1  

Ashfall is intentionally designed as a **run-based survival experience**.

---

## 🧠 Design Pillars

- **Systemic Survival** – Hunger, oxygen, fall damage, zombies, scarcity
- **Escalating Pressure** – Each in-game day increases difficulty globally
- **Scarcity** – Pickups **do not respawn** once collected in a run
- **Replayability** – Short runs, fast deaths, constant tension
- **Clean Architecture** – Minimal coupling between systems

---

## 🕹️ Player Systems

### 🚶 Player Movement
- Third-person movement using `CharacterController`
- Camera-relative input
- Sprinting, jumping, air control
- Smooth rotation toward movement direction

### 🧍 Third-Person Camera
- Over-the-shoulder framing
- Mouse-driven yaw & pitch
- Pitch clamping
- Smoothed follow
- Shoulder offset for aiming clarity
- Cursor lock & visibility handling

### 🦶 Footstep Audio
- Dynamic footstep system
- Separate walk/run cadence
- Speed detection via:
  - `CharacterController`
  - `Rigidbody`
  - Input fallback
- Integrated sprint state

---

## ❤️ Survival Stats System

### PlayerStats

The player manages **three survival resources**:

| Stat | Description |
|----|----|
| Health | Damage from zombies, falls, starvation |
| Hunger | Decays over time |
| Oxygen | Decays over time |

**Mechanics**
- Hunger & oxygen drain every second
- If either reaches zero → **health drains**
- Health reaching zero triggers death
- Stats are clamped and event-driven

---

## 🪂 Fall Damage

Two complementary implementations (iterated design):

### Advanced Fall Damage (PlayerMovement)
- Tracks fall start height
- Tracks peak downward velocity
- Applies damage only if:
  - Minimum fall height exceeded
  - Impact speed exceeds safe threshold
- Damage scales with impact speed
- Max damage clamp prevents instant death

### Lightweight Fall Damage (FallDamage.cs)
- Velocity-based approximation
- Uses `CharacterController.isGrounded`
- Linear damage interpolation

---

## 🔫 Weapons & Combat

### PlayerWeapon System
- Runtime weapon equipping
- Raycast hitscan shooting
- Fire-rate limiting
- ADS animation support
- Muzzle flash VFX
- Per-weapon audio
- Self-collision filtering
- Layer-mask aware hit detection

### Gun Pickup
- Rotating pickup
- Trigger-based equip
- Weapon prefab + optional shot sound
- Automatically replaces current weapon

---

## 🧟 Zombie Systems

### Zombie AI
- NavMesh-driven chasing
- Player detection radius
- Smooth facing at close range
- Attack cooldown system
- **Damage scales per day**
- Animator velocity syncing
- Robust player acquisition fallback logic

### Zombie Health
- Damageable entity
- Event-driven death
- Auto-destruction on death

### Zombie Audio
- Ambient groans
- Randomized intervals
- Coroutine-driven playback
- Auto start/stop on enable/disable

---

## 🧠 Enemy Spawning

### ZombieSpawner (NavMesh-aware)
- Multiple spawn points
- Min distance from player
- Max alive cap
- Spawn interval
- NavMesh-snapped spawns
- Auto cleanup of dead zombies

### EnemySpawner (Radius-based)
- Radius-based random spawns
- Per-spawner caps
- Timed despawn
- Lightweight pressure spawner

---

## 🍎 Resource Pickups

### Pickup Types
- **Food**
- **Oxygen**

### PickupSpawnManager
- Static spawn points
- Random prefab selection
- **One-time pickup per run**
- Consumed spawns tracked via stable IDs
- Prevents farming

### Pickup Behavior
- World rotation for visibility
- Trigger-based collection
- Applies stat increases
- Reports consumption to spawn manager
- Self-destructs on pickup

---

## 🖥️ UI & HUD

### HUDController
- Health slider
- Hunger slider
- Oxygen slider
- Day counter
- Live updates from PlayerStats & GameManager

### Death UI
- Pauses time
- Unlocks cursor
- Displays:
  - Day reached
  - Final health
  - Hunger
  - Oxygen
- Restart reloads gameplay scene

---

## 🌅 Game Manager (Run Controller)

### Responsibilities
- Global singleton
- Day progression
- Difficulty scaling
- Scene bootstrapping
- Player death handling
- Run summary generation

### Day System
- Fixed-length days
- Each new day:
  - Hunger decay increases
  - Oxygen decay increases
  - Zombie damage multiplier increases

---

## 🧱 Architecture Notes

- Component-based design
- Event-driven where appropriate
- No monolithic “god script”
- Systems communicate via:
  - Public APIs
  - Events
  - GameManager orchestration

---

## 🛠️ Tech Stack

- **Engine:** Unity
- **Language:** C#
- **AI:** NavMesh Agents
- **UI:** Unity UI + TextMeshPro
- **Audio:** AudioSource-based
- **Input:** Unity Input Manager
- **Architecture:** Modular component systems

---

## 🚧 Project Status

**Implemented**
- Core survival loop
- Player movement & camera
- Zombie AI & spawning
- Resource scarcity
- Weapon system
- Death & restart flow

**Planned**
- More weapons
- Inventory system
- Environmental hazards
- Map expansion
- Meta-progression

---

## 📌 Purpose

Ashfall exists to demonstrate:
- Systems thinking
- Gameplay architecture
- Iterative mechanic design
- Clean Unity engineering
- Long-term scalability

> *Ashfall is designed to break the player, not the code...*
