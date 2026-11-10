**English** | [한국어](README.ko.md)

# Autonomous Boat Simulator

A marine route-driving simulator built in Unity. Steer a boat from the start point through a checkpoint and reach the destination before time or fuel runs out. Press **V** to switch to autonomous mode, where the boat follows waypoints to the goal and avoids obstacles on its own.

Term project for *IT-based Marine Mobility Systems* (Spring 2026), 4-person team.

![Gameplay](docs/screenshot.png)

## My Contribution: Autonomous Navigation & Obstacle Avoidance

I built the autonomous driving feature, [`BoatAutoNav.cs`](come/Assets/Scripts/BoatAutoNav.cs).

![Autonomous mode](docs/autonav.gif)

- **Waypoint following**: Visits scene waypoints in order, then heads to the destination. Arrival radius is 15 m for waypoints and 8 m for the destination.
- **Obstacle avoidance**: Casts 9 rays from the bow (0°, ±20°, ±45°, ±70°, ±90°). A hit on a side ray adds steering toward the opposite side, weighted by proximity (`1 - distance / detectDist`). If only the front ray is blocked, the boat turns right to escape.
- **Steering & speed control**: Combines the heading error (`SignedAngle × 0.05`) with the avoidance bias, clamps it to [-1, 1], and applies it as torque. Angular velocity is damped every physics step to prevent oscillation, and horizontal speed is capped.
- **Manual override**: V toggles autonomous mode. Any WASD input immediately hands control back to the player.
- **Collision detection**: Counts a hit only above an impulse threshold, with a 2-second cooldown so one crash isn't counted several times. Each hit is reported to the HUD to drain fuel.
- **Restart**: R resets position, velocity, fuel and game state in one step.
- **Debug gizmos**: Draws ray directions, the waypoint path and arrival radii in the Scene view for tuning.

## Credits

The ocean environment (sea, islands, Gerstner waves, water rendering) and boat models come from Unity Technologies' demo project [Boat Attack](https://github.com/Unity-Technologies/BoatAttack), licensed under the Unity Companion License (see `come/LICENSE.md`).

On top of it, our team built the course and obstacle layout, the simulation flow, the HUD, autonomous navigation and difficulty levels. See [Project Structure](#project-structure) for which files are ours.

## Rules

| | |
|---|---|
| Goal | Start → pass the checkpoint → reach the destination within 60 s |
| FINISH | Reach the destination ring (`DestinationTrigger`) |
| FAIL | Fuel runs out or time is up |
| Fuel | Drains linearly from 100% to 0% over 60 s; each collision costs an extra 5% |

Flow: `Title → (Space) → Standby → Checkpoint → "START" for 2 s → Timer → FINISH / FAIL`. Press R to restart.

## Controls

| Key | Action |
|---|---|
| Space | Skip title screen |
| W / S | Forward / Reverse |
| A / D | Turn left / right |
| V | Toggle autonomous mode |
| R | Restart |
| T, F | (Debug) Trigger checkpoint / destination |

## Features

### Boat Physics — `BoatController.cs`
Movement uses Rigidbody `AddForce` (forward/reverse) and `AddTorque` (yaw). The vertical component of the forward vector is removed so the boat stays on the water surface, and `ForceMode.Acceleration` gives mass-independent acceleration.

### Ocean Environment
Sea, islands, reefs, buoys, fog and waves. The wave, buoyancy and water rendering systems are from Boat Attack.
- **Gerstner waves**: `Water.cs` generates wave data. The GPU (ComputeBuffer) renders it, and the CPU (`GerstnerWavesJobs`, Burst + `IJobParallelFor`) samples surface height every frame for buoyancy.
- **Water rendering**: The URP Renderer Feature `WaterSystemFeature` runs `WaterFxPass` (foam, normals, displacement) and `WaterCausticsPass`.
- **Fog / Day-night**: `FogOverride.cs`, `DayNightController.cs`
- **Moving obstacles** (team-built): `WindsurferMover.cs` loops windsurfers along waypoints. They're reused through the `Windsurfer_PatrolGroup` prefab, and `Tools > Setup Windsurfer Colliders` adds colliders in one click.

### Simulation Flow — `Simulation_Manager.cs`
Manages screens (title, standby, running, finish/fail) and the timer in one place. `CheckpointTrigger.cs` starts the timer, and `DestinationTrigger.cs` ends the run.

### HUD — `BoatHUD.cs`, `MinimapHUD.cs`

| Element | How it works |
|---|---|
| Speedometer | `Rigidbody.velocity.magnitude × 3.6` → km/h |
| Fuel gauge | Time-based drain plus a penalty per collision; FAIL at 0 |
| HIT counter | Total collisions |
| Compass | Needle follows the boat's yaw; shows 8-point heading (N, NE, E, …) |
| Minimap | Projects obstacle positions relative to the boat as red dots |

### Difficulty Levels
Each checkpoint ring has a `level` value (1 or 2) set in the Inspector, shown on screen as "Level.1" / "Level.2".
- **Level 1**: Wide channel, few obstacles
- **Level 2**: Narrow rocky passage, dense obstacles

![Level 2](docs/level2.png)

## Project Structure

Team-written files (under `come/Assets/`):

| File | Role |
|---|---|
| `Scripts/BoatAutoNav.cs` | Autonomous navigation, obstacle avoidance, restart |
| `BoatController.cs` | WASD manual control |
| `WindsurferMover.cs` | Moving obstacles along waypoints |
| `Scripts/Simulation_Manager.cs` | Game flow and timer |
| `Scripts/CheckpointTrigger.cs` | Checkpoint pass, level |
| `Scripts/DestinationTrigger.cs` | Destination arrival |
| `Scripts/BoatHUD.cs` | Speed, fuel, HIT, compass |
| `Scripts/MinimapHUD.cs` | Minimap |
| `Scripts/Editor/GameSetup.cs`, `HUDSetup.cs`, `CheckpointSetup.cs`, `WindsurferSetup.cs` | Editor `Tools` menu items that auto-build UI, HUD, checkpoints and colliders |

Main files used as-is from Boat Attack: `Scripts/Boat/` (Engine, Boat, …), `Scripts/Environment/` (FogOverride, DayNightController, …), `Packages/com.verasl.water-system/`.

## Tech Stack

- Unity 2022.3.62f3, Universal Render Pipeline
- C#
- Burst / C# Job System (wave sampling, from Boat Attack)

## Getting Started

1. Install Unity **2022.3.62f3**.
2. This repo stores textures and models with Git LFS. Install [Git LFS](https://git-lfs.com) before cloning (ZIP downloads may miss LFS files).
   ```
   git lfs install
   git clone https://github.com/kyuyeon76/autonomous-boat-simulator.git
   ```
3. Open the `come` folder in Unity Hub.
4. Open `Assets/Scenes/_levels/level_Island.unity` and press Play.
