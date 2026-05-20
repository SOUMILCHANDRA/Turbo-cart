# Turbo Kart - Anti-Gravity Racing System (Modular Edition)

An arcade-style, high-performance anti-gravity kart racing controller for Unity 3D. Designed to handle extreme track geometry (including vertical wall sections, full 360-degree loops, corkscrews, and upside-down ceilings) with stability, zero gimbal lock, and a smooth camera follow.

---

## 📂 Project Architecture

```
Assets/
├── Editor/
│   └── TrackBuilderEditor.cs     # Custom Inspector tool for snap-snapping track pieces
└── Scripts/
    ├── Camera/
    │   └── AntiGravityCamera.cs  # Smooth follow camera matching kart local Up vector
    ├── Data/
    │   └── KartSettings.cs       # ScriptableObject containing physical tuning parameters
    ├── Input/
    │   └── PlayerInputReader.cs  # Local keyboard/controller input reading
    ├── Interfaces/
    │   └── IKartInput.cs         # Decoupled input interface for player/AI/network
    ├── Movement/
    │   ├── DriftSystem.cs        # Slide steering, grip overrides, and drift-boost charge
    │   └── KartMovement.cs       # Engine acceleration, braking, speed-based yaw, lateral grip
    ├── Physics/
    │   └── AntiGravityPhysics.cs # Raycasting, spring-damper suspension, magnetic gravity downforce
    └── Track/
        ├── BoostPad.cs           # Acceleration trigger zones
        └── TrackPiece.cs         # Modular segment snap registration component
```

---

## 🚀 Script Connections

- **`IKartInput` / `PlayerInputReader`** captures input commands (`Throttle`, `Steering`, `IsDrifting`).
- **`AntiGravityPhysics`** determines whether the kart is grounded, calculates the average track normal, manages suspension height, aligns the kart's orientation, and applies custom speed-dependent magnetic gravity to pull the kart down onto the track.
- **`KartMovement`** applies motor forces relative to the kart's local orientation and uses input steering to rotate karts around their local Up vector. It applies lateral counter-slip force (grip) to stop karts from sliding off high-speed turns.
- **`DriftSystem`** overrides lateral grip and steering multipliers in `KartMovement` when drifting, accumulates drift charge, and releases a forward speed boost when the drift action concludes.
- **`AntiGravityCamera`** aligns its position behind the kart and uses the kart's local Up vector to remain oriented during vertical loops, avoiding gimbal lock.
- **`BoostPad`** overrides speed limits and pushes karts forward when they enter trigger volumes.
- **`TrackPiece`** identifies snap locations to assist with modular track layouts.

---

## 🛠️ Editor Setup Guide

### 1. Prefab Setup
1. Create a parent GameObject called **Kart**.
2. Add a **Rigidbody** to the parent.
   - Set **Use Gravity** to `false`.
   - Set **Interpolation** to `Interpolate`.
   - Set **Collision Detection** to `Continuous`.
3. Attach the following components:
   - `PlayerInputReader`
   - `AntiGravityPhysics`
   - `KartMovement`
   - `DriftSystem`
4. Create 4 empty child objects at the bottom corners of your kart prefab (e.g. Front-Left, Front-Right, Rear-Left, Rear-Right). Drag these transforms into the **Suspension Anchors (Ground Points)** list on `AntiGravityPhysics`.
5. Select a `Track` layer and assign it as the **Track Layer** in the settings.

### 2. Creating Tuning Assets
1. Right-click in your Asset window and choose `Create > TurboKart > Settings`.
2. Assign the newly created `KartSettings` asset to the `Settings` variable on your kart's `AntiGravityPhysics` component.
3. Tweak variables like **Hover Height**, **Hover Snappiness**, and **Magnetic Adhesion (Speed Adhesion Factor)**.

### 3. Track Setup
- Ensure all track pieces have **Mesh Colliders** and are set to the designated `Track` layer.

### 4. Modular Snapping Usage
1. Open a track prefab or object that has the `TrackPiece` component.
2. Select it in the Hierarchy.
3. In the Inspector, drag another track prefab into the **Next Piece Prefab** slot of the **Rapid Track Builder** window.
4. Click **Append Next Piece** to snap the new piece to the current one.

---

This project is designed for arcade racing enthusiasts and developers. Feel free to use and modify it for your futuristic racing projects!

## Author

Soumil Chandra
